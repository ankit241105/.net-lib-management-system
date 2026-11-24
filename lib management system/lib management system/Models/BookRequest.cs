namespace lib_management_system.Models
{
    public class BookRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }

        // "Pending" | "Approved" | "Declined"
        public string Status { get; set; } = "Pending";

        public DateTime RequestedDate { get; set; }
        public DateTime? DecisionDate { get; set; }
    }
}
