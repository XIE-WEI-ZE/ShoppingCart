namespace prjECommerceDemo.ViewModel
{
    public class CKeywordViewModel
    {
        public string? txtKeyword { get; set; }
        // 關鍵字（商品名 / 描述）
        public decimal? minPrice { get; set; } // 最低價格
        public decimal? maxPrice { get; set; } // 最高價格
    }
}
