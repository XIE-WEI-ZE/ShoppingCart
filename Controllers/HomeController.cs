using Microsoft.AspNetCore.Mvc;
using prjECommerceDemo.Models;
using prjECommerceDemo.ViewModel;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
namespace prjECommerceDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbShoppingCartExerciseContext _context;
        private readonly GoogleReCaptchaSettings _captchaSettings;

        public HomeController(ILogger<HomeController> logger, DbShoppingCartExerciseContext context,
                      IOptions<GoogleReCaptchaSettings> captchaOptions)
        {
            _logger = logger;
            _context = context;
            _captchaSettings = captchaOptions.Value;
        }

        // 首頁
        public IActionResult Index()
        {
            if (HttpContext.Session.Keys.Contains(CDictionary.SK_PURCHASED_LOGINED_USER))
            {
                string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_LOGINED_USER);
                TCustomer member = JsonSerializer.Deserialize<TCustomer>(json);
                ViewBag.Message = $"歡迎回來，{member.FName}";
            }
            return View();
        }

        // 註冊 GET
        public IActionResult Register()
        {
            return View();
        }

        // 註冊 POST
        [HttpPost]
        public IActionResult Register(CRegisterViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            if (_context.TCustomers.Any(c => c.FAccount == vm.FAccount))
            {
                ViewBag.Message = "此帳號已被註冊";
                return View(vm);
            }

            string salt = Guid.NewGuid().ToString().Substring(0, 8);
            string saltedPwd = vm.FPassword + salt;
            string hashPwd = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(saltedPwd)));

            string fullAddress = $"{vm.FCity}{vm.FDistrict}{vm.FRoadAddress}";

            TCustomer cust = new TCustomer
            {
                FAccount = vm.FAccount,
                FEmail = vm.FEmail,
                FPassword = hashPwd,
                FSalt = salt,
                FName = vm.FName,
                FPhone = vm.FPhone,
                FAddress = fullAddress,
                FCity = vm.FCity,
                FDistrict = vm.FDistrict,
                FRoadAddress = vm.FRoadAddress,
                FGender = vm.FGender,
                FBirthday = vm.FBirthday,
                FCreatedDate = DateTime.Now,
                FIsEnabled = true
            };

            _context.TCustomers.Add(cust);
            _context.SaveChanges();

            TempData["message"] = "註冊成功，請登入系統";
            return RedirectToAction("Login");
        }

        public IActionResult EditProfile()
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_LOGINED_USER);
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Login");

            TCustomer member = JsonSerializer.Deserialize<TCustomer>(json);

            var vm = new CEditProfileViewModel
            {
                FName = member.FName,
                FPhone = member.FPhone,
                FEmail = member.FEmail,
                FCity = member.FCity,
                FDistrict = member.FDistrict,
                FRoadAddress = member.FRoadAddress,
                FGender = member.FGender,
                ExistingPhoto = member.FPhoto
            };

            if (member.FBirthday.HasValue)
            {
                var date = member.FBirthday.Value;
                vm.BirthYear = date.Year;
                vm.BirthMonth = date.Month;
                vm.BirthDay = date.Day;
            }

            return View(vm);
        }


        [HttpPost]
        public IActionResult EditProfile(CEditProfileViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_LOGINED_USER);
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Login");

            TCustomer member = JsonSerializer.Deserialize<TCustomer>(json);
            var dbMember = _context.TCustomers.FirstOrDefault(m => m.FId == member.FId);
            if (dbMember == null)
                return RedirectToAction("Login");

            dbMember.FName = vm.FName;
            dbMember.FPhone = vm.FPhone;
            dbMember.FEmail = vm.FEmail;
            dbMember.FCity = vm.FCity;
            dbMember.FDistrict = vm.FDistrict;
            dbMember.FRoadAddress = vm.FRoadAddress;
            dbMember.FAddress = $"{vm.FCity}{vm.FDistrict}{vm.FRoadAddress}";
            dbMember.FGender = vm.FGender;
            if (vm.BirthYear.HasValue && vm.BirthMonth.HasValue && vm.BirthDay.HasValue)
            {
                dbMember.FBirthday = new DateOnly(vm.BirthYear.Value, vm.BirthMonth.Value, vm.BirthDay.Value);
            }
            else
            {
                dbMember.FBirthday = null;
            }


            if (vm.Photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", photoName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    vm.Photo.CopyTo(stream);
                }
                dbMember.FPhoto = photoName;
            }

            _context.SaveChanges();

            // 更新 Session 中的資料
            string updatedJson = JsonSerializer.Serialize(dbMember);
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_LOGINED_USER, updatedJson);

            TempData["Message"] = "個人資料已更新成功！";
            return RedirectToAction("EditProfile");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(CChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_LOGINED_USER);
            if (string.IsNullOrEmpty(json))
                return RedirectToAction("Login");

            TCustomer member = JsonSerializer.Deserialize<TCustomer>(json);
            var dbmember = _context.TCustomers.FirstOrDefault(m => m.FId == member.FId);
            if (dbmember == null)
                return RedirectToAction("Login");

            // 驗證舊密碼
            string salted = vm.OldPassword + dbmember.FSalt;
            string hashOld = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(salted)));
            if (hashOld != dbmember.FPassword)
            {
                ViewBag.Message = "舊密碼錯誤";
                return View(vm);
            }

            // 新密碼加密
            string newSalt = Guid.NewGuid().ToString().Substring(0, 8);
            string newSalted = vm.NewPassword + newSalt;
            string newHash = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(newSalted)));

            dbmember.FSalt = newSalt;
            dbmember.FPassword = newHash;
            _context.SaveChanges();

            ViewBag.Message = "新密碼已成功更新";
            return View();
        }


        // 登入 GET
        public IActionResult Login()
        {
            CLoginViewModel vm = new CLoginViewModel();
            ViewBag.SiteKey = _captchaSettings.SiteKey;
            if (Request.Cookies.ContainsKey("RememberAccount"))
            {
                vm.txtAccount = Request.Cookies["RememberAccount"];
                vm.RememberMe = true;
            }
            return View(vm);
        }

        // 登入 POST
        [HttpPost]
        public async Task<IActionResult> Login(CLoginViewModel vm)
        {
            // Step 1: 驗證 reCAPTCHA token
            string token = Request.Form["g-recaptcha-response"];
            if (!await ValidateCaptchaAsync(token))
            {
                ViewBag.SiteKey = _captchaSettings.SiteKey;
                ViewBag.Error = "reCAPTCHA 驗證失敗，請再試一次";
                return View(vm);
            }

            // Step 2: 表單驗證
            if (!ModelState.IsValid)
            {
                ViewBag.SiteKey = _captchaSettings.SiteKey;
                ViewBag.Error = "請輸入帳號與密碼";
                return View(vm);
            }

            var member = _context.TCustomers.FirstOrDefault(m => m.FAccount == vm.txtAccount && m.FIsEnabled == true);
            if (member == null)
            {
                ViewBag.SiteKey = _captchaSettings.SiteKey;
                ViewBag.Error = "帳號錯誤或帳號已被停權";
                return View(vm);
            }

            if (string.IsNullOrEmpty(member.FSalt) || string.IsNullOrEmpty(member.FPassword))
            {
                ViewBag.SiteKey = _captchaSettings.SiteKey;
                ViewBag.Error = "帳號尚未設定密碼，請聯絡管理員";
                return View(vm);
            }

            string saltedPwd = vm.txtPassword + member.FSalt;
            string hashPwd = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(saltedPwd)));

            if (hashPwd != member.FPassword)
            {
                ViewBag.SiteKey = _captchaSettings.SiteKey;
                ViewBag.Error = "密碼錯誤";
                return View(vm);
            }

            if (vm.RememberMe)
            {
                CookieOptions option = new CookieOptions { Expires = DateTime.Now.AddDays(7) };
                Response.Cookies.Append("RememberAccount", member.FAccount, option);
            }
            else
            {
                Response.Cookies.Delete("RememberAccount");
            }

            string json = JsonSerializer.Serialize(member);
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_LOGINED_USER, json);

            return RedirectToAction("Index");
        }

        // ✅ 驗證 reCAPTCHA token
        private async Task<bool> ValidateCaptchaAsync(string token)
        {
            string secret = _captchaSettings.SecretKey;
            using var client = new HttpClient();
            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={token}", null);
            var json = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);
            return document.RootElement.GetProperty("success").GetBoolean();
        }





        // 登出
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_LOGINED_USER);
            return RedirectToAction("Index");
        }

        public IActionResult Profile() {
            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_LOGINED_USER);
            if(string.IsNullOrEmpty(json))
                return RedirectToAction("Login");
            TCustomer member = JsonSerializer.Deserialize<TCustomer>(json);
            return View(member);       // 使用原本的 TCustomer 當 model
        }

        //忘記密碼
        public IActionResult ForgetPassword() { 
            return View();  
        }
        [HttpPost]
        public IActionResult ForgetPassword(CForgetPasswordViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // 查詢信箱是否存在
            var user = _context.TCustomers.FirstOrDefault(c => c.FEmail == vm.Email);
            if (user == null)
            {
                TempData["Message"] = "查無此信箱，請確認是否已註冊";
                return RedirectToAction("ForgetPassword");
            }

            // 產生臨時密碼
            string tempPwd = Guid.NewGuid().ToString().Substring(0, 8);
            string salt = Guid.NewGuid().ToString().Substring(8, 8);
            string salted = tempPwd + salt;
            string hashPwd = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(salted)));

            // 更新密碼與臨時狀態
            user.FSalt = salt;
            user.FPassword = hashPwd;
            user.IsTempPassword = true;
            _context.SaveChanges();

            // 寄出 Email
            string subject = "臨時密碼通知";
            string body = $"您的臨時密碼為：{tempPwd}\n請使用該密碼登入並盡快修改為正式密碼。";
            try
            {
                SendEmail(user.FEmail, subject, body);
                TempData["Message"] = "已寄出臨時密碼至您的信箱，請盡快登入修改密碼。";
            }
            catch (Exception ex)
            {
                TempData["Message"] = "發送 Email 發生錯誤，請稍後再試";
            }

            return RedirectToAction("ForgetPassword");
        }

        private void SendEmail(string to, string subject, string body) {
            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("xie8610@gmail.com", "tokq vhwr szed adsd"),
                EnableSsl = true
            };
            var message = new MailMessage("xie8610@gmail.com", to)
            {
                Subject = subject,
                Body = body
            };
            smtp.Send(message);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
