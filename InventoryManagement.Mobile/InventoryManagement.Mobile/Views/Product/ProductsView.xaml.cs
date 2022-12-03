using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class ProductsView : ContentPage
    {
        public ProductsView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<ProductsViewModel>();
        }
    }
}