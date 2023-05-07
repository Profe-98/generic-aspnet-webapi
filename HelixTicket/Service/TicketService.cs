using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using System.Threading.Tasks;
using HelixTicket.InternalModels;
using HelixTicket.Handler;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Reflection;
using MimeKit;
using WebApiFunction.Threading.Task;
using WebApiFunction.Mail;
using WebApiFunction.Threading.Service;
using WebApiFunction.Database;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Configuration;
using WebApiFunction.Application;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Controller;
using WebApiFunction.Utility;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Log;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Application.Model.Database.MySql.Helix;

namespace HelixTicket
{
    public class TicketService : BackgroundService
    {
        private readonly ILogger<TicketService> _logger;
        private readonly ITaskSchedulerBackgroundServiceQueuer _taskScheduler;
        private readonly IMailHandler _mailHandler;
        private readonly ISingletonDatabaseHandler _databaseHandler;
        private readonly ISingletonTicketHandler _ticketHandler;
        private readonly ISingletonVulnerablityHandler _vulnerablityHandler;
        private readonly IAppconfig _appConfig;

        public TicketService(ILogger<TicketService> logger,IAppconfig appConfig,ITaskSchedulerBackgroundServiceQueuer taskScheduler, IMailHandler mailHandler, ISingletonDatabaseHandler singletonDatabaseHandler, ISingletonTicketHandler ticketHandler,ISingletonVulnerablityHandler vulnerablityHandler)
        {
            _logger = logger;
            _taskScheduler = taskScheduler;
            _mailHandler = mailHandler;
            _databaseHandler = singletonDatabaseHandler;
            _ticketHandler = ticketHandler;
            _vulnerablityHandler = vulnerablityHandler;
            _appConfig = appConfig;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken) 
        {
            var t = Task.Run(() =>
            {

                Init();
            }, stoppingToken);
            return t;
        }
        public override void Dispose()
        {
            base.Dispose();
        }
        private async void Init()
        {

            TimeSpan repeatitionTime = TimeSpan.Zero;
            repeatitionTime = new TimeSpan(0, 0, 30);

            TaskObject getMailsProcedure = new TaskObject(async () =>
            {

                if (AppManager.SystemUsedMediumAccess == null)
                {
                    QueryResponseData<SystemMessageUserModel> queryResponseData = await _databaseHandler.ExecuteQuery<SystemMessageUserModel>("SELECT * FROM system_message_user WHERE active = 1 AND deleted = 0;");
                    if (queryResponseData.HasData)
                    {
                        AppManager.SystemUsedMediumAccess = queryResponseData.DataStorage;
                        _logger.Logging(LogLevel.Debug, "ticket system fully init", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                    }
                }


                if (AppManager.SystemUsedMediumAccess != null)
                {
                    Guid openTicketStateUuid = new Guid("8c7cb72d-bfa7-11eb-a11f-309c2364fdb6");

                    Guid communicationMediumUuid = BackendAPIDefinitionsProperties.CommunicationMedium[BackendAPIDefinitionsProperties.CmKeyExtImapService];
                    var imapCredentials = AppManager.SystemUsedMediumAccess.FindAll(x => x.CommunicationMediumUuid == communicationMediumUuid);
                    _logger.Logging(LogLevel.Debug, "[mail]: " + " system users filterd for communication medium external imap", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                    foreach (SystemMessageUserModel model in imapCredentials)
                    {
                        string user = model.User;
                        string pw = model.Password;
                        MailKit.Net.Imap.ImapClient client = await _mailHandler.CreateImapConnection(user, pw);
                        MailKit.Net.Smtp.SmtpClient smtpClient = await _mailHandler.CreateSmtpConnection(user, pw);
                        if (client != null && smtpClient != null)
                        {
                            _logger.Logging(LogLevel.Debug, "[mail]: imap & smtp connection established for user: " + user + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                            //read out queue by system user ... INFO/WARN: one system user can only have one queue
                            MessageQueueModel queue = await _ticketHandler.GetQueue(model.Uuid);
                           
                            if (queue == null)
                            {
                                _logger.Logging(LogLevel.Error, "[database:error]: no queue found for user: " + user + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                continue;
                            }
                            if(queue.IsJunk)
                            {
                                AppManager.SystemUsedMediumAccess.Remove(model);
                                continue;
                            }
                            if (queue.HasHirarchicalError)
                            {
                                _logger.Logging(LogLevel.Error, "[database:error]: queue for user: " + user + " has hirarchical errors", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                continue;
                            }
                            _logger.Logging(LogLevel.Debug, "[database]: queue for user: " + user + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);


                            MailKit.IMailFolder sourceFolder = null;
                            MailKit.IMailFolder destFolder = null;

                            if (!queue.DeleteMailsAfterProcessing)
                            {
                                if (queue.HasIndividualFolders)
                                {
                                    sourceFolder = await _mailHandler.GetMailFolder(queue.InboxFolder, imapClient: client);
                                    destFolder = await sourceFolder.GetSubfolderAsync(queue.ProceddedFolder);

                                    _logger.Logging(LogLevel.Debug, "[database]: queue has following folders:\nSource: " + queue.InboxFolder + "\nDest: " + queue.ProceddedFolder + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                }
                                else
                                {
                                    _logger.Logging(LogLevel.Warning, "[database:warning]: queue has not source & dest folder or something went wrong with the given, processed mails markes as readed, default source is: INBOX", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);
                                }
                            }
                            else
                            {
                                _logger.Logging(LogLevel.Debug, "[database]: queue is set for delete mails after parsing to database", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);
                            }

                            if (queue.DeleteMailsAfterProcessing || sourceFolder != null)
                            {
                                _logger.Logging(LogLevel.Debug, "[mail]: try to load new emails from source with state: not seen", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                var mails = await _mailHandler.DownloadMails(MailKit.Search.SearchQuery.NotSeen, sourceFolder.FullName, imapClient: client);
                                _logger.Logging(LogLevel.Debug, "[mail]: loaded " + mails.Keys.Count + " mails with state: not seen", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                List<MailKit.UniqueId> proceddedMails = new List<MailKit.UniqueId>();
                                foreach (var key in mails.Keys)
                                {
                                    bool newCreated = false;
                                    var mail = mails[key];

                                    int sumAttachmentSize = 0;
                                    Dictionary<string,List<string>> notAllowedAttachments = new Dictionary<string, List<string>>();
                                    var parts = mail.BodyParts.OfType<MimeKit.TextPart>().ToList();
                                    //parts.ForEach(x => System.Diagnostics.Debug.WriteLine(x.FileName));
                                    byte[] mailBodyBinary = _mailHandler.GetAttachmentBinary(parts != null?parts.Find(x => x.FileName == null):null);//body content/mail text content extract
                                    nClam.ClamScanResult scanResult = await _vulnerablityHandler.Scan(mailBodyBinary);
                                    if(scanResult.Result == nClam.ClamScanResults.Clean)
                                    {
                                        _logger.Logging(LogLevel.Debug, "\t[mail:uid]: check by av, result: clean", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                        if (mail.Attachments != null)
                                        {

                                            mail.Attachments.ToList().ForEach(async (x) => {

                                                string f = x.ContentDisposition?.FileName ?? x.ContentType.Name ?? "attachment-"+DateTime.Now.Ticks+"";

                                                if (!notAllowedAttachments.ContainsKey(f))
                                                {
                                                    notAllowedAttachments.Add(f, new List<string>());
                                                }
                                                if (!String.IsNullOrEmpty(f))
                                                {
                                                    byte[] attachmentBinary = _mailHandler.GetAttachmentBinary(x);
                                                    if (queue.AttachmentAvCheck)
                                                    {
                                                        //test with eicar test string, because many mail provider also block av files, test with sent mail from web.de mail to helixdb mail fails bcs this
                                                        //string testpath = @"F:\Users\0x00405a00\Desktop\test.txt";
                                                        /*
                                                         * Test with binary array
                                                         * attachmentBinary = System.IO.File.ReadAllBytes(testpath);
                                                        nClam.ClamScanResult scanResult = await _vulnerablityHandler.Scan(attachmentBinary);*/

                                                        /*
                                                         * test with file
                                                        var scanResult = await _vulnerablityHandler.CheckFile(testpath);
                                                        */
                                                        scanResult = await _vulnerablityHandler.Scan(attachmentBinary);

                                                        if (scanResult.Result != nClam.ClamScanResults.Clean)
                                                        {
                                                            notAllowedAttachments[f].Add("file contains virus content");
                                                        }

                                                    }
                                                    if (queue.HasFileExtensionRestriction)
                                                    {

                                                        bool allowed = queue.MaxAllowedFileExtensions.Find(y => f.ToLower().EndsWith(y.Extension.ToLower())) != null;
                                                        if (!allowed)
                                                        {
                                                            notAllowedAttachments[f].Add("file extension not allowed");
                                                        }
                                                        else
                                                        {

                                                        }
                                                    }
                                                    if (queue.HasMaxFileSizeRestriction)
                                                    {
                                                        long size = attachmentBinary.Length / 1000;
                                                        sumAttachmentSize += (int)size;
                                                        if (queue.MaxAttachmentFileSize <= sumAttachmentSize)
                                                        {

                                                            notAllowedAttachments[f].Add("max file size reached (sum of all attachments reaching limit of " + queue.MaxAttachmentFileSize + "kB)");
                                                        }
                                                    }

                                                }
                                                else
                                                {

                                                    notAllowedAttachments[f].Add("file name is null or empty");
                                                }
                                            });

                                        }
                                        string errorMsg = null;
                                        foreach (string nkey in notAllowedAttachments.Keys)
                                        {
                                            int c = notAllowedAttachments[nkey].Count;
                                            if (c != 0)
                                            {
                                                errorMsg += " - " + nkey + "";
                                                for (int i = 0; i < c; i++)
                                                {
                                                    errorMsg += "\t" + notAllowedAttachments[nkey][i];
                                                }
                                            }
                                        }
                                        if (errorMsg != null)
                                        {
                                            _logger.Logging(LogLevel.Error, "\t[mail:uid:" + key + ":error]: attachment(s) reaching restrictions", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);
                                            _logger.Logging(LogLevel.Error, "\t[mail:uid:" + key + ":error]: \n" + errorMsg + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                            proceddedMails.Add(key);
                                        }
                                        else
                                        {
                                            var attachmentsOnFileSystem = await _mailHandler.GetMimeMessageAttachements(_appConfig.AppServiceConfiguration.MailConfigurationModel.EmailAttachmentPath, mail);

                                            _logger.Logging(LogLevel.Debug, "\t[mail:uid:" + key + "]: get and selected for parsing", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                            TicketIdentifierCheckResponse ticketIdentifierCheckResponse = _ticketHandler.StringContainsTicketIdentifier(mail.Subject);
                                            MessageConversationModel messageConversationModel = null;
                                            if (ticketIdentifierCheckResponse.IsValidTicketIdentifier)//valide jetzt prüfen ob zu dem identifier eine konversation besteht
                                            {
                                                _logger.Logging(LogLevel.Debug, "\t[mail:uid:" + key + "]: has a ticket-conversation identifier (" + ticketIdentifierCheckResponse.TicketIdentifierFully + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                messageConversationModel = await _ticketHandler.GetConversation(ticketIdentifierCheckResponse);
                                                if (messageConversationModel == null)// existiert nicht, abweisen mit hinweis(mail) das es kein ticket unter dieser ticket.nr gibt und das der prefix aus dem betreff verschwinden muss um ein neues ticket zu eröffnen, sender muss mail erneut senden
                                                {
                                                    _logger.Logging(LogLevel.Error, "\t[mail:uid:" + key + ":error]: ticket-conversation identifier is correct syntax but no matched ticket find in database", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                    continue;
                                                }
                                                else
                                                {
                                                    _logger.Logging(LogLevel.Debug, "\t[mail:uid:" + key + "]: ticket-conversation found with identifier (" + messageConversationModel.DisplayConfIdentifier + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                }
                                            }

                                            if (messageConversationModel == null)//ticket konnte vorab nicht gefunden werden d.h. neues ticket erzeugen
                                            {

                                                messageConversationModel = await _ticketHandler.CreateNewTicket(openTicketStateUuid, communicationMediumUuid, queue.Uuid);
                                                newCreated = true;
                                                _logger.Logging(LogLevel.Debug, "\t[mail:uid:" + key + "]: new ticket opened (" + messageConversationModel.DisplayConfIdentifier + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                            }
                                            else
                                            {
                                                _logger.Logging(LogLevel.Debug, "\t[mail:uid:" + key + "]: try to check ticket-conversation sub-queue", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                var childQueue = await _ticketHandler.GetQueue(messageConversationModel.MessageQueueUuid, false);
                                                if (childQueue.Uuid != queue.Uuid)//wenn rootQueue nicht gleich zugeteilte queue von conversation ist, dann ist es ein child, hier hierarchie prüfen
                                                {
                                                    _logger.Logging(LogLevel.Debug, "\t[mail:uid:" + key + "]: sub-queue identified", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                    if (queue.HasHirarchicalError)
                                                    {
                                                        _logger.Logging(LogLevel.Error, "\t[mail:uid:" + key + ":error]: sub-queue has hirarchical errors", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    _logger.Logging(LogLevel.Debug, "\t[mail:uid:" + key + "]: sub-queue not found, current queue is root", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                }
                                            }
                                            if (messageConversationModel != null)//ticket/konversation wurde gefunden
                                            {
                                                List<AccountModel> conversationParticipants = await _ticketHandler.GetAllConversationParticipants(messageConversationModel);
                                                List<MessageAttachmentModel> attachmentModels = attachmentsOnFileSystem.Keys.Count != 0 ?
                                                new List<MessageAttachmentModel>() : null;

                                                MessageModel messageModel = await _ticketHandler.AddMessageToTicket(mail.Subject, System.Text.Encoding.Default.GetBytes(mail.GetTextBody(MimeKit.Text.TextFormat.Plain)), false, false, false, "{\"test\":\"1\"}", messageConversationModel, attachmentModels);
                                                if (messageModel != null)//fehler message konnte nicht erzeugt werden
                                                {
                                                    _logger.Logging(LogLevel.Debug, "\t[database:uid:" + key + "]: new message added to ticket (" + messageModel.Uuid + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                    Func<MimeKit.InternetAddress, System.Threading.Tasks.Task<AccountModel>> fAcc = (x) => WebApiFunction.Utility.Utils.CallAsyncFunc<MimeKit.InternetAddress, AccountModel>(x, async (x) =>
                                                    {

                                                        string userName = String.IsNullOrEmpty(x.Name) ? x.ToString() : x.Name;
                                                        _logger.Logging(LogLevel.Debug, "\t\t[database:uid:" + key + "]: try to find account: " + userName + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);
                                                        var accountModel = await _ticketHandler.GetAccount(userName, communicationMediumUuid);
                                                        if (accountModel == null && !String.IsNullOrEmpty(userName))
                                                        {
                                                            _logger.Logging(LogLevel.Debug, "\t\t\t[database:uid:" + key + "]: account could not be found, try to add", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                            accountModel = await _ticketHandler.AddAccount(userName, communicationMediumUuid);
                                                            if (accountModel == null)
                                                            {
                                                                _logger.Logging(LogLevel.Error, "\t\t\t[database:uid:" + key + ":error]: account could not be added", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                            }
                                                        }
                                                        return accountModel;
                                                    });

                                                    bool goesOn = false;
                                                    foreach (var mailInetAddr in mail.From)
                                                    {
                                                        AccountModel acc = await fAcc(mailInetAddr);
                                                        bool hasAccess = !newCreated ?
                                                        _ticketHandler.IsAccountPartOfList(acc, conversationParticipants) : newCreated;

                                                        if (!hasAccess)//abweisen, keine berechtigung
                                                        {
                                                            _logger.Logging(LogLevel.Error, "\t[database:uid:" + key + ":restriction]: account is no part of conversation (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                        }
                                                        else if (acc != null)
                                                        {
                                                            _logger.Logging(LogLevel.Debug, "\t[database:uid:" + key + "]: account is part of conversation (" + acc.User + ", Ticket new created? " + (newCreated ? "Yes" : "No") + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                            foreach (string path in attachmentsOnFileSystem.Keys)
                                                            {
                                                                _logger.Logging(LogLevel.Debug, "\t\t[database:uid:" + key + "]: try to parse attachment (" + path + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                string tmpExt = System.IO.Path.GetExtension(path);
                                                                string ext = String.IsNullOrEmpty(tmpExt) ? "." : tmpExt;
                                                                string hash = await _ticketHandler.CalculateMessageAttachmentHash(path);
                                                                _logger.Logging(LogLevel.Debug, "\t\t\t[database:uid:" + key + "]: Extension: " + ext + ", Hash: " + hash + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                MessageAttachmentModel messageAttachmentModel = new MessageAttachmentModel
                                                                {
                                                                    Hash = hash,
                                                                    FileExtension = ext
                                                                };

                                                                var msgAttRef = await _ticketHandler.GetMessageAttachment(messageAttachmentModel);

                                                                messageAttachmentModel = new MessageAttachmentModel
                                                                {
                                                                    AccountUuid = acc.Uuid,//Aaccount_uuid
                                                                    CheckForSexualContentMeta = "{\"test\":\"1\"}",
                                                                    CheckForSexualContent = false,
                                                                    CheckForViolence = false,
                                                                    CheckForVirus = false,
                                                                    Hash = messageAttachmentModel.Hash,
                                                                    Path = path,
                                                                    FileExtension = ext
                                                                };

                                                                if (msgAttRef == null)
                                                                {
                                                                    _logger.Logging(LogLevel.Debug, "\t\t\t[database:uid:" + key + "]: no attachment found for Hash: '" + hash + "', try to add new", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                    messageAttachmentModel.FirstSourceMessageUuid = messageModel.Uuid;//attachment existierte vorher nicht, somit ist die oben verarbeitete mail die erste source msg von diesem attachment
                                                                    msgAttRef = await _ticketHandler.CreateMessageAttachment(messageAttachmentModel);
                                                                    if (msgAttRef != null)
                                                                    {
                                                                        _logger.Logging(LogLevel.Error, "\t\t\t[database:uid:" + key + "]: attachment could created", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                        messageAttachmentModel.Uuid = msgAttRef.Uuid;
                                                                        System.IO.File.Move(path, msgAttRef.Path, true);
                                                                        _logger.Logging(LogLevel.Debug, "\t\t\t[database:uid:" + key + "]: try to rename file '" + path + "' to final filename '" + msgAttRef.Path + "'", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                        if (msgAttRef.FileExists)//successfully renamed file from old 'path' to new filename of value from 'msgAttRef.Path'
                                                                        {
                                                                            _logger.Logging(LogLevel.Debug, "\t\t\t[database:uid:" + key + "]: file successfully renamed" + msgAttRef.Path + "'", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                            attachmentModels.Add(msgAttRef);
                                                                        }
                                                                        else
                                                                        {

                                                                            _logger.Logging(LogLevel.Error, "\t\t\t[database:uid:" + key + ":error]: file could not be renamed '" + path + "'", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                        }

                                                                    }
                                                                    else
                                                                    {
                                                                        _logger.Logging(LogLevel.Error, "\t\t\t[database:uid:" + key + ":error]: attachment could not created", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    _logger.Logging(LogLevel.Debug, "\t\t\t[database:uid:" + key + "]: attachment exists already, prepare for attachment relation to message", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                    messageAttachmentModel.Uuid = msgAttRef.Uuid;
                                                                    attachmentModels.Add(msgAttRef);
                                                                }
                                                                if (msgAttRef != null)
                                                                {
                                                                    _logger.Logging(LogLevel.Debug, "\t\t[database:uid:" + key + "]: start to create relation between message and attachment", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                    var rel = await _ticketHandler.CreateRelationBetweenMessageAndAttachment(messageModel, messageAttachmentModel);
                                                                    if (rel == null)
                                                                    {
                                                                        _logger.Logging(LogLevel.Error, "\t\t[database:uid:" + key + ":error]: relation could not be created", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                    }

                                                                }
                                                                System.IO.File.Delete(path);
                                                            }
                                                            goesOn = true;//ist bereits mitglieder der konversation
                                                            _logger.Logging(LogLevel.Debug, "\t[database:uid:" + key + "]: start to create relation between message and account from mail sender", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                            MessageRelationToAccountModel relation = await _ticketHandler.CreateRelationBetweenMessageAndAccount(messageModel, acc, TicketHandler.MESSAGE_SENDING_TYPE.From);
                                                            if (relation == null)
                                                                _logger.Logging(LogLevel.Error, "\t[database:uid:" + key + ":error]: relation could not be created", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                        }
                                                        else
                                                        {
                                                            _logger.Logging(LogLevel.Error, "\t[database:uid:" + key + ":error]: account of sender is null after creation try", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                        }
                                                    }
                                                    if (!goesOn)//ist false wenn der absender (mail.From) kein mitglieder der konversation ist, somit kann er auch keine weiteren mitglieder der konversation benennen (To,Bcc,Cc)
                                                    {
                                                        _logger.Logging(LogLevel.Error, "\t[database:uid:" + key + ":restriction]: mail sender is not restricted to have access due this conversation", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                        proceddedMails.Add(key);

                                                    }
                                                    else
                                                    {


                                                        foreach (var mailInetAddr in mail.To)
                                                        {
                                                            AccountModel acc = await fAcc(mailInetAddr);
                                                            bool isParticipant = _ticketHandler.IsAccountPartOfList(acc, conversationParticipants);
                                                            if (!isParticipant && acc != null)
                                                            {
                                                                _logger.Logging(LogLevel.Debug, "\t[database:uid:" + key + "]: mail sender is currently no participant of conv., try to add (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                MessageRelationToAccountModel relation = await _ticketHandler.CreateRelationBetweenMessageAndAccount(messageModel, acc, TicketHandler.MESSAGE_SENDING_TYPE.To);
                                                                if (relation == null)
                                                                {
                                                                    _logger.Logging(LogLevel.Error, "\t\t[database:uid:" + key + ":error]: could not add to participant (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                }
                                                                else
                                                                {
                                                                    _logger.Logging(LogLevel.Debug, "\t\t[database:uid:" + key + "]: added to conv. (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                }
                                                            }
                                                        }
                                                        foreach (var mailInetAddr in mail.Cc)
                                                        {
                                                            AccountModel acc = await fAcc(mailInetAddr);
                                                            bool isParticipant = _ticketHandler.IsAccountPartOfList(acc, conversationParticipants);
                                                            if (!isParticipant && acc != null)
                                                            {
                                                                _logger.Logging(LogLevel.Debug, "\t[database:uid:" + key + "]: mail sender is currently no participant of conv., try to add (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                MessageRelationToAccountModel relation = await _ticketHandler.CreateRelationBetweenMessageAndAccount(messageModel, acc, TicketHandler.MESSAGE_SENDING_TYPE.To);
                                                                if (relation == null)
                                                                {
                                                                    _logger.Logging(LogLevel.Error, "\t\t[database:uid:" + key + ":error]: could not add to participant (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                }
                                                                else
                                                                {
                                                                    _logger.Logging(LogLevel.Debug, "\t\t[database:uid:" + key + "]: added to conv. (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                }
                                                            }
                                                        }
                                                        foreach (var mailInetAddr in mail.Bcc)
                                                        {
                                                            AccountModel acc = await fAcc(mailInetAddr);
                                                            bool isParticipant = _ticketHandler.IsAccountPartOfList(acc, conversationParticipants);
                                                            if (!isParticipant && acc != null)
                                                            {
                                                                _logger.Logging(LogLevel.Debug, "\t[database:uid:" + key + "]: mail sender is currently no participant of conv., try to add (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                MessageRelationToAccountModel relation = await _ticketHandler.CreateRelationBetweenMessageAndAccount(messageModel, acc, TicketHandler.MESSAGE_SENDING_TYPE.To);
                                                                if (relation == null)
                                                                {
                                                                    _logger.Logging(LogLevel.Error, "\t\t[database:uid:" + key + ":error]: could not add to participant (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                }
                                                                else
                                                                {
                                                                    _logger.Logging(LogLevel.Debug, "\t\t[database:uid:" + key + "]: added to conv. (" + acc.User + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                }
                                                            }
                                                        }

                                                        if (newCreated && messageConversationModel != null)
                                                        {
                                                            var replyMail = _mailHandler.PrepareReply(mail, queue.InitialMessage, false, "" + messageConversationModel.DisplayConfIdentifier + " - Ticket opened");
                                                            var mailAddrSender = MimeKit.MailboxAddress.Parse(user);
                                                            replyMail.Sender = mailAddrSender;
                                                            replyMail.From.Add(mailAddrSender);
                                                            replyMail.To.Remove(mailAddrSender);

                                                            _logger.Logging(LogLevel.Debug, "\t[mail:uid:" + key + "]: start to send initial ticket-newly-create message", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                            if (replyMail.To.Count != 0)
                                                            {

                                                                try
                                                                {
                                                                    smtpClient.Send(replyMail);
                                                                    _logger.Logging(LogLevel.Debug, "\t\t[mail:uid:" + key + "]: initial message sent", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                }
                                                                catch (Exception ex)
                                                                {
                                                                    _logger.Logging(LogLevel.Error, "\t\t[mail:uid:" + key + "]: initial message couldnt sent (" + ex.Message + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                                }
                                                            }
                                                            proceddedMails.Add(key);
                                                        }
                                                        else
                                                        {

                                                            proceddedMails.Add(key);
                                                        }
                                                    }

                                                }
                                                else
                                                {
                                                    _logger.Logging(LogLevel.Error, "\t[database:uid:" + key + ":error]: message could not be added to conversation", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                                }

                                            }
                                            else//interner fehler (ticket konnte nicht ausgelesen werden und nicht erstellt werden, wenn es vorab nicht existiert hat)
                                            {
                                                _logger.Logging(LogLevel.Error, "\t[database:uid:" + key + ":error]: ticket-conv. could not be created", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);


                                            }
                                        }
                                        
                                    }
                                    else//wenn body als virus klassifiziert wird
                                    {
                                        proceddedMails.Add(key);
                                        _logger.Logging(LogLevel.Information, "\t[mail:uid]: contains infected body part, reg. procedure skipped", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                    }
                                }



                                if (!queue.DeleteMailsAfterProcessing)
                                {
                                    if (destFolder != null)
                                    {

                                        var movedMails = await _mailHandler.MoveMails(sourceFolder, proceddedMails, destFolder, imapClient: client);
                                        if (movedMails != null)
                                        {
                                            _logger.Logging(LogLevel.Debug, "\t[mail]: all processed mails moved to target destination (dest) (" + movedMails.Count + " pieces of " + proceddedMails.Count + " processed mails count)", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);
                                        }
                                    }
                                    else
                                    {

                                        var markedAsReaded = await _mailHandler.MarkMailsAs(sourceFolder, proceddedMails, MailKit.MessageFlags.Seen);
                                        if (markedAsReaded != null)
                                        {
                                            _logger.Logging(LogLevel.Debug, "\t[mail]: all processed mails marked as read (" + markedAsReaded.Count + " pieces of " + proceddedMails.Count + " processed mails count)", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                        }
                                    }
                                }
                                else
                                {

                                    var deletedMails = await _mailHandler.DeleteMails(sourceFolder, proceddedMails);
                                    if (deletedMails != null)
                                    {
                                        _logger.Logging(LogLevel.Debug, "\t[mail]: all processed mails would deleted after correct parsing (" + deletedMails.Count + " pieces of " + proceddedMails.Count + " processed mails count)", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                    }
                                }
                                _logger.Logging(LogLevel.Debug, "\t[mail]: action completed, disconect from imap & smtp", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                                smtpClient.Disconnect(true);
                                smtpClient.Dispose();
                                client.Disconnect(true);
                                client.Dispose();
                            }
                        }
                        else
                        {
                            _logger.Logging(LogLevel.Error, "\t[mail]: system user removed due error (" + model.User + ", Medium: " + model.CommunicationMediumUuid + ")", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);
                            _logger.Logging(LogLevel.Error, "\t[mail]: connection state to smtp & imap: SMTP: " + (smtpClient == null ? "No" : smtpClient.IsConnected ? "Yes" : "No") + "\nIMAP: " + (client == null ? "No" : client.IsConnected ? "Yes" : "No") + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);
                            _logger.Logging(LogLevel.Error, "\t\t[mail]: Note that the username + password for smtp&imap should be the same, only the server+port is not equal", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemGeneral);

                            AppManager.SystemUsedMediumAccess.Remove(model);
                        }
                    }
                }

            }, repeatitionTime);
            TaskObject deleteMailsFromJunkProcedure = new TaskObject(async () => 
            {
                _logger.Logging(LogLevel.Debug, "[junk]: observe conversations in junks", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                var junks = await _ticketHandler.GetQueues();
                junks = junks != null ? junks.FindAll(x => x.IsJunk) : null;
                if(junks != null)
                {

                    if (junks.Count != 0)
                    {

                        _logger.Logging(LogLevel.Debug, "[junk]: " + junks.Count + " junks loaded", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                        List<Guid> junkUuids = junks.GroupBy(x => x.Uuid).Select(x => x.Key).ToList();
                        string inClauseFields = _databaseHandler.ConvertListToWhereInFieldArray<Guid>(junkUuids);
                        bool success = await _databaseHandler.SetEnvironmentVar<string>(MySqlDatabaseHandler.MYSQL_ENV_VARS.SAFE_UPDATES, "1", "0", true);
                        QueryResponseData response = await _databaseHandler.ExecuteQuery("UPDATE message_conversation SET deleted = 1 WHERE message_queue_uuid " + inClauseFields + ";");
                        if (response.HasSuccess)
                        {
                            _logger.Logging(LogLevel.Debug, "[junk]: " + response.Message + " message_conversations marked as deleted", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                        }
                        else
                        {
                            _logger.Logging(LogLevel.Error, "[junk:error]: " + response.Message + " messages could not marked as deleted", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                        }
                    }
                }

            }, repeatitionTime);

            TaskObject sendMails = new TaskObject(async () =>
            {
                Guid communicationMediumUuid = BackendAPIDefinitionsProperties.CommunicationMedium[BackendAPIDefinitionsProperties.CmKeyExtImapService];
                MimeMessage msg = null;
                do
                {
                    foreach(string sender in _mailHandler.Queue.Keys)
                    {
                        int count = _mailHandler.Queue[sender].Count;
                        if (count != 0)
                        {
                            _logger.Logging(LogLevel.Debug, "[send-mails]: "+ count + " for mail: "+sender+"", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);
                            msg = _mailHandler.Dequeue(sender);
                            try
                            {
                                _logger.Logging(LogLevel.Debug, "[send-mails:select:" + msg.MessageId + "]: sender: " + sender + ", to: " + msg.To.ToString() + ", subject: " + msg.Subject + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                                msg.Sender = MailboxAddress.Parse(sender);
                                var imapCredentials = AppManager.SystemUsedMediumAccess.Find(x => x.CommunicationMediumUuid == communicationMediumUuid && x.User == sender);
                                if (imapCredentials != null)
                                {
                                    SystemMessageUserModel model = imapCredentials;
                                    string user = model.User;
                                    string pw = model.Password;

                                    MailKit.Net.Smtp.SmtpClient smtpClient = await _mailHandler.CreateSmtpConnection(user, pw);

                                    try
                                    {
                                        _logger.Logging(LogLevel.Debug, "[send-mails:select:" + msg.MessageId + ":send]: try", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                                        smtpClient.Send(msg);
                                        _logger.Logging(LogLevel.Debug, "[send-mails:select:" + msg.MessageId + ":send]: sent", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Logging(LogLevel.Debug, "[send-mails:select:" + msg.MessageId + ":send]: " + ex.Message + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                                    }
                                    finally
                                    {

                                    }
                                }
                                else
                                {
                                    _logger.Logging(LogLevel.Debug, "[send-mails]: no smtp account found for " + sender + "", MethodBase.GetCurrentMethod(), this.GetType().Name, CustomLogEvents.TicketSystemInit);

                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            finally
                            {

                            }
                        }
                        
                    }
                    
                }
                while (msg != null || _mailHandler.Queue.Count != 0);
                /*asafasf
                AuthentificationController->[HttpPost("register")]
        public async Task<ActionResult> RegisterFrontEndUser([FromBody] RegisterModel registerModel)*/
                
                //Queue wo man Mimemessages anhängen kann, die hier abgearbeitet werden (versendet werden)
            }, repeatitionTime);


            //reminder: am besten für jedes imap postfach eine task enqueuen, ist performanter, anstatt eine task mit foreach für jedes postfach
            _taskScheduler.Enqueue(getMailsProcedure);
            _taskScheduler.Enqueue(deleteMailsFromJunkProcedure);
            _taskScheduler.Enqueue(sendMails);

        }

    }
    public static class TicketServiceExtension
    {
        public static IServiceCollection UseMailTicketSystem(this IServiceCollection builder)
        {
            return builder.AddHostedService<TicketService>();
        }
    }
}
