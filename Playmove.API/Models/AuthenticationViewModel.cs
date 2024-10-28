using System.ComponentModel.DataAnnotations;

namespace Playmove.API.Models
{
    public class UserRegisterVM
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [EmailAddress(ErrorMessage = "O email digitado não é válido!")]
        public string Email { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        
        public string Nome { get; set; }
    }

     public class AuthenticationVM
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [EmailAddress(ErrorMessage = "O email digitado não é válido!")]
        public string Email { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "O campo {0} é obrigatório!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       
    }


    public class RoleVM
    {
        [Required]
        public string Nome { get; set; }


    }

    public class UserRoleVM
    {
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string RoleName { get; set; }


    }


}
