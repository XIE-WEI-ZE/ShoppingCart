using Microsoft.AspNetCore.Mvc;
using prjECommerceDemo.Models;
using prjECommerceDemo.Models.Factory;
using prjECommerceDemo.ViewModel;
namespace prjECommerceDemo.Controllers

{
    public class CustomerController : SupperController
    {
        public IActionResult List(CKeywordViewModel vm, int page = 1)
        {
            int pageSize = 10;
            string keyword = vm?.txtKeyword;

            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();

            //  建立查詢基礎（query）
            var query = db.TCustomers.AsQueryable();

            //  模糊搜尋處理在 query 上
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(c =>
                    (c.FName != null && c.FName.Contains(keyword)) ||
                    (c.FPhone != null && c.FPhone.Contains(keyword)) ||
                    (c.FEmail != null && c.FEmail.Contains(keyword)) ||
                    (c.FAddress != null && c.FAddress.Contains(keyword)) ||
                    (c.FAccount != null && c.FAccount.Contains(keyword))
                );
            }

            //  總筆數與分頁查詢同樣用 query
            int totalCount = query.Count();
            var customers = query
                .OrderBy(c => c.FId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            //  傳到 View 的 ViewBag 設定
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Keyword = keyword;

            return View(customers); // View 裡用 IEnumerable<TCustomer>
        }



        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TCustomer p)
        {
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();

            // 處理 Salt 與加密
            string salt = CPasswordFactory.GetSalt();
            string hash = CPasswordFactory.HashPassword(p.FPassword, salt);
            p.FSalt = salt;
            p.FPassword = hash;

            // 處理建立時間與狀態
            p.FCreatedDate = DateTime.Now;
            p.FIsEnabled = true;

            // 組合完整地址
            p.FAddress = $"{p.FCity}{p.FDistrict}{p.FRoadAddress}";

            db.TCustomers.Add(p);
            db.SaveChanges();

            return RedirectToAction("List");
        }



        public IActionResult Edit(int? id) {
            if (id == null) return RedirectToAction("List");
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            TCustomer x = db.TCustomers.FirstOrDefault(c=>c.FId == id);
            if (x == null)
                return RedirectToAction("List");
            return View(x);
        }

        [HttpPost]
        public IActionResult Edit(TCustomer uiCust) {
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            TCustomer dbCust = db.TCustomers.FirstOrDefault(c=>c.FId == uiCust.FId);
            if(dbCust == null) return RedirectToAction("List");
            dbCust.FName = uiCust.FName;
            dbCust.FPhone = uiCust.FPhone;
            dbCust.FEmail = uiCust.FEmail;
            dbCust.FAccount = uiCust.FAccount;
            dbCust.FGender = uiCust.FGender;
            dbCust.FCity = uiCust.FCity;
            dbCust.FDistrict = uiCust.FDistrict;
            dbCust.FRoadAddress = uiCust.FRoadAddress;
            dbCust.FIsEnabled = uiCust.FIsEnabled;

            dbCust.FAddress = $"{uiCust.FCity}{uiCust.FDistrict}{uiCust.FRoadAddress}";
            db.SaveChanges();
            return RedirectToAction("List");
        }


        //改成停用帳號，替代刪除會員
        public IActionResult ToggleStatus(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            TCustomer cust = db.TCustomers.FirstOrDefault(c => c.FId == id);

            if (cust != null)
            {
                cust.FIsEnabled = !cust.FIsEnabled;
                db.SaveChanges();
            }

            return RedirectToAction("List");
        }


    }
}
