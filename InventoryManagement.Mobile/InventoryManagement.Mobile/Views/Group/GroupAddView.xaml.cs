using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class GroupAddView : ContentPage
    {
        public GroupAddView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<GroupAddViewModel>();
        }
    }
}