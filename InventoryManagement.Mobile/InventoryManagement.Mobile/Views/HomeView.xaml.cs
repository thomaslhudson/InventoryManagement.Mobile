using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class HomeView : ContentPage
    {
        public HomeView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<HomeViewModel>();

            // https://blog.verslu.is/xamarin/xamarin-forms-xamarin/zxing-android-skipping-frames/
            Scanner.Options.DelayBetweenAnalyzingFrames = 5; // 5 milliseconds, and lower than the default - weird that it's better
            Scanner.Options.DelayBetweenContinuousScans = 2000; //2000
            Scanner.Options.InitialDelayBeforeAnalyzingFrames = 300;
            Scanner.Options.TryHarder = false;
            Scanner.Options.TryInverted = false;
            Scanner.Options.AutoRotate = false;
            Scanner.Options.UseFrontCameraIfAvailable = false;
            Scanner.Options.PossibleFormats = new[] { ZXing.BarcodeFormat.UPC_A };
        }
    }
}