using dankweb.API;
using DashLib.Models.Network;
using DashLib.Models.Settings;
using DashLib.Models.Settings.Monitoring;
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
    public class MailController(IDbContextFactory<DashDbContext> dbContext, web.Services.MailService mailService) : Controller
    {
        private readonly IDbContextFactory<DashDbContext> _dbFactory = dbContext;
        private readonly web.Services.MailService _mailService = mailService;

        private MimeMessage CreateMessage(string title, string body)
        {
            var settings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);

            if (null == settings)
                throw new InvalidDataException("couldnt fetch settings file");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("dash Alert Service", "alerts@dash.net"));
            message.To.Add(new MailboxAddress("username", settings.MonitoringSettings.SmtpSettings.TargetEmail));
            message.Subject = title;
            message.Body = new TextPart("html") { Text = body };

            return message;
        }

        private SmtpClient ConnectMail()
        {
            var settings = AllSettings.GetCurrentSettingsFile(AllSettings.SettingsPath);

            if (settings.MonitoringSettings.SmtpSettings.Password != string.Empty)
            {
                settings.MonitoringSettings.SmtpSettings = SmtpSettings.DecryptPassword(settings.MonitoringSettings.SmtpSettings);
            }

            if (null == settings)
                throw new InvalidDataException("couldnt fetch settings file");

            var client = new SmtpClient();
            client.Connect(settings.MonitoringSettings.SmtpSettings.ServerAddress, settings.MonitoringSettings.SmtpSettings.Port, false);

            if (settings.MonitoringSettings.SmtpSettings.AuthenticationIsRequired)
            {
                client.Authenticate(settings.MonitoringSettings.SmtpSettings.Username, settings.MonitoringSettings.SmtpSettings.Password);
            }

            return client;
        }

        [HttpGet]
        [Route("[controller]/v1/send/test")]
        public async Task<IActionResult> SendTestEmail()
        {
            try 
            {
                await _mailService.SendMailAsync("Test email", "body text");
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
