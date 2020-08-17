using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using Ben_Project.Models;
using System.Diagnostics;

namespace Ben_Project.Services.MessageService
{
    public class MessageService
    {

        public String sendMail(Mail fromMail, Mail toMail, String acknowledgementCode, String flag) {

            if (fromMail.Equals(null) || toMail.Equals(null) || flag.Equals(null) ){
                return "error";
            }
            SmtpClient client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential()
                {
                    UserName = fromMail.mailAddress,
                    Password = fromMail.password
                }
            };
            MailAddress FromEmail = new MailAddress(fromMail.mailAddress, fromMail.name);
            MailAddress ToEmail = new MailAddress(toMail.mailAddress, toMail.name);

            MailMessage Message = new MailMessage();

            String MessageBody ="";

            if (flag == "sendAck") {
                if (acknowledgementCode.Equals(null)) {
                    return "";
                }
                 MessageBody = "The disbursement is ready for collection. The acknowledge code is " + acknowledgementCode;
                Message.Subject = "Disbursement Details";
            } else if (flag == "sendPO") {
                 MessageBody = "You get order from Store!";

                Message.Subject = "Order Details";
            }
            Message.To.Add(ToEmail);
            Message.From = FromEmail;
            Message.To.Add(ToEmail);
            Message.Body = MessageBody;
            try
            {
                client.Send(Message);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
            return "";
        }
    }
}
