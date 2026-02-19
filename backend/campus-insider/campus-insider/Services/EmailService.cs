// Services/EmailService.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace campus_insider.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendNotificationEmailAsync(
            string toEmail,
            string toName,
            string subject,
            string message,
            string? actionUrl = null)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"],
                    _configuration["Email:FromAddress"]
                ));
                email.To.Add(new MailboxAddress(toName, toEmail));
                email.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GenerateEmailTemplate(toName, subject, message, actionUrl)
                };

                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _configuration["Email:SmtpServer"],
                    int.Parse(_configuration["Email:SmtpPort"]!),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    _configuration["Email:Username"],
                    _configuration["Email:Password"]
                );

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent to {Email}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }

        private string GenerateEmailTemplate(string name, string subject, string message, string? actionUrl)
        {
            var actionButton = !string.IsNullOrEmpty(actionUrl)
                ? $@"<tr>
                        <td style='padding: 20px 0;'>
                            <a href='{actionUrl}' style='background-color: #4F46E5; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; display: inline-block;'>
                                View Details
                            </a>
                        </td>
                    </tr>"
                : "";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
    <div style='background-color: #f8f9fa; border-radius: 10px; padding: 30px;'>
        <div style='text-align: center; margin-bottom: 30px;'>
            <h1 style='color: #4F46E5; margin: 0;'>Campus Insider</h1>
        </div>
        
        <div style='background-color: white; border-radius: 8px; padding: 30px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);'>
            <h2 style='color: #1f2937; margin-top: 0;'>Hi {name},</h2>
            
            <p style='font-size: 16px; color: #4b5563;'>{message}</p>
            
            <table width='100%'>
                {actionButton}
            </table>
            
            <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 30px 0;'>
            
            <p style='font-size: 14px; color: #6b7280;'>
                This is an automated message from Campus Insider. Please do not reply to this email.
            </p>
            
            <p style='font-size: 14px; color: #6b7280;'>
                If you have any questions, contact us at support@campus-insider.com
            </p>
        </div>
        
        <div style='text-align: center; margin-top: 20px; color: #9ca3af; font-size: 12px;'>
            <p>&copy; 2026 Campus Insider. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string firstName)
        {
            var subject = "Welcome to Campus Insider!";
            var message = @"
                <p>Thank you for joining Campus Insider, your campus community platform!</p>
                <p>You can now:</p>
                <ul>
                    <li>Share and borrow equipment</li>
                    <li>Join or create carpools</li>
                    <li>Connect with your campus community</li>
                </ul>
                <p>Get started by completing your profile and exploring what's available!</p>
            ";

            return await SendNotificationEmailAsync(toEmail, firstName, subject, message,
                _configuration["AppSettings:BaseUrl"] + "/feed");
        }
    }
}