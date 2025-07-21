using prjECommerceDemo.Models;
using System.ComponentModel;

namespace prjECommerceDemo.ViewModel
{
    public class CShoppingCartItem
    {
        public TProduct product { get; set; }
        public int productId { get; set; }
        public int count { get; set; }
        public decimal price { get; set; }
        [DisplayName("售價")]
        public decimal 小計 { get { return this.count * this.price; } }
    }
}
