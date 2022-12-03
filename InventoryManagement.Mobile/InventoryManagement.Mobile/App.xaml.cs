using InventoryManagement.Mobile.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventoryManagement.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Startup.ConfigureServices();

            MainPage = new AppShell();

            if (string.IsNullOrWhiteSpace(Preferences.Get(PreferenceKey.ApiBaseUri.ToString(), string.Empty)))
            {
                Shell.Current.GoToAsync($"//{nameof(SettingsView)}");
            }
        }

        //protected override void OnStart()
        //{
        //}

        //protected override void OnSleep()
        //{
        //}

        //protected override void OnResume()
        //{
        //}
    }
}
