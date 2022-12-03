using Android.Media;
using InventoryManagement.Mobile.Droid;
using InventoryManagement.Mobile.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidSoundProvider))]
namespace InventoryManagement.Mobile.Droid
{
    public class AndroidSoundProvider : ISoundProvider
    {
        public Task PlaySoundAsync(string filename)
        {
            // Create media player
            var player = new MediaPlayer();

            // Create task completion source to support async/await
            var tcs = new TaskCompletionSource<bool>();

            // Open the resource
            // deprecated code: var fd = Xamarin.Forms.Forms.Context.Assets.OpenFd(filename);
            var fd = Android.App.Application.Context.Assets.OpenFd(filename);

            // Hook up some events
            player.Prepared += (s, e) => { player.Start(); };

            player.Completion += (sender, e) => { tcs.SetResult(true); };

            // Start playing
            player.SetDataSource(fd.FileDescriptor);
            player.Prepare();

            return tcs.Task;
        }
    }
}