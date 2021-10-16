using System.ComponentModel.DataAnnotations;

namespace BaseApiWithIdentity.Controllers
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Password is requried!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "New password is requried!")]
        public string NewPassword { get; set; }
    }
}
