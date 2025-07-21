using System.ComponentModel.DataAnnotations;

namespace prjECommerceDemo.ViewModel
{
    public class CForgetPasswordViewModel
    {
        [Required(ErrorMessage ="請輸入註冊時的email")]
        [EmailAddress]
        [Display(Name ="註冊 Email")]
        public string Email { get; set; }
    }
}
