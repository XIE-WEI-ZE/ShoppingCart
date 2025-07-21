using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace prjECommerceDemo.ViewModel
{
    public class CEditProfileViewModel
    {
        [Display(Name = "姓名")]
        public string? FName { get; set; }

        [Display(Name = "電話")]
        public string? FPhone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string? FEmail { get; set; }

        [Display(Name = "縣市")]
        public string? FCity { get; set; }

        [Display(Name = "鄉鎮區")]
        public string? FDistrict { get; set; }

        [Display(Name = "路段地址")]
        public string? FRoadAddress { get; set; }

        [Display(Name = "性別")]
        public string? FGender { get; set; }

        [Display(Name = "生日")]
        [DataType(DataType.Date)]
        public int? BirthYear { get; set; }
        public int? BirthMonth { get; set; }
        public int? BirthDay { get; set; }

        [Display(Name = "上傳頭像")]
        public IFormFile? Photo { get; set; }

        public string? ExistingPhoto { get; set; } // 用於顯示舊照片
    }
}
