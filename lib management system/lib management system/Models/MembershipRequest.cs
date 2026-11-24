namespace lib_management_system.Models
{
    // A User's request to upgrade to a Member (mock ₹199 payment). An Admin approves
    // (which flips the user's role to "Member") or declines it.
    public class MembershipRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        // Recorded amount only - no real payment gateway.
        public decimal Amount { get; set; }

        // "Pending" | "Approved" | "Declined"
        public string Status { get; set; } = "Pending";

        public DateTime RequestedDate { get; set; }
        public DateTime? DecisionDate { get; set; }
    }
}
