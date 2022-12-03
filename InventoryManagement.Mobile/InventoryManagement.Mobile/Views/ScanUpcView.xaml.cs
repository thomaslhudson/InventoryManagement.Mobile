using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class ScanUpcView : ContentPage
    {
        public ScanUpcView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<ScanUpcViewModel>();

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

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    //_scanUpcViewModel.PlayerLoad();
        //}
    }
}