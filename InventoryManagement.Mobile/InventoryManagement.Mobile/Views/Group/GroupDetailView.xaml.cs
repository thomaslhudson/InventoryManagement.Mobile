using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class GroupDetailView : ContentPage
    {
        public GroupDetailView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<GroupDetailViewModel>();
        }
    }
}