namespace lib_management_system.Common
{
    // Central lending/late-fine policy so the rule lives in exactly one place.
    public static class Fines
    {
        public const int LoanDays = 15;      // a loan is due 15 days after borrowing
        public const decimal PerDay = 10m;   // ₹10 for each day a book is overdue

        // Fine owed as of a given moment (0 if not overdue). Compared by date, so a
        // book returned on its due date is not fined.
        public static decimal Calculate(DateTime dueDate, DateTime asOf)
        {
            var daysLate = (asOf.Date - dueDate.Date).Days;
            return daysLate > 0 ? daysLate * PerDay : 0m;
        }
    }
}
