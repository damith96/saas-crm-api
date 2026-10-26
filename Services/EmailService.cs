using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using VertexCRM.Interfaces;

namespace VertexCRM.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:3000";
        var resetLink = $"{frontendUrl}/auth/reset-password?token={Uri.EscapeDataString(resetToken)}";

        var smtp = _configuration.GetSection("Smtp");

        var fromEmail = smtp["FromEmail"] ?? "no-reply@vertexcrm.com";
        var fromName = smtp["FromName"] ?? "Vertex CRM";
        var host = smtp["Host"] ?? throw new InvalidOperationException("Smtp:Host is not configured.");
        var port = int.Parse(smtp["Port"] ?? "587");
        var username = smtp["Username"] ?? throw new InvalidOperationException("Smtp:Username is not configured.");
        var password = smtp["Password"] ?? throw new InvalidOperationException("Smtp:Password is not configured.");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Reset your Vertex CRM password";
        message.Body = new TextPart("html")
        {
            Text = $"""
                <!DOCTYPE html>
                <html>
                <body style="font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 24px;">
                  <h2 style="color: #4F46E5;">Password Reset Request</h2>
                  <p>We received a request to reset the password for your Vertex CRM account.</p>
                  <p>Click the button below to reset your password. This link expires in <strong>1 hour</strong>.</p>
                  <a href="{resetLink}"
                     style="display:inline-block;padding:12px 24px;background:#4F46E5;color:#fff;text-decoration:none;border-radius:6px;margin:16px 0;">
                    Reset Password
                  </a>
                  <p>Or copy and paste this URL into your browser:</p>
                  <p style="word-break:break-all;color:#6B7280;">{resetLink}</p>
                  <hr style="border:none;border-top:1px solid #E5E7EB;margin:24px 0;" />
                  <p style="font-size:12px;color:#9CA3AF;">
                    If you didn't request a password reset, you can safely ignore this email.
                    Your password will not be changed.
                  </p>
                </body>
                </html>
                """
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(username, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("[EmailService] Password reset email sent to {Email}", toEmail);
    }
}
