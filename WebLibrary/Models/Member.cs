namespace WebLibrary.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MembershipType { get; set; }
        public int BooksRead { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime JoinDate { get; set; }

        public ICollection<Loan>? Loans { get; set; }
        public ICollection<Review>? Reviews { get; set; }
    }
}
