using System.Threading.Tasks;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.Services
{
    public class SoundService : ISoundProvider
    {
        private readonly ISoundProvider _soundProvider;

        public SoundService()
        {
            _soundProvider = DependencyService.Get<ISoundProvider>();
        }

        public async Task PlaySoundAsync(string filename)
        {
            await _soundProvider.PlaySoundAsync(filename);
        }
    }
}
