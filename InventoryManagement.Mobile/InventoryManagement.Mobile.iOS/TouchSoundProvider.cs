using AVFoundation;
using Foundation;
using InventoryManagement.Mobile.iOS;
using InventoryManagement.Mobile.Services;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(TouchSoundProvider))]
namespace InventoryManagement.Mobile.iOS
{
    public class TouchSoundProvider : NSObject, ISoundProvider
    {
        private AVAudioPlayer _player;

        public Task PlaySoundAsync(string filename)
        {
            var tcs = new TaskCompletionSource<bool>();

            string path = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(filename),
                Path.GetExtension(filename));

            var url = NSUrl.FromString(path);
            _player = AVAudioPlayer.FromUrl(url);

            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) =>
            {
                _player = null;
                tcs.SetResult(true);
            };

            _player.Play();

            return tcs.Task;
        }
    }
}