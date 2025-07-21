using System.ComponentModel.DataAnnotations;

namespace prjECommerceDemo.ViewModel
{
    public class CRegisterViewModel
    {
        public string FName { get; set; }
        public string FPhone { get; set; }
        public string FEmail { get; set; }
        public string FAccount { get; set; }
        public string FPassword { get; set; }

        public string FGender { get; set; }
        public string FCity { get; set; }
        public string FDistrict { get; set; }
        public string FRoadAddress { get; set; }
        // 🔽 三個欄位組生日
        public int BirthYear { get; set; }
        public int BirthMonth { get; set; }
        public int BirthDay { get; set; }

        // ✅ 若你 Controller 要存 DateOnly，這裡可以多一個屬性組合
        public DateOnly? FBirthday =>
            BirthYear > 0 && BirthMonth > 0 && BirthDay > 0
            ? new DateOnly(BirthYear, BirthMonth, BirthDay)
            : null;
    }
}
