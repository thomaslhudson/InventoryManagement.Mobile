using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class ProductDetailView : ContentPage
    {
        public ProductDetailView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<ProductDetailViewModel>();
        }
    }
}