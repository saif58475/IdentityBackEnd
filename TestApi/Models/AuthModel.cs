using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;

namespace TestApi.Models
{
    public class AuthModel
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles{ get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn{ get; set; }
    }
    public class RegisterModel
    {
        [Required]
        public string FirstName { get; set;}
        [Required]
        public string LastName { get; set; }
        public string UserName{ get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class SignInModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
