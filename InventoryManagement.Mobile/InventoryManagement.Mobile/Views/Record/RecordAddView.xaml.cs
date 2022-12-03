using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class RecordAddView : ContentPage
    {
        public RecordAddView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<RecordAddViewModel>();
        }
    }
}