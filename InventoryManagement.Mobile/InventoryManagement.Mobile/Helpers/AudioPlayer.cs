using Plugin.SimpleAudioPlayer;
using Xamarin.Forms;

namespace InventoryManagement.Mobile
{
    public class AudioPlayer
    {
        private readonly ISimpleAudioPlayer Player;

        public AudioPlayer()
        {
            Player = CrossSimpleAudioPlayer.Current;
            Player.Load("EffectTick.wav");
        }

        public void Play()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Player.Play();
            });
        }
    }
}
