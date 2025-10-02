using System.ComponentModel.DataAnnotations;

namespace CerealLib
{
    public class User
    {
        [Key]
        public int User_Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
