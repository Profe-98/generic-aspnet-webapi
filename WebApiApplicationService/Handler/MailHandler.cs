using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit;
using MailKit.Search;
using MailKit.Security;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using System.Net.Sockets;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Collections.Concurrent;

namespace WebApiApplicationService.Handler
{
    public class MailHandler : IMailHandler
    {
        private ConcurrentDictionary<string, ConcurrentQueue<MimeMessage>> _queue = new ConcurrentDictionary<string, ConcurrentQueue<MimeMessage>>();

        public ConcurrentDictionary<string, ConcurrentQueue<MimeMessage>> Queue
        {
            get
            {
                return _queue;
            }
        }

        public void Enqueue(string key, MimeMessage mail)//add a MimeMessage to List by given key
        {
            if(!_queue.ContainsKey(key))
            {
                _queue.TryAdd(key, new ConcurrentQueue<MimeMessage>());
            }
            _queue[key].Enqueue(mail);
        }
        
        public MimeMessage Dequeue(string key)//returns the first item of List<MimeMessage> by given key and remove the item of list
        {
            if(_queue.ContainsKey(key))
            {
                    _queue[key].TryDequeue(out MimeMessage mail);
                return mail;
            }
            return null;
        }
        public MimeMessage Peek(string key)//return the first item of List<MimeMessage> by given key
        {
            if (_queue.ContainsKey(key))
            {
                _queue[key].TryPeek(out MimeMessage mail);
                return mail;
            }
            return null;
        }

        private IMailFolder _currentFolder = null;
        public readonly string ServerImap;
        public readonly int PortImap;
        public readonly SecureSocketOptions SecureSocketOptionsImap;
        protected string UserImap;
        protected string PasswordImap;
        public readonly int TimeoutImap;
        public readonly string LoggerFileImap;

        public readonly string ServerSmtp;
        public readonly int PortSmtp;
        public readonly SecureSocketOptions SecureSocketOptionsSmtp;
        protected string UserSmtp;
        protected string PasswordSmtp;
        public readonly int TimeoutSmtp;
        public readonly string LoggerFileSmtp;

        private readonly ILogger<MailHandler> _logger;

        public IMailFolder CurrentFolder
        {
            get
            {
                return _currentFolder;
            }
            set
            {
                _currentFolder = value;
            }
        }

        public MailHandler(IAppconfig appconfig)
        {
            _logger = new LoggerFactory().CreateLogger<MailHandler>();

            ServerImap = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.Server;
            PortImap = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.Port;
            SecureSocketOptionsImap = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.SecureSocketOptions;
            
            TimeoutImap = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.Timeout;
            LoggerFileImap = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.LoggerFile;

            ServerSmtp = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.Server;
            PortSmtp = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.Port;
            SecureSocketOptionsSmtp = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.SecureSocketOptions;
            
            TimeoutSmtp = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.Timeout;
            LoggerFileSmtp = appconfig.AppServiceConfiguration.MailConfigurationModel.ImapSettings.LoggerFile;
            Init();
        }

        private async void Init()
        {
            _logger.Logging(LogLevel.Information, "mailhandler init, smtp & imap services are avaible when are configured", MethodBase.GetCurrentMethod(), this.GetType().Name);

        }
        public byte[] GetAttachmentBinary(MimeEntity x)
        {
            using (var measure = new MemoryStream())
            {
                if (x is MessagePart)
                {
                    var rfc822 = (MessagePart)x;
                    rfc822.Message.WriteTo(measure);
                }
                else if (x is MimePart)
                {
                    var part = (MimePart)x;
                    part.Content.DecodeTo(measure);
                }
                else if (x is Multipart)
                {
                    var part = (Multipart)x;
                    var bodyContent = part;
                    
                }
                byte[] response = measure.ToArray();
                return response;
            }
        }
        /// <summary>
        /// Returns the Size in kB from a given MimeEntity
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public long GetMimePartSize(MimeEntity x)
        {
            var binary = GetAttachmentBinary(x);
            return binary.Length / 1000;
        }

        public async Task<Dictionary<string,MimeEntity>> GetMimeMessageAttachements(string storeFolder,MimeMessage message)
        {
            Dictionary<string, MimeEntity> response = new Dictionary<string, MimeEntity>();
            foreach(var x in message.Attachments.ToList())
            {
                using (EncryptionHandler encryp = new EncryptionHandler())
                {
                    
                    string fileName = "tmp_"+(x.ContentDisposition?.FileName ?? x.ContentType.Name ?? ""+DateTime.Now.Ticks);

                    string path = Path.Combine(storeFolder, fileName);
                    using (FileStream stream = File.Create(path))
                    {
                        if (x is MessagePart)
                        {
                            var rfc822 = (MessagePart)x;

                            rfc822.Message.WriteTo(stream);
                        }
                        else
                        {
                            var part = (MimePart)x;

                            part.Content.DecodeTo(stream);
                        }
                    }
                    var s = File.OpenRead(path);

                    string hash = await encryp.MD5Async(s);
                    if(!response.ContainsKey(hash))
                        response.Add(path, x);

                    s.Close();
                    s.Dispose();
                }
            }
            return response;
        }
        public async Task<IMailFolder> GetMailFolder(string folderName, string user = null, string password = null, ImapClient imapClient = null)
        {
            bool wasNotInit = imapClient != null;
            imapClient = wasNotInit ? imapClient : await CreateImapConnection(user, password);
            IMailFolder mailFolder = await imapClient.GetFolderAsync(folderName);

            return mailFolder;
        }
        public async Task<Dictionary<UniqueId, MimeMessage>> DownloadMails(SearchQuery searchQuery,string folderName = null,string user = null, string password = null, ImapClient imapClient = null)
        {
            bool wasNotInit = imapClient != null;
            Dictionary<UniqueId, MimeMessage> response = new Dictionary<UniqueId, MimeMessage>();
            try
            {
                imapClient = wasNotInit ? imapClient : await CreateImapConnection(user, password);
                IMailFolder mailFolder = folderName == null ?
                        imapClient.Inbox : await imapClient.GetFolderAsync(folderName);

                FolderAccess acces = mailFolder.Open(FolderAccess.ReadOnly);
                CurrentFolder = mailFolder;
                
                IList<UniqueId> uids = mailFolder.Search(searchQuery);
                /*System.Collections.Concurrent.BlockingCollection<UniqueId> threadSafeList = new System.Collections.Concurrent.BlockingCollection<UniqueId>();
                uids.ToList().ForEach(x => threadSafeList.Add(x));
                threadSafeList..ForEach(async (x) => response.Add(x, await Utils.CallAsyncFunc<UniqueId, MimeMessage>(x, async (x) => await mailFolder.GetMessageAsync(x))));
                */
                foreach(UniqueId uid in uids)
                {
                    var mime = await mailFolder.GetMessageAsync(uid);
                    response.Add(uid,mime);
                }
                mailFolder.Close();
                if (!wasNotInit)
                    imapClient.Disconnect(true);
            }
            catch(Exception ex)
            {

            }
            return response;
        }

        public async Task<List<UniqueId>> SearchInFolder(IMailFolder mailFolder,SearchQuery searchQuery, string user = null, string password = null, ImapClient imapClient = null)
        {
            List<UniqueId> response = new List<UniqueId>();
            bool wasNotInit = imapClient != null;
            imapClient = wasNotInit ? imapClient : await CreateImapConnection(user, password);
            FolderAccess acces = mailFolder.Open(FolderAccess.ReadOnly);

            IList<UniqueId> uids = mailFolder.Search(searchQuery);
            mailFolder.Close();
            if (!wasNotInit)
                imapClient.Disconnect(true);
            return response;
        }
        public async Task<List<IMailFolder>> DeleteFolder(IMailFolder mailFolder, string user = null, string password = null, ImapClient imapClient = null)
        {
            return await DeleteFolders(new List<IMailFolder> { mailFolder },user,password, imapClient);
        }
        public async Task<List<IMailFolder>> DeleteFolders(List<IMailFolder> mailFolder, string user = null, string password = null, ImapClient imapClient = null)
        {
            bool wasNotInit = imapClient != null;
            imapClient = wasNotInit ? imapClient : await CreateImapConnection(user, password);
            mailFolder.ForEach(async (x) =>
            {
                try
                {
                    FolderAccess folderAccessSource = x.Open(FolderAccess.ReadWrite);
                    await x.DeleteAsync();
                    mailFolder.Remove(x);
                    x.Close();
                }
                catch (Exception ex)
                {

                }

            });
            if (!wasNotInit)
                imapClient.Disconnect(true);
            return mailFolder;
        }
        public async Task<List<UniqueId>> DeleteMail(IMailFolder mailFolder, UniqueId uniqueId, string user = null, string password = null, ImapClient imapClient = null)
        {

            return await MarkMailsAs(mailFolder, new List<UniqueId> { uniqueId }, MessageFlags.Deleted, user, password,imapClient);
        }
        public async Task<List<UniqueId>> DeleteMails(IMailFolder mailFolder, List<UniqueId> uniqueIds, string user = null, string password = null, ImapClient imapClient = null)
        {

            return await MarkMailsAs(mailFolder, uniqueIds, MessageFlags.Deleted, user, password,imapClient);
        }
        public async Task<MimeMessage> SendMail(MimeMessage mimeMessage, string user = null, string password = null, SmtpClient smtpClient = null)
        {
            List<MimeMessage> messages = (await SendMails(new List<MimeMessage> { mimeMessage }, user, password, smtpClient));
            return messages.Count==0?null: messages[0];
        }
        public async Task<List<MimeMessage>> SendMails(List<MimeMessage> mimeMessages, string user = null, string password = null, SmtpClient smtpClient = null)
        {
            bool wasNotInit = smtpClient != null;
            try
            {
                smtpClient = wasNotInit ? smtpClient : await CreateSmtpConnection(user, password);
                mimeMessages.ForEach(async (x) =>
                {
                    try
                    {
                        await smtpClient.SendAsync(x);
                    }
                    catch (Exception ex)
                    {

                    }

                });
                if (!wasNotInit)
                    smtpClient.Disconnect(true);
            }
            catch(Exception ex)
            {

            }
            return mimeMessages;
        }
        public MimeMessage CreateMimeMessage(InternetAddressList to, InternetAddressList cc, MailboxAddress sender,string subject,string messageStr)
        {
            MimeMessage message = new MimeMessage();
            if (to.Count > 0)
            {
                message.To.AddRange(to);
            }
            if (cc != null && cc.Count > 0)
            {
                message.Cc.AddRange(cc);
            }
            if (sender != null)
            {
                message.From.Add(sender);
            }
            else
                throw new Exception();
            message.Sender = sender;

            message.Subject = subject;


            using (var quoted = new StringWriter())
            {
                quoted.Write(messageStr);

                message.Body = new TextPart("plain")
                {
                    Text = quoted.ToString()
                };
            }
            return message;
        }
        public MimeMessage PrepareReply(MimeMessage message,string replyMessage,bool replyToAll,string customSubject = null, string user = null, string password = null)
        {
            MimeMessage reply = new MimeMessage();
            // reply to the sender of the message
            if (message.ReplyTo.Count > 0)
            {
                reply.To.AddRange(message.ReplyTo);
            }
            else if (message.From.Count > 0)
            {
                reply.To.AddRange(message.From);
            }
            else if (message.Sender != null)
            {
                reply.To.Add(message.Sender);
            }

            if (replyToAll)
            {
                reply.To.AddRange(message.To);
                reply.Cc.AddRange(message.Cc);
            }
            if(customSubject == null)
            {
                if (!message.Subject.StartsWith("Aw:", StringComparison.OrdinalIgnoreCase))
                    reply.Subject = "Aw:" + message.Subject;
                else
                    reply.Subject = message.Subject;
            }
            else
            {
                reply.Subject = customSubject;
            }

            if (!string.IsNullOrEmpty(message.MessageId))
            {
                reply.InReplyTo = message.MessageId;
                foreach (var id in message.References)
                    reply.References.Add(id);
                reply.References.Add(message.MessageId);
            }

            using (var quoted = new StringWriter())
            {
                var sender = message.Sender ?? message.From.Mailboxes.FirstOrDefault();
                quoted.Write(replyMessage + "\n");
                quoted.WriteLine("Am {0}, {1} schrieb:", message.Date.ToString("f"), !String.IsNullOrEmpty(sender.Name) ? sender.Name : sender.Address);
                using (var reader = new StringReader(message.TextBody))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        quoted.Write("> ");
                        quoted.WriteLine(line);
                    }
                }

                reply.Body = new TextPart("plain")
                {
                    Text = quoted.ToString()
                };
            }
            return reply;
        }
        public async Task<List<UniqueId>> MoveMails(IMailFolder source,List<UniqueId> uniqueIds, IMailFolder destination, string user = null, string password = null, ImapClient imapClient = null)
        {
            bool wasNotInit = imapClient != null;
            List<UniqueId> response = new List<UniqueId>();
            imapClient = wasNotInit ? imapClient : await CreateImapConnection(user, password);
            FolderAccess folderAccessSource = await source.OpenAsync(FolderAccess.ReadWrite);
            //FolderAccess folderAccessDest = await destination.OpenAsync(FolderAccess.ReadWrite);
            foreach(UniqueId uniqueId in uniqueIds)
            {
                try
                {
                    Nullable<UniqueId> tmp = await source.MoveToAsync(uniqueId, destination);
                    response.Add(tmp ?? throw new Exception());
                }
                catch (Exception ex)
                {

                }
            }
            source.Close();
            //destination.Close();
            if (!wasNotInit)
                imapClient.Disconnect(true);
            return response;
        }
        public async Task<List<UniqueId>> MarkMailsAs(IMailFolder mailFolder, List<UniqueId> uniqueIds, MessageFlags messageFlags, string user = null, string password = null, ImapClient imapClient = null)
        {
            bool wasNotInit = imapClient != null;
            imapClient = wasNotInit ? imapClient : await CreateImapConnection(user, password);
            FolderAccess folderAccessDest = mailFolder.Open(FolderAccess.ReadWrite);
            uniqueIds.ForEach(async (x) =>
            {
                try
                {
                    await mailFolder.AddFlagsAsync(x, messageFlags, true);
                    uniqueIds.Remove(x);//als response der funktion werden alle  zurückgegeben wo es nicht geklappt hat
                }
                catch (Exception ex)
                {

                }

            });
            mailFolder.Close();
            if (!wasNotInit)
                imapClient.Disconnect(true);
            return uniqueIds;
        }

        public async Task<ImapClient> CreateImapConnection(string user, string password)
        {
            try
            {

                ImapClient imapClient = null;
                try
                {
                    imapClient = new ImapClient(new ProtocolLogger(LoggerFileImap, true));
                }
                catch(Exception ex)
                {
                    DateTime dateTime = DateTime.Now;
                    FileStream fileStream = File.Create(dateTime.ToString("yyyy_MM_dd")+"_" + dateTime.Ticks + "_" + LoggerFileImap);
                    imapClient = new ImapClient(new ProtocolLogger(fileStream, true));
                }

                imapClient.Timeout = TimeoutImap;
                UserImap = user;
                PasswordImap = password;
                await imapClient.ConnectAsync(ServerImap, PortImap, SecureSocketOptionsImap);
                await imapClient.AuthenticateAsync(user, password);
                return imapClient;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<SmtpClient> CreateSmtpConnection(string user, string password)
        {
            try
            {
                SmtpClient smtpClient = null;
                try
                {
                    smtpClient = new SmtpClient(new ProtocolLogger(LoggerFileSmtp, true));
                }
                catch (Exception ex)
                {
                    DateTime dateTime = DateTime.Now;
                    FileStream fileStream = File.Create(dateTime.ToString("yyyy_MM_dd") + "_" + dateTime.Ticks + "_" + LoggerFileSmtp);
                    smtpClient = new SmtpClient(new ProtocolLogger(fileStream, true));
                }
                smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
                smtpClient.Timeout = TimeoutSmtp;

                UserSmtp = user;
                PasswordSmtp = password;
                await smtpClient.ConnectAsync(ServerSmtp, PortSmtp, SecureSocketOptionsSmtp);
                await smtpClient.AuthenticateAsync(user, password);
                return smtpClient;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


    }
    public static class MailHandlerExtension
    {
        public static void Reply(this MimeMessage message, SmtpClient smtpClient, MimeMessage reply)
        {
            using (smtpClient)
            {
                smtpClient.Send(reply);
                smtpClient.Disconnect(true);
            }
        }
    }
}
