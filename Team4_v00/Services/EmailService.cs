using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

namespace Ben_Project.Services
{
    public static class EmailService
    {
        public static void SendEmail(MailAddress FromEmail, MailAddress ToEmail, string Subject, string MessageBody)
        {
            // sending email to dept rep
            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = "sa50team4@gmail.com",
                    Password = "sa50team4adproject"
                }
            };

            MailMessage Message = new MailMessage()
            {
                From = FromEmail,
                Subject = Subject,
                Body = MessageBody
            };

            Message.To.Add(ToEmail);

            try
            {
                client.Send(Message);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
        }
    }

}
