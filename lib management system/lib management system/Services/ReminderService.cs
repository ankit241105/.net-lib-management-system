using Microsoft.Extensions.Options;
using lib_management_system.Common;
using lib_management_system.Models;
using lib_management_system.Repositories;

namespace lib_management_system.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IBorrowRepository _borrowRepo;
        private readonly IEmailService _email;
        private readonly EmailSettings _settings;
        private readonly ILogger<ReminderService> _logger;

        public ReminderService(
            IBorrowRepository borrowRepo,
            IEmailService email,
            IOptions<EmailSettings> settings,
            ILogger<ReminderService> logger)
        {
            _borrowRepo = borrowRepo;
            _email = email;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<int> SendDueRemindersAsync()
        {
            var cutoff = DateTime.UtcNow.AddHours(_settings.ReminderHours);
            var due = await _borrowRepo.GetDueForReminderAsync(cutoff);

            var sent = 0;
            foreach (var r in due)
            {
                var email = r.User?.Email;
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogInformation(
                        "Loan {LoanId}: no email on the account, reminder skipped.", r.Id);
                    continue;   // leave ReminderSentAt null so it can still go out if an email is added
                }

                await _email.SendAsync(email, BuildSubject(r), BuildBody(r));
                r.ReminderSentAt = DateTime.UtcNow;   // mark once -> never resent
                sent++;
            }

            if (due.Count > 0) await _borrowRepo.SaveChangesAsync();
            _logger.LogInformation(
                "Due-reminder run: {Sent} email(s) sent for {DueCount} loan(s) due within {Hours}h.",
                sent, due.Count, _settings.ReminderHours);
            return sent;
        }

        private static string BuildSubject(BorrowRecord r) =>
            $"Reminder: \"{r.Book?.Title}\" is due soon";

        private static string BuildBody(BorrowRecord r) =>
            $@"<p>Hi {System.Net.WebUtility.HtmlEncode(r.User?.Username)},</p>
<p>This is a reminder that your borrowed book
<strong>{System.Net.WebUtility.HtmlEncode(r.Book?.Title)}</strong> is due on
<strong>{r.DueDate:dddd, dd MMM yyyy}</strong>.</p>
<p>Please return it on time to avoid a late fine of <strong>₹{Fines.PerDay}/day</strong>.</p>
<p>Thank you,<br/>Library Management</p>";
    }
}
