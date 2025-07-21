using System.ComponentModel.DataAnnotations;

namespace prjECommerceDemo.ViewModel
{
    public class CChangePasswordViewModel
    {
        [Required(ErrorMessage = "請輸入舊密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "舊密碼")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "請輸入新密碼")]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "請再次輸入新密碼")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "兩次密碼不一致")]
        [Display(Name = "確認新密碼")]
        public string ConfirmPassword { get; set; }
    }
}
