using Microsoft.AspNetCore.Http;
using prjECommerceDemo.Models;

namespace prjECommerceDemo.ViewModel
{
    public class CProductWrap
    {
        private TProduct _product;

        public CProductWrap()
        {
            _product = new TProduct();
        }

        public TProduct product
        {
            get { return _product; }
            set { _product = value; }
        }

        public int FId
        {
            get { return _product.FId; }
            set { _product.FId = value; }
        }

        public string FName
        {
            get { return _product.FName; }
            set { _product.FName = value; }
        }

        public int? FQty
        {
            get { return _product.FQty; }
            set { _product.FQty = value; }
        }

        public decimal? FCost
        {
            get { return _product.FCost; }
            set { _product.FCost = value; }
        }

        public decimal? FPrice
        {
            get { return _product.FPrice; }
            set { _product.FPrice = value; }
        }

        public string FImagePath
        {
            get { return _product.FImagePath; }
            set { _product.FImagePath = value; }
        }

        public int? FCategoryId
        {
            get { return _product.FCategoryId; }
            set { _product.FCategoryId = value; }
        }

        public string FDescription
        {
            get { return _product.FDescription; }
            set { _product.FDescription = value; }
        }

        public bool? FIsAvailable
        {
            get { return _product.FIsAvailable; }
            set { _product.FIsAvailable = value; }
        }

        public DateTime? FCreatedDate
        {
            get { return _product.FCreatedDate; }
            set { _product.FCreatedDate = value; }
        }

        public DateTime? FUpdatedDate
        {
            get { return _product.FUpdatedDate; }
            set { _product.FUpdatedDate = value; }
        }

        public IFormFile? photo { get; set; }
    }
}
