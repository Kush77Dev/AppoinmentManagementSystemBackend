using System;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Send a general email with a custom subject and message body.
    /// </summary>
    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Appointment Management", _configuration["EmailSettings:From"]));
        emailMessage.To.Add(new MailboxAddress("", toEmail));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:Port"]),
                    MailKit.Security.SecureSocketOptions.StartTls
                );

                await client.AuthenticateAsync(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]
                );

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Send a welcome email to a new user after successful registration.
    /// </summary>
    public async Task SendWelcomeEmailAsync(string toEmail, string userName)
    {
        string subject = "Welcome to Appointment Management System";

        string htmlBody1 = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
                <h2 style='color: #4CAF50; text-align: center;'>Welcome to Appointment Management System</h2>
                <p style='font-size: 16px; color: #555;'>Dear {userName},</p>
                <p style='font-size: 16px; color: #555;'>
                    Thank you for registering with our system. We are excited to have you on board!
                </p>
                <p style='font-size: 16px; color: #555;'>
                    If you have any questions, feel free to contact our support team.
                </p>
                <p style='font-size: 16px; color: #555;'><strong>Best regards,</strong><br>Appointment Management Team</p>
            </div>";

        await SendEmailAsync(toEmail, subject, htmlBody1);
    }

    /// <summary>
    /// Send credentials email to HR Admin with their login details.
    /// </summary>
    public async Task SendHRAdminCredentialsEmailAsync(string toEmail, string password)
    {
        string subject = "Your HR Admin Account Credentials";

        string htmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
                <h2 style='color: #333; text-align: center;'>Welcome to Appointment Management</h2>
                <p style='font-size: 16px; color: #555;'>Hello,</p>
                <p style='font-size: 16px; color: #555;'>
                    Your HR Admin account has been successfully created. Below are your login credentials:
                </p>
                <div style='background-color: #fff; padding: 15px; border-radius: 5px; border: 1px solid #ccc;'>
                    <p style='font-size: 16px;'><strong>Email:</strong> {toEmail}</p>
                    <p style='font-size: 16px;'><strong>Password:</strong> {password}</p>
                </div>
                <p style='font-size: 16px; color: #555;'>
                    Please log in and change your password immediately for security reasons.
                </p>
                <p style='text-align: center; margin-top: 20px;'>
                    <a href='https://yourcompany.com/login' style='background-color: #007bff; color: #fff; text-decoration: none; padding: 10px 20px; border-radius: 5px; font-size: 16px;'>Login Now</a>
                </p>
                <p style='font-size: 16px; color: #555;'><strong>Best regards,</strong><br>Your Company Team</p>
            </div>";

        await SendEmailAsync(toEmail, subject, htmlBody);
    }

    public async Task SendOTPEmailAsync(string toEmail, string otpCode)
    {
        string subject = "Your OTP Code for Verification";

        string htmlBody = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; padding: 20px; border: 1px solid #ddd; border-radius: 10px; background-color: #f9f9f9;'>
            <h2 style='color: #007bff; text-align: center;'>OTP Verification Code</h2>
            <p style='font-size: 16px; color: #555;'>Hello,</p>
            <p style='font-size: 16px; color: #555;'>Your One-Time Password (OTP) for verification is:</p>
            <div style='background-color: #fff; padding: 15px; border-radius: 5px; border: 1px solid #ccc; text-align: center; font-size: 20px; font-weight: bold;'>
                {otpCode}
            </div>
            <p style='font-size: 16px; color: #555;'>This OTP is valid for only 5 minutes. Do not share this code with anyone.</p>
            <p style='font-size: 16px; color: #555;'><strong>Best regards,</strong><br>Appointment Management Team</p>
        </div>";

        await SendEmailAsync(toEmail, subject, htmlBody);
    }


}
