using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using InventoryManagement.Mobile.Helpers.Converters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class RecordItemDetailViewModel : BaseViewModel, IQueryAttributable
    {
        #region Fields
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private Mode _mode;
        private string _recordItemId;
        private decimal _quantity;
        /* Using the string data type for '_addQuantity' and 'AddQuantity' solves an issue when 
           the back space button is used to remove a single zero from the Entry control, removing a single 
           zero from the Entry control does not fire any event when the bound Property is a decimal */
        private string _addQuantity;
        private string _productId;
        private string _productName;
        private decimal _productUnitPrice;
        private string _productGroupName;
        private byte _recordMonth;
        private short _recordYear;
        private string _recordMonthYear;
        private bool _currentQuantityLabelIsVisible;
        private Color _titleBackgroundColor;
        private Color _titleTextColor;
        private IEnumerable<RecordItem> _recordItems;
        #endregion

        public RecordItemDetailViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            SaveCommand = new Command(async () => await SaveRecordItemAsync(), ValidateSave);
            PropertyChanged += (_, __) => SaveCommand.ChangeCanExecute();

            CancelCommand = new Command(() => Shell.Current.GoToAsync(".."));
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public string RecordItemId
        {
            get => _recordItemId;
            set => SetProperty(ref _recordItemId, value);
        }

        public string RecordId { get; set; }

        public decimal Quantity
        {
            get => _quantity;
            set => SetProperty(ref _quantity, value);
        }

        /* Using the string data type for '_addQuantity' and 'AddQuantity' solves an issue when 
           the back space button is used to remove a single zero from the Entry control, removing a single 
           zero from the Entry control does not fire any event when the bound Property is a decimal */
        public string AddQuantity
        {
            get => _addQuantity;
            set
            {
                SetProperty(ref _addQuantity, value);
            }
        }

        public string ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        public decimal ProductUnitPrice
        {
            get => _productUnitPrice;
            set => SetProperty(ref _productUnitPrice, value);
        }

        public string ProductGroupName
        {
            get => _productGroupName;
            set => SetProperty(ref _productGroupName, value);
        }

        public byte RecordMonth
        {
            get => _recordMonth;
            set => SetProperty(ref _recordMonth, value);
        }

        public short RecordYear
        {
            get => _recordYear;
            set => SetProperty(ref _recordYear, value);
        }

        public string RecordMonthYear
        {
            get => _recordMonthYear;
            set => SetProperty(ref _recordMonthYear, value);
        }

        public IEnumerable<RecordItem> RecordItems
        {
            get => _recordItems;
            set => SetProperty(ref _recordItems, value);
        }

        public Mode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        public double ScreenWidth
        {
            get => Screen.Width;
        }

        public bool CurrentQuantityLabelIsVisible
        {
            get => _currentQuantityLabelIsVisible;
            set => SetProperty(ref _currentQuantityLabelIsVisible, value);
        }

        public Color TitleBackgroundColor
        {
            get => _titleBackgroundColor;
            set => SetProperty(ref _titleBackgroundColor, value);
        }

        public Color TitleTextColor
        {
            get => _titleTextColor;
            set => SetProperty(ref _titleTextColor, value);
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(AddQuantity);
        }

        public async Task GetRecordItemsForProductAsync(string productId)
        {
            try
            {
                var recordItems = await _apiService.GetRecordItemsForProduct(productId);
                if (recordItems != null)
                {
                    RecordItems = recordItems;
                }
            }
            catch (IMHttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Http Exception", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        public async Task GetRecordItemAsync(string reId)
        {
            try
            {
                var recordItem = await _apiService.GetRecordItemAsync(reId);
                if (recordItem != null)
                {
                    RecordItemId = recordItem.Id;
                    Quantity = recordItem.Quantity;
                    ProductId = recordItem.ProductId.ToString();
                    ProductName = recordItem.ProductName;
                    ProductUnitPrice = recordItem.ProductUnitPrice;
                    ProductGroupName = recordItem.ProductGroupName;
                    RecordId = recordItem.RecordId.ToString();
                    RecordMonth = recordItem.RecordMonth;
                    RecordYear = recordItem.RecordYear;
                }
            }
            catch (IMHttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Http Exception", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        public async Task SaveRecordItemAsync()
        {
            decimal? newQuantity = decimal.TryParse(AddQuantity, out decimal addQuantity) ? (Quantity + addQuantity) : (decimal?)null;
            if (newQuantity is null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not parse the decimal {AddQuantity}", "OK");
                return;
            }

            if (newQuantity < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Final Quantity cannot be less than 0", "OK");
                return;
            }

            var newRecordItem = new RecordItem()
            {
                Id = RecordItemId,
                Quantity = (decimal)newQuantity,
                RecordId = RecordId,
                ProductId = ProductId,
            };

            try
            {
                if (Mode == Mode.Create)
                {
                    await _apiService.PostRecordItemAsync(newRecordItem);
                }
                else
                {
                    await _apiService.PutRecordItemAsync(newRecordItem);
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (IMHttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Http Exception", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        public void ConfigureEntryMode()
        {
            switch (Mode)
            {
                case Mode.Create:
                    // Dark Green #69FF47
                    TitleBackgroundColor = Color.FromHex("#69FF47");
                    TitleTextColor = Color.FromHex("#ffffff");
                    CurrentQuantityLabelIsVisible = false;
                    break;
                case Mode.Update:
                default:
                    // Light Blue #96d1ff
                    TitleBackgroundColor = Color.FromHex("#96d1ff");
                    TitleTextColor = Color.FromHex("#ffffff");
                    CurrentQuantityLabelIsVisible = true;
                    break;
            }
        }

        public async void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();

            if (query.TryGetValue("RecordItemId", out var paramRecordItemId))
            {
                if (!string.IsNullOrWhiteSpace(paramRecordItemId))
                {
                    RecordItemId = paramRecordItemId;
                }
            }

            if (query.TryGetValue("RecordId", out var paramRecordId))
            {
                if (!string.IsNullOrWhiteSpace(paramRecordId))
                {
                    RecordId = paramRecordId;
                }
            }

            if (query.TryGetValue("RecordMonth", out var paramRecordMonth))
            {
                if (byte.TryParse(paramRecordMonth, out byte byteRecordMonth))
                {
                    RecordMonth = byteRecordMonth;
                }
            }

            if (query.TryGetValue("RecordYear", out var paramRecordYear))
            {
                if (short.TryParse(paramRecordYear, out short shortRecordYear))
                {
                    RecordYear = shortRecordYear;
                }
            }

            if (query.TryGetValue("ProductId", out var paramProductId))
            {
                if (!string.IsNullOrWhiteSpace(paramProductId))
                {
                    ProductId = paramProductId;
                }
            }

            if (query.TryGetValue("ProductName", out var paramProductName))
            {
                if (!string.IsNullOrWhiteSpace(paramProductName))
                {
                    ProductName = Uri.UnescapeDataString(paramProductName);
                }
            }

            if (query.TryGetValue("Mode", out var paramMode))
            {
                if (Enum.TryParse(paramMode, true, out Mode enumMode))
                {
                    Mode = enumMode;
                }
            }

            if (query.TryGetValue("ProductUnitPrice", out var paramProductUnitPrice))
            {
                if (decimal.TryParse(paramProductUnitPrice, out decimal decimalProductUnitPrice))
                {
                    ProductUnitPrice = decimalProductUnitPrice;
                }
            }

            if (query.TryGetValue("ProductGroupName", out var paramProductGroupName))
            {
                if (!string.IsNullOrWhiteSpace(paramProductGroupName))
                {
                    ProductGroupName = Uri.UnescapeDataString(paramProductGroupName);
                }
            }

            if (Mode == Mode.Update && !string.IsNullOrWhiteSpace(RecordItemId))
            {
                await GetRecordItemAsync(RecordItemId);
            }

            RecordMonthYear = $"{Dates.GetMonthName(RecordMonth)} / {RecordYear}";

            await GetRecordItemsForProductAsync(ProductId);

            ConfigureEntryMode();
        }
    }
}