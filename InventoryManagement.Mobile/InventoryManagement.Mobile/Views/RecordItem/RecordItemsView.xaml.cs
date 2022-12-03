using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class RecordItemsView : ContentPage
    {
        public RecordItemsView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<RecordItemsViewModel>();
        }
    }
}