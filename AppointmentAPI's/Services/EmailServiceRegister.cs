using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace AppointmentAPI_s.Services
{
    public class EmailServiceRegister
    {
        private readonly IConfiguration _configuration;

        public EmailServiceRegister(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Appointment System", "no-reply@appointmentsystem.com"));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = "Welcome to Appointment Management System";

            email.Body = new TextPart("html")
            {
                Text = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2 style='color: #4CAF50;'>Welcome to Appointment Management System</h2>
                        <p>Dear {userName},</p>
                        <p>Thank you for registering with our system. We are excited to have you on board!</p>
                        <p>If you have any questions, feel free to contact our support team.</p>
                        <p>Best regards,</p>
                        <p><strong>Appointment Management Team</strong></p>
                    </body>
                    </html>"
            };

            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync("smtp.your-email-provider.com", 587, false);
                await smtp.AuthenticateAsync("your-email@example.com", "your-email-password");
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                // Log the error in real application
                Console.WriteLine($"Email sending failed: {ex.Message}");
            }
        }
    }
}
