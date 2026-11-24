namespace lib_management_system.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        public List<BorrowRecord> BorrowRecords { get; set; } = new();
    }
}
