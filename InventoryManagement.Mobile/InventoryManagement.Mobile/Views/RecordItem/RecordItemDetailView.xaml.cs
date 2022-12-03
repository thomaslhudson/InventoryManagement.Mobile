using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class RecordItemDetailView : ContentPage
    {
        public RecordItemDetailView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<RecordItemDetailViewModel>();
        }
    }
}