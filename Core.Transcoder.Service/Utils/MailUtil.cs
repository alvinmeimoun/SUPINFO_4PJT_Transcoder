using Core.Transcoder.DataAccess;
using Core.Transcoder.DataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Core.Transcoder.Service.Utils
{
    public static class MailUtil
    {

        public static void SendMail()
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;

                MailMessage mm = new MailMessage("Transcoder - Conversion en ligne", "malfatti.antonin@gmail.com", "subject", "body");
                mm.IsBodyHtml = true;
                client.Send(mm);
               
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not end email\n\n" + e.ToString());
            }

        }

        public static void SendMail(string typeDemande, TASK task = null, PanierViewModel panier = null)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.EnableSsl = true;

                MailMessage mm = task != null ? GenerateMailMessage(typeDemande, task) : GenerateMailMessage(typeDemande, null , panier);
                client.Send(mm);

                //TRACE Trace = new TRACE { FK_ID_TASK = task.PK_ID_TASK, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, FK_ID_SERVER = 1, DESCRIPTION = mm.ToString(), METHOD = "MAIL APRES CONVERSION", TYPE = "INFO" };
                //new TRACE_Service().AddTrace(Trace);
            }
            catch (Exception e)
            {
                TRACE Trace = new TRACE { FK_ID_TASK = task != null ? task.PK_ID_TASK : panier.TransactionId, DATE_TRACE = DateTime.Now, NOM_SERVER = System.Environment.MachineName, FK_ID_SERVER = 1, DESCRIPTION = e.Message, METHOD = "MAIL APRES DEMANDE/CONVERSION", TYPE = "ERROR" };
                new TRACE_Service().AddTrace(Trace);
            }

        }

        public static MailMessage GenerateMailMessage(string typeDemande, TASK task = null, PanierViewModel panier = null)
        {
            var message = new MailMessage();
            var user = new USER();
            if (task != null)
            {
                message = new MailMessage("transcodernoreply@gmail.com", task.USER.EMAIL);
            }
            else
            {
                user = new USER_Service().FindUserByID(panier.UserId);
                message = new MailMessage("transcodernoreply@gmail.com", user.EMAIL);
            }

            message.IsBodyHtml = true;
            message.BodyEncoding = Encoding.UTF8;

            switch (typeDemande)
            {
                case Enums.StringManager.CONVERSION_TERMINEE:
                    {
                        message.Subject = "Votre conversion n°" + task.PK_ID_TASK;
                        message.Body = "Bonjour " + task.USER.FIRSTNAME + " " + task.USER.LASTNAME + ", <br/>  <br/>";
                        message.Body += "Votre conversion n° " + task.PK_ID_TASK + " a été effectuée. <br/>";
                        message.Body += "Vous pouvez télécharger votre média en vous rendant sur Transcoder dans la section 'Mes Conversions'<br/>";
                        message.Body += "Nous vous souhaitons une agréable journée. <br/>";
                        message.Body += "Cordialement, <br/>";
                        message.Body += "<p> L'équipe TRANSCODER France </p> <br/>";
                        break;
                    }
                case Enums.StringManager.PAIEMENT_ACCEPTE:
                    {
                        message.Subject = "Votre commande n° " + panier.TransactionId;
                        message.Body = "<p>Bonjour " + user.FIRSTNAME + " " + user.LASTNAME + ",</p> <br/>";
                        message.Body += "Votre paiement pour la commande n° " + panier.TransactionId + " a été accepté et est en cours de traitement par nos services. <br/><br/>";
                        message.Body += "Vous recevrez un mail lorsque votre commande sera terminée et prête a être téléchargée. <br/><br/>";
                        message.Body += "<h3> Récapitulatif de votre commande </h3><hr/>";
                        message.Body += "<table style='border: solid 1px gray;'>";
                        message.Body += "<thead>";
                        message.Body += "<tr><td><strong>Nom du fichier</strong></td><td><strong>Format de base</strong></td><td><strong>Format de conversion</strong></td><td><strong>Statut</strong></td><td><strong>Prix</strong></td><tr></thead>";
                        foreach (var item in panier.ListOfConversions)
                        {
                            message.Body += "<tr>";
                            message.Body += "<td>" + item.FILE_URL_ACCESS + "</td>";
                            message.Body += "<td>" + item.FORMAT_BASE + "</td>";
                            message.Body += "<td>" + item.FORMAT_CONVERT + "</td>";
                            message.Body += "<td>" + item.STATUS + "</td>";
                            message.Body += "<td>" + item.PRICE + "</td>";
                            message.Body += "</tr>";
                        }
                        message.Body += "</table><hr/><h3 style='color: green'>Montant total de votre commande :<span  style='float:right;' >" + panier.GlobalPrice + "€</span></h3>";
                        message.Body += "Nous vous souhaitons une agréable journée. <br/>";
                        message.Body += "Cordialement, <br/>";
                        message.Body += "<p> L'équipe TRANSCODER France </p> <br/>";
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
            return message;
        }

    }
}
