using dankweb.API;
using DashLib.Network;
using DashLib.Settings;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using web.Client.Pages;
using web.Services;

namespace web.Controllers
{
    [ApiController]
    public class MailController(IDbContextFactory<DashDbContext> dbContext) : Controller
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory = dbContext;

        private MimeMessage CreateMessage(string title, string body)
        {
            var settings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);

            if (null == settings)
                throw new InvalidDataException("couldnt fetch settings file");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("dash Alert Service", "alerts@dash.net"));
            message.To.Add(new MailboxAddress("username", settings.MonitoringSettings.SmtpTargetEmail));
            message.Subject = title;
            message.Body = new TextPart("html") { Text = body };

            return message;
        }

        private SmtpClient ConnectMail()
        {
            var settings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);

            if (settings.MonitoringSettings.SmtpPassword != string.Empty)
            {
                settings.MonitoringSettings = MonitoringSettings.DecryptPassword(settings.MonitoringSettings);
            }

            if (null == settings)
                throw new InvalidDataException("couldnt fetch settings file");

            var client = new SmtpClient();
            client.Connect(settings.MonitoringSettings.SmtpServerAddress, settings.MonitoringSettings.SmtpPort, false);

            if (settings.MonitoringSettings.SmtpAuthenticationIsRequired)
            {
                client.Authenticate(settings.MonitoringSettings.SmtpUsername, settings.MonitoringSettings.SmtpPassword);
            }

            return client;
        }

        [HttpGet]
        [Route("[controller]/v1/send/test")]
        public async Task<Results<BadRequest<string>, Ok<string>>> SendTestEmail()
        {
            var message = CreateMessage("Test Email", "Test body text");
            using var client = ConnectMail();
            
            try
            {
                await client.SendAsync(message);
                client.Disconnect(true);
                return TypedResults.Ok("success");
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("[controller]/v1/send/downtimealert")]
        public async Task<Results<BadRequest<string>, Ok<List<IP>>>> SendDowntimeAlerts(List<IP> ipList)
        {
            var settings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);

            using var client = ConnectMail();
            using var context = _dbFactory.CreateDbContext();

            var oldDate = DateTime.UtcNow.AddMinutes(-60);
            bool send = false;

            string body = string.Empty;
            
            if (settings != null)
            {
                body = $"<h2>Downtime Alert</h2><p>The following have been down for more than {settings.MonitoringSettings.AlertIfDownForPercent}% of the last {settings.MonitoringSettings.AlertTimePeriodInMinutes} minutes::</p><ul>";
                oldDate = DateTime.UtcNow.AddMinutes(settings.MonitoringSettings.AlertAgainAfterInMinutes);
            }
            else
            {
                body = "<h2>Downtime Alert</h2><p>The following have been down for more than 50% of the last hour::</p><ul>";
            }

                
            foreach (var ip in ipList)
            {
                if (ip.LastAlertSent < oldDate)
                {
                    body += $"<li>{IP.ConvertToString(ip.Address)}</li>";
                    send = true;

                    var dbIp = context.IPs.Find(ip.ID);
                    if (dbIp != null)
                    {
                        dbIp.LastAlertSent = DateTime.UtcNow;
                        context.IPs.Update(dbIp);
                    }
                }
                
            }
            body += "</ul>";

            var message = CreateMessage("Downtime Alert", body);

            try
            {
                if (send)
                {
                    await client.SendAsync(message);
                    await context.SaveChangesAsync();
                }
                return TypedResults.Ok(ipList);
            }
            catch (Exception ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        }
    }
}
