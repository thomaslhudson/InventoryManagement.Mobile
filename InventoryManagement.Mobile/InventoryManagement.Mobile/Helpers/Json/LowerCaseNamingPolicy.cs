using System.Text.Json;

namespace InventoryManagement.Mobile.Helpers
{
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) => name.ToLower();
    }
}
