using InventoryManagement.Mobile.ViewModels;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Views
{
    public partial class RecordsView : ContentPage
    {
        //private readonly RecordsViewModel _recordsViewModel;
        //private readonly ISimpleAudioPlayer Player = CrossSimpleAudioPlayer.Current;

        public RecordsView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<RecordsViewModel>();
            //BindingContext = _recordsViewModel = Startup.Resolve<RecordsViewModel>();

            //Player = CrossSimpleAudioPlayer.Current;
            //Player.Load("EffectTick.wav");
        }

        //private void Tgr_OnTap(object sender, System.EventArgs e)
        //{
        //    Player.Play();
        //}
    }
}