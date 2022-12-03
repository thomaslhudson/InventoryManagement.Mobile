using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventoryManagement.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportView : ContentPage
    {
        public ReportView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<ReportsViewModel>();
        }
    }
}