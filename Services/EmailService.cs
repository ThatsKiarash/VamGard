using System.Net;
using System.Net.Mail;

namespace VamYab.Services;

public class EmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string toEmail)
    {
        try
        {
            var smtpHost = _config["Smtp:Host"] ?? "vamgard.org";
            var smtpPort = int.Parse(_config["Smtp:Port"] ?? "465");
            var smtpUser = _config["Smtp:Username"] ?? "info@vamgard.org";
            var smtpPass = _config["Smtp:Password"] ?? "";

            if (string.IsNullOrEmpty(smtpPass))
            {
                _logger.LogWarning("SMTP password not configured, skipping welcome email");
                return;
            }

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var msg = new MailMessage
            {
                From = new MailAddress(smtpUser, "وام‌گرد"),
                Subject = "به خبرنامه وام‌گرد خوش آمدید!",
                IsBodyHtml = true,
                Body = $@"
                    <div dir='rtl' style='font-family:Tahoma,Arial,sans-serif;max-width:600px;margin:0 auto;padding:20px;'>
                        <div style='background:linear-gradient(135deg,#2563eb,#059669);padding:30px;border-radius:16px 16px 0 0;text-align:center;'>
                            <h1 style='color:#fff;margin:0;font-size:24px;'>وام‌گرد</h1>
                            <p style='color:rgba(255,255,255,0.8);margin:8px 0 0;'>مرجع جامع مقایسه وام‌های بانکی ایران</p>
                        </div>
                        <div style='background:#fff;padding:30px;border:1px solid #e5e7eb;border-top:none;border-radius:0 0 16px 16px;'>
                            <h2 style='color:#1e293b;font-size:18px;margin:0 0 16px;'>سلام! 👋</h2>
                            <p style='color:#475569;line-height:1.8;'>از اینکه در خبرنامه وام‌گرد عضو شدید متشکریم.</p>
                            <p style='color:#475569;line-height:1.8;'>از این پس، جدیدترین اخبار وام‌های بانکی، تغییرات نرخ سود و فرصت‌های ویژه تسهیلات را در ایمیل خود دریافت خواهید کرد.</p>
                            <div style='text-align:center;margin:24px 0;'>
                                <a href='https://vamgard.org' style='display:inline-block;background:#2563eb;color:#fff;padding:12px 32px;border-radius:8px;text-decoration:none;font-weight:bold;'>مشاهده وام‌ها</a>
                            </div>
                            <p style='color:#94a3b8;font-size:13px;text-align:center;margin:16px 0 0;'>این ایمیل به {toEmail} ارسال شده است.</p>
                        </div>
                    </div>"
            };
            msg.To.Add(toEmail);

            await client.SendMailAsync(msg);
            _logger.LogInformation("Welcome email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
        }
    }
}
