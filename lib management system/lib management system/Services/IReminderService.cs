namespace lib_management_system.Services
{
    public interface IReminderService
    {
        // Sends "due soon" reminders for loans due within the configured window that
        // haven't been reminded yet. Returns how many emails were sent.
        Task<int> SendDueRemindersAsync();
    }
}
