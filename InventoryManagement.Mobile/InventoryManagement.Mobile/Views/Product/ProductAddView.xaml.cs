using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class ProductAddView : ContentPage
    {
        public ProductAddView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<ProductAddViewModel>();
        }
    }
}