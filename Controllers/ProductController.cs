using Microsoft.AspNetCore.Mvc;
using prjECommerceDemo.Models;
using prjECommerceDemo.ViewModel;

namespace prjECommerceDemo.Controllers
{
    public class ProductController : SupperController
    {
        public IActionResult List(CKeywordViewModel vm, int page = 1) {
            int pageSize = 10;
            string keyword = vm?.txtKeyword;
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            var query = db.TProducts.AsQueryable();
            if (!string.IsNullOrEmpty(keyword)) {
                query = query.Where(p => p.FName.Contains(keyword) || p.FDescription.Contains(keyword));
            }
            int totalCount = query.Count();
            var datas = query.OrderBy(p=>p.FId).Skip((page-1)*pageSize).Take(pageSize).ToList();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            ViewBag.Keyword = keyword;

            return View(datas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CProductWrap wrap)
        {
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            TProduct prod = wrap.product;

            //  上傳圖片處理
            if (wrap.photo != null)
            {
                string photoName = Guid.NewGuid().ToString() + ".jpg";
                string savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", photoName);
                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    wrap.photo.CopyTo(stream);
                }
                prod.FImagePath = photoName;
            }

            //  系統自動補欄位
            prod.FCreatedDate = DateTime.Now;
            prod.FUpdatedDate = DateTime.Now;
            prod.FIsAvailable = true;

            db.TProducts.Add(prod);
            db.SaveChanges();

            return RedirectToAction("List");
        }

        public IActionResult Edit(int? id) {
            if (id == null)
                return RedirectToAction("List");
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            TProduct prod = db.TProducts.FirstOrDefault(p => p.FId == id);
            if(prod == null) return RedirectToAction("List");
            CProductWrap wrap = new CProductWrap();
            wrap.product = prod;
            return View(wrap);
        }

        [HttpPost]
        public IActionResult Edit(CProductWrap wrap) {
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            TProduct dbProd = db.TProducts.FirstOrDefault(p => p.FId == wrap.FId);
            if (dbProd == null)
                return RedirectToAction("List");

            if (wrap.photo != null) {
                string photoName = Guid.NewGuid().ToString()+".jpg";
                string savePath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/images",photoName);
                using (var stream = new FileStream(savePath, FileMode.Create)) { 
                    wrap.photo.CopyTo(stream);
                }
                dbProd.FImagePath = photoName;
            }

            dbProd.FName = wrap.FName;
            dbProd.FQty = wrap.FQty;
            dbProd.FCost = wrap.FCost;
            dbProd.FPrice = wrap.FPrice;
            dbProd.FDescription = wrap.FDescription;
            dbProd.FCategoryId = wrap.FCategoryId;
            dbProd.FIsAvailable = wrap.FIsAvailable;
            dbProd.FUpdatedDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("List");
        }

        //產品上架/下架狀態
        public IActionResult ToggleAvailable(int? id) {
            if (id == null)
                return RedirectToAction("List");
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            var prod = db.TProducts.FirstOrDefault(P=>P.FId == id);
            if (prod != null) {
                prod.FIsAvailable = !(prod.FIsAvailable ?? true);
                prod.FUpdatedDate = DateTime.Now;
                db.SaveChanges();
            }
            return RedirectToAction("List");
        
        }

        public IActionResult Delete(int? id) {
            if(id == null)
                return RedirectToAction("List");
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            var prod = db.TProducts.FirstOrDefault(p=>p.FId == id);
            if (prod != null) {
                db.TProducts.Remove(prod);
                db.SaveChanges();
            }
            return RedirectToAction("List");  
        }

        public IActionResult CopyAndCreate(int? id) { 
            if(id == null)
                return RedirectToAction("List");
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            TProduct prod = db.TProducts.FirstOrDefault(p => p.FId == id);
            if (prod == null) {
                return RedirectToAction("List");
            }
            CProductWrap wrap = new CProductWrap { 
                FName = prod.FName,
                FQty = prod.FQty,
                FCost = prod.FCost,
                FPrice = prod.FPrice,
                FImagePath  = prod.FImagePath,
                FDescription = prod.FDescription,
                FCategoryId = prod.FCategoryId,
                FIsAvailable = prod.FIsAvailable
            };
            return View("Create",wrap);
        }
    }
}
