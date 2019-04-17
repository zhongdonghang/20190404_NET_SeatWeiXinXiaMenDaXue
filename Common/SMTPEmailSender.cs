using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Common
{
    public class SMTPEmailSender
    {
        //If your smtp server wants authentication,use it 
        public SMTPEmailSender(String smtpServer, String user, String password)
        {
            mailMessage = new MailMessage();
            smtpClient = new SmtpClient();
            smtpClient.Host = smtpServer;
            smtpClient.Port = 25;
            smtpClient.Credentials = new NetworkCredential(user, password);
        }

        //If your smtp server doesn't want authentication,use it 
        public SMTPEmailSender(String smtpServer)
        {
            mailMessage = new MailMessage();
            smtpClient = new SmtpClient(smtpServer);
        }

        public bool IsBodyHtml
        {
            get {
                return mailMessage.IsBodyHtml;
            }
            set {
                mailMessage.IsBodyHtml = value;
            }
        }

        public String Subject
        {
            get
            {
                return mailMessage.Subject;
            }

            set
            {
                mailMessage.Subject = value;
            }
        }

        //get/set the email's content
        public String Content
        {
            get
            {
                return mailMessage.Body;
            }
            set
            {
                mailMessage.Body = value;
            }
        }

        public String From
        {
            get
            {
                return mailMessage.From.Address;
            }

            set
            {
                mailMessage.From = new MailAddress(value);
            }
        }

        public void AddReceiver(String email)
        {
            mailMessage.To.Add(email);
        }

        public void Send()
        {
            smtpClient.Send(mailMessage);
        }

        public void AddAttachment(String filename)
        {
            mailMessage.Attachments.Add(new Attachment(filename));
        }

        private MailMessage mailMessage;
        private SmtpClient smtpClient;
    }
}
