using Microsoft.AspNetCore.Identity.UI.Services; // IEmailSender interface
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.Utility
{
    // ASP.NET Core Identity needs an IEmailSender to send things like "confirm your email"
    // and "reset your password" messages. This is a STUB implementation: it satisfies the
    // interface so the app runs, but it doesn't actually send anything (returns a completed
    // task and does nothing). To send real email you'd plug in SendGrid/SMTP here.
    // It is wired up in Program.cs with:  AddScoped<IEmailSender, EmailSender>()
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Task.CompletedTask; // no-op: pretend the email was sent successfully
        }
    }
}
