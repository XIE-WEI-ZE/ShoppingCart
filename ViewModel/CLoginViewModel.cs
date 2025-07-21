using System.ComponentModel.DataAnnotations;

namespace prjECommerceDemo.ViewModel
{
    public class CLoginViewModel
    {
        [Display(Name = "帳號")]
        public string? txtAccount { get; set; }
        [Display(Name = "密碼")]
        public string? txtPassword { get; set; }
        [Display(Name = "記住我")]
        public bool RememberMe { get; set; }
    }
}
