using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using Web_App.Data;
using Web_App.Settings;
using Microsoft.Extensions.Options;

namespace Web_App.Services
{
    public interface IEmailService
    {
        public Task<bool> SendConfirmationAsync(string To, string Subject, string Body);
    }
    public class EmailService : IEmailService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IOptions<SmtpSetting> _smtpSetting;
        private const int CREDENTIAL_ID = 1;
        public EmailService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IOptions<SmtpSetting> smtpSetting)
        {
            _dbContextFactory = dbContextFactory;
            _smtpSetting = smtpSetting;
        }

        public async Task<bool> SendConfirmationAsync(string To, string Subject, string Body)
        {
            using ApplicationDbContext appDbContext = await _dbContextFactory.CreateDbContextAsync();
            Credentials? smtp = await appDbContext.Credentials.AsNoTracking().Where(c => c.CredentialId == CREDENTIAL_ID).FirstOrDefaultAsync();
            if (smtp is not null)
            {
                try
                {
                    var message = new MailMessage(smtp.Username, To, Subject, Body);

                    using var emailClient = new SmtpClient(_smtpSetting.Value.Host)
                    {
                        Port = _smtpSetting.Value.Port,
                        Credentials = new NetworkCredential(smtp.Username, smtp.Password),
                        EnableSsl = true
                    };

                    await emailClient.SendMailAsync(message);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
