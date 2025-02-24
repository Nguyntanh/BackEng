namespace signup.Models
{
    public class User
    {
        public string Email { get; set; } // Đặt làm khóa chính
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}