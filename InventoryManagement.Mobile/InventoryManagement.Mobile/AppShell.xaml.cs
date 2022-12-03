using InventoryManagement.Mobile.Views;
using InventoryManagement.Mobile.Views.Report;
using Xamarin.Forms;

namespace InventoryManagement.Mobile
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // HomeView
            // HomeView added via AppShell  Flyout ShellContent
            //Routing.RegisterRoute(nameof(HomeView), typeof(HomeView));

            // Record
            // RecordsView added via AppShell Flyout ShellContent
            //Routing.RegisterRoute(nameof(RecordsView), typeof(RecordsView));
            Routing.RegisterRoute(nameof(RecordAddView), typeof(RecordAddView));

            // RecordItems
            Routing.RegisterRoute(nameof(RecordItemDetailView), typeof(RecordItemDetailView));
            Routing.RegisterRoute(nameof(RecordItemsView), typeof(RecordItemsView));

            // Product
            // ProductsView added via AppShell Flyout ShellContent
            //Routing.RegisterRoute(nameof(ProductsView), typeof(ProductsView));
            Routing.RegisterRoute(nameof(ProductAddView), typeof(ProductAddView));
            Routing.RegisterRoute(nameof(ProductDetailView), typeof(ProductDetailView));

            // Group
            // GroupsView added via AppShell Flyout ShellContent
            //Routing.RegisterRoute(nameof(GroupsView), typeof(GroupsView));
            Routing.RegisterRoute(nameof(GroupAddView), typeof(GroupAddView));
            Routing.RegisterRoute(nameof(GroupDetailView), typeof(GroupDetailView));

            // Report
            // ReportView added via AppShell Flyout ShellContent
            //Routing.RegisterRoute(nameof(ReportView), typeof(ReportView));
            Routing.RegisterRoute(nameof(GroupTotalsByRecordView), typeof(GroupTotalsByRecordView));
            Routing.RegisterRoute(nameof(GroupTotalsByRecordSubsetView), typeof(GroupTotalsByRecordSubsetView));

            // SettingsView
            Routing.RegisterRoute(nameof(SettingsView), typeof(SettingsView));

            // The following line will request Android to raise the screen contents when the 
            // keyboard is displayed. It does, but messes up some of the formatting so it's not being used
            //App.Current.On<Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
        }
    }
}