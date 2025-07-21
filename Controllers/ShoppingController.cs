using Microsoft.AspNetCore.Mvc;
using prjECommerceDemo.Models;
using prjECommerceDemo.ViewModel;
using System.Collections.Generic;
using System.Text.Json;
using prjECommerceDemo.ViewModel;
namespace prjECommerceDemo.Controllers
{
    public class ShoppingController : Controller
    {
        public IActionResult List(CKeywordViewModel vm)
        {
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            var query = db.TProducts.Where(p => p.FIsAvailable == true);

            if (!string.IsNullOrEmpty(vm.txtKeyword))
            {
                query = query.Where(p =>
                    p.FName.Contains(vm.txtKeyword) ||
                    p.FDescription.Contains(vm.txtKeyword)); 
            }

            if (vm.minPrice.HasValue)
            {
                query = query.Where(p => p.FPrice >= vm.minPrice);
            }

            if (vm.maxPrice.HasValue)
            {
                query = query.Where(p => p.FPrice <= vm.maxPrice);
            }

            var datas = query.ToList();

            List<CProductWrap> list = new List<CProductWrap>();
            foreach (var p in datas)
            {
                list.Add(new CProductWrap() { product = p });
            }

            return View(list);
        }


        public IActionResult Details(int? id)
        {
            if (id == null)
                return RedirectToAction("List");
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            var prod = db.TProducts.FirstOrDefault(p => p.FId == id && p.FIsAvailable == true);
            if (prod == null)
                return RedirectToAction("List");
            CProductWrap wrap = new CProductWrap() { product = prod };
            return View(wrap);
        }

        public IActionResult AddToCart(int? id)
        {
            if (id == null)
                return RedirectToAction("List");

            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            var prod = db.TProducts.FirstOrDefault(p => p.FId == id);
            if (prod == null)
                return RedirectToAction("List");

            ViewBag.FId = prod.FId;
            ViewBag.Name = prod.FName;
            ViewBag.Price = prod.FPrice ?? 0;
            ViewBag.Qty = prod.FQty ?? 0;
            ViewBag.Image = string.IsNullOrEmpty(prod.FImagePath) ? "noimage.jpg" : prod.FImagePath;

            return View();
        }


        [HttpPost]
        public IActionResult AddToCart(CAddToCartViewModel p)
        {
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            TProduct dbProd = db.TProducts.FirstOrDefault(m => m.FId == p.txtFId);
            if (dbProd == null)
                return RedirectToAction("List");
            List<ViewModel.CShoppingCartItem> cart;
            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            if (!string.IsNullOrEmpty(json))
                cart = JsonSerializer.Deserialize<List<ViewModel.CShoppingCartItem>>(json);
            else
                cart = new List<ViewModel.CShoppingCartItem>();
            ViewModel.CShoppingCartItem item = new ViewModel.CShoppingCartItem
            {
                productId = p.txtFId,
                count = p.txtCount,
                price = dbProd.FPrice ?? 0,
                product = dbProd
            };

            cart.Add(item);

            json = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);
            return RedirectToAction("List");
        }

        public IActionResult CartView()
        {
            List<CShoppingCartItem> cart = new List<CShoppingCartItem>();
            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            if (!string.IsNullOrEmpty(json))
            {
                cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);
            }
            return View(cart);

        }

        public IActionResult RemoveFromCart(int id)
        {
            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            if (!string.IsNullOrEmpty(json))
            {
                List<CShoppingCartItem> cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);
                var item = cart.FirstOrDefault(x => x.productId == id);
                if (item != null)
                {
                    cart.Remove(item);
                    json = JsonSerializer.Serialize(cart);
                    HttpContext.Session.SetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST, json);

                }


            }
            return RedirectToAction("List");
        }

        public IActionResult Checkout()
        {
            // 取得購物車資料
            string json = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            if (string.IsNullOrEmpty(json))
            {
                TempData["message"] = "購物車是空的，無法結帳";
                return RedirectToAction("CartView");
            }

            List<CShoppingCartItem> cart = JsonSerializer.Deserialize<List<CShoppingCartItem>>(json);
            if (cart == null || cart.Count == 0)
            {
                TempData["message"] = "購物車內容異常，請重新操作";
                return RedirectToAction("CartView");
            }

            // 取得登入者資訊
            string userJson = HttpContext.Session.GetString(CDictionary.SK_PURCHASED_LOGINED_USER);
            if (string.IsNullOrEmpty(userJson))
            {
                TempData["message"] = "請先登入才能結帳";
                return RedirectToAction("CartView");
            }

            TCustomer customer = JsonSerializer.Deserialize<TCustomer>(userJson);
            if (customer == null)
            {
                TempData["message"] = "會員資訊異常，請重新登入";
                return RedirectToAction("CartView");
            }

            // 寫入資料庫
            DbShoppingCartExerciseContext db = new DbShoppingCartExerciseContext();
            foreach (var item in cart)
            {
                TShoppingCart order = new TShoppingCart
                {
                    FDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),  
                    FCustomerId = customer.FId,
                    FProductId = item.productId,
                    FCount = item.count,
                    FPrice = item.price
                };
                db.TShoppingCarts.Add(order);
            }
            db.SaveChanges();

            // 清空購物車 Session
            HttpContext.Session.Remove(CDictionary.SK_PURCHASED_PRODUCTS_LIST);
            TempData["message"] = " 結帳成功！";
            return RedirectToAction("CartView");
        }

    }
}
