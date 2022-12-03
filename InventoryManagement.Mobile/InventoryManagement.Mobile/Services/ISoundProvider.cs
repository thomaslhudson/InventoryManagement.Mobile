using System.Threading.Tasks;

namespace InventoryManagement.Mobile.Services
{
    public interface ISoundProvider
    {
        Task PlaySoundAsync(string filename);
    }
}
