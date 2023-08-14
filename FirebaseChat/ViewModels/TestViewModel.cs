using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirebaseChat.ViewModels
{
    [QueryProperty(nameof(ProductPrice), "ProductPrice")]
    internal class TestViewModel : ViewModelBase
    {
        public TestViewModel() 
        {
            
        }

        private double _productPrice;
        public double ProductPrice
        {
            get => _productPrice;
            set => SetProperty(ref _productPrice, value);
        }
    }
}
