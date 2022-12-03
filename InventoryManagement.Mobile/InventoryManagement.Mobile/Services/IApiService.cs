using InventoryManagement.Mobile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.Mobile.Services
{
    public interface IApiService
    {
        #region Record
        Task<Record> GetRecordAsync(string recordId);
        Task<IEnumerable<Record>> GetRecordsAsync();
        Task PostRecordAsync(Record record);
        Task PutRecordAsync(Record record);
        Task DeleteRecordAsync(Record record);
        #endregion

        #region RecordItem
        Task<IEnumerable<RecordItem>> GetRecordItemsAsync(string recordId);
        Task<RecordItem> GetRecordItemAsync(string recordId);
        Task<RecordItem> GetRecordItemAsync(string recordId, string productId);
        Task<IEnumerable<RecordItem>> GetRecordItemsForProduct(string productId);
        Task PostRecordItemAsync(RecordItem recordItem);
        Task PutRecordItemAsync(RecordItem recordItem);
        #endregion

        #region Product
        Task<Product> GetProductByIdAsync(string id);
        Task<Product> GetProductByNameAsync(string name);
        Task<Product> GetProductByUpcAsync(string upc);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task PostProductAsync(Product product);
        Task PutProductAsync(Product product);
        #endregion

        #region Product Group
        Task<Group> GetGroupAsync(string id);
        Task<IEnumerable<Group>> GetGroupsAsync();
        Task PostGroupAsync(Group group);
        Task PutGroupAsync(Group group);
        #endregion

        #region Report
        Task<IEnumerable<GroupTotalsByRecord>> GetGroupTotalsByRecordAsync(string recordId);
        #endregion
    }
}
