using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class User
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

}
