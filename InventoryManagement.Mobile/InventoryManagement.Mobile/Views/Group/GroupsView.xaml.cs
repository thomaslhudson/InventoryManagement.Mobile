using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class GroupsView : ContentPage
    {
        public GroupsView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<GroupsViewModel>();
        }
    }
}