using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Text;
using System.Net;

namespace WebApiApplicationService.Handler
{
    [Obsolete("Wird ersetzt durch Handler/MailHandler.cs, beinhaltet IMAP+SMTP Funktion")]
    public class SmtpHandler
    {
        private readonly SmtpClient _smtpClient;

        public SmtpHandler(string server,int port, ICredentialsByHost credentialsByHost, bool ssl, int timeoutSec = 10)
        {
            _smtpClient = new SmtpClient(server,port);
            _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            _smtpClient.EnableSsl = ssl;
            _smtpClient.Timeout = timeoutSec;
            _smtpClient.Credentials = credentialsByHost;
            _smtpClient.SendCompleted += MailSendCompleted;
        }

        private void MailSendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }

        public MailMessage PrepareMail(string from, MailAddressCollection to, string subject, MailPriority mailPriority, Encoding subjectEncoding, string body, Encoding bodyEncoding, AttachmentCollection attachments = null, MailAddressCollection cc = null, MailAddressCollection bcc = null, string sender = null)
        {

            using (MailMessage message = new MailMessage())
            {
                to.ToList().ForEach(x => message.To.Add(x));
                cc.ToList().ForEach(x => message.CC.Add(x));
                bcc.ToList().ForEach(x => message.Bcc.Add(x));
                message.From = new MailAddress(from);
                attachments.ToList().ForEach(x => message.Attachments.Add(x));
                message.BodyEncoding = bodyEncoding;
                message.SubjectEncoding = subjectEncoding;
                message.Subject = subject;
                message.Priority = mailPriority;
                message.Sender = new MailAddress(sender ?? from);
                
                return message;
            }
            return null;
        }
        public List<MailAction> SendMails(List<MailMessage> mailMessage)
        {
            List<MailAction> mailActionResponses = new List<MailAction>();
            mailMessage.ForEach(async(x) => mailActionResponses.Add(await Utils.CallAsyncFunc<MailMessage, MailAction>(x, async (x) => await SendMail(x))));
            return mailActionResponses;
        }

        public async Task<MailAction> SendMail(MailMessage mailMessage)
        {
            var sendTime = DateTime.MinValue;
            var sentTime = DateTime.MinValue;
            Exception exc = null;
            try
            {
                sendTime = DateTime.Now;
                await _smtpClient.SendMailAsync(mailMessage);
                sentTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                exc = ex;
            }
            var t = new MailAction(mailMessage, sendTime, sentTime, exc);

            return  t;
        }


        public class MailAction
        {
            public Exception Exception { get; set; }
            public MailMessage Message { get; private set; }
            public DateTime SendTime { get; set; } = DateTime.MinValue;//the time where the smtp client is try to send  the email
            public DateTime SentTime { get; set; } = DateTime.MinValue;//time where the mail is fully delivered
            public bool IsSent
            {
                get
                {
                    return SentTime != DateTime.MinValue;
                }
            }
            public bool HasMailAttachments
            {
                get
                {
                    return Message == null?false:Message.Attachments.Count != 0;
                }
            }
            public double TimeGoneForSent
            {
                get
                {
                    return (SentTime-SendTime).TotalMilliseconds; 
                }
            }
            public MailAction(MailMessage mailMessage, DateTime sendTime,DateTime sentTime,Exception exception = null)
            {
                Message = mailMessage;
                SendTime = sendTime;
                SentTime = sentTime;
                Exception = exception;
            }
        }
    }
}
