namespace WebLibrary.Models
{
    public class Loan
    {
        public int LoanId { get; set; }

        public int MemberId { get; set; }
        public int BookId { get; set; }
        public int StaffId { get; set; }

        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string LoanStatus { get; set; }

        public Member Member { get; set; }
        public Book Book { get; set; }
        public Staff Staff { get; set; }
    }
}
