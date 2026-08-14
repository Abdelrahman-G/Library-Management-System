namespace Library_Management_System.Models
{
    public class Member
    {
        public int MemberId { get; set; }
        public string MembershipNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Address {  get; set; } = string.Empty;
        public ICollection<BorrowingTransaction> BorrowingTransactions { get; set; } = new List<BorrowingTransaction>();
        public DateTime JoinDate { get; set; } = DateTime.Now;
        //public DateTime MembershipDate { get; set; }

    }
}
