namespace lib_management_system.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public DateTime? ReminderSentAt { get; set; }

        public decimal Fine { get; set; }


        public int BookId { get; set; }
        public Book? Book { get; set; }


        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
