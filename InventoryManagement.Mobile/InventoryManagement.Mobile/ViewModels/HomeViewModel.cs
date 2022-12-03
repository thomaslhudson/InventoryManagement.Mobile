using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using InventoryManagement.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using ZXing;
using ZXing.Net.Mobile.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class HomeViewModel : BaseViewModel, IQueryAttributable
    {
        #region Global Page Fields
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private bool _constructed;
        private bool _scanTabVisible = true;
        private bool _enterTabVisible;
        private bool _selectTabVisible;
        private Mode _mode;
        private string _recordId;
        private byte _month;
        private short _year;
        private string _monthYear;
        #endregion

        #region Tab 1 (Scan) Fields
        private bool _scanBarcodeButtonEnabled = true;
        private bool _isAnalyzing;
        private bool _isScanning;
        private ObservableCollection<Record> _records;
        private Record _selectedRecord;

        private Result _scanResult;
        #endregion

        #region Tab 2 (Enter UPC) Fields
        private string _entryUpc;
        #endregion

        #region Tab 3 (Select Product) Fields
        private ObservableCollection<Product> _productsFiltered = new ObservableCollection<Product>();
        private List<Product> _productsAll = new List<Product>();
        private string _searchProductText;
        #endregion

        public HomeViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            _ = PopulateRecordsAsync();
            _ = PopulateProductListAsync();

            SelectTabCommand = new Command<string>((tabId) => SelectTab(tabId));

            #region Scan Tab
            ScanBarcodeCommand = new Command(() => ScanBarcode());
            ProcessScanResultCommand = new Command(async () => await ProcessScanResultAsync());
            #endregion

            #region Enter Upc Tab
            SubmitUpcCommand = new Command<string>(async (upc) => await ProcessUpcAsync(upc), ValidateSubmitUpc);
            PropertyChanged += (_, __) => SubmitUpcCommand.ChangeCanExecute();
            #endregion

            #region Select Product Tab
            SelectProductCommand = new Command<string>(async (productId) => { await SelectProduct(productId); });
            #endregion
        }

        #region Tab 1 (Scan)  Commands / Properties / Methods

        public Command ScanBarcodeCommand { get; set; }
        public Command ProcessScanResultCommand { get; set; }

        public ZXingScannerPage ZxingScanner
        {
            get 
            {
                var scannerPage = new ZXingScannerPage();
                scannerPage.HeightRequest = ZXingHeight;
                scannerPage.WidthRequest = ZXingWidth;
                return new ZXingScannerPage();
            }
    }

        public bool ScanBarcodeButtonEnabled
        {
            get => _scanBarcodeButtonEnabled;
            set => SetProperty(ref _scanBarcodeButtonEnabled, value);
        }

        public bool IsAnalyzing
        {
            get => _isAnalyzing;
            set => SetProperty(ref _isAnalyzing, value);
        }

        public bool IsScanning
        {
            get => _isScanning;
            set => SetProperty(ref _isScanning, value);
        }

        public double ZXingWidth
        {
            // 36.4583% of the Screen.Width
            // Originally 175 in XAML
            get => .364583 * Screen.Width;
        }

        public double ZXingHeight
        {
            // 29.605% of the Screen.Height
            // Originally 300 in XAML
            get => .29605 * Screen.Height;
        }

        public void ScanBarcode()
        {
            IsAnalyzing = true;
            IsScanning = true;

            ScanBarcodeButtonEnabled = !ScanBarcodeButtonEnabled;
        }

        public Result ScanResult
        {
            get => _scanResult;
            set => SetProperty(ref _scanResult, value);
        }

        public async Task ProcessScanResultAsync()
        {
            IsAnalyzing = false;
            IsScanning = false;

            ScanBarcodeButtonEnabled = !ScanBarcodeButtonEnabled;

            var productUpc = ScanResult.Text;

            await ProcessUpcAsync(productUpc);
        }

        #endregion

        #region Tab 2 (Enter UPC) Commands / Properties / Methods
        public Command SubmitUpcCommand { get; set; }

        public string EntryUpc
        {
            get => _entryUpc;
            set => SetProperty(ref _entryUpc, value);
        }

        private bool ValidateSubmitUpc(string entryUpc)
        {
            return !string.IsNullOrWhiteSpace(entryUpc)
                && entryUpc.Length == 12;
        }

        #endregion

        #region Tab 3 (Select Product) Commands / Properties / Methods

        public Command SelectProductCommand { get; set; }
        public ObservableCollection<Product> ProductsFiltered
        {
            get => _productsFiltered;
            set => SetProperty(ref _productsFiltered, value);
        }

        public string SearchProductText
        {
            get => _searchProductText;
            set
            {
                SetProperty(ref _searchProductText, value);
                Search(value);
            }
        }

        public async Task PopulateProductListAsync()
        {
            _productsAll.Clear();
            ProductsFiltered.Clear();

            try
            {
                var products = await _apiService.GetProductsAsync();
                if (products != null && products.Any())
                {
                    _productsAll = products.ToList();
                    _productsAll.ForEach(p => ProductsFiltered.Add(p));
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No products were retrieved", "OK");
                    return;
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

        private async Task SelectProduct(string productId)
        {
            try
            {
                if (!string.IsNullOrEmpty(productId))
                {
                    var product = await _apiService.GetProductByIdAsync(productId);
                    var recordItem = await _apiService.GetRecordItemAsync(RecordId, productId);

                    Mode mode = recordItem is null ? Mode.Create : Mode.Update;
                    var recordItemId = recordItem is null ? Guid.Empty.ToString() : recordItem.Id.ToString();

                    await Shell.Current.GoToAsync($"{nameof(RecordItemDetailView)}?" +
                        $"{nameof(RecordItemDetailViewModel.RecordItemId)}={recordItemId}" +
                        $"&{nameof(RecordItemDetailViewModel.RecordId)}={RecordId}" +
                        $"&{nameof(RecordItemDetailViewModel.RecordMonth)}={Month}" +
                        $"&{nameof(RecordItemDetailViewModel.RecordYear)}={Year}" +
                        $"&{nameof(RecordItemDetailViewModel.Mode)}={mode}" +
                        $"&{nameof(RecordItemDetailViewModel.ProductId)}={product.Id}" +
                        $"&{nameof(RecordItemDetailViewModel.ProductName)}={product.Name}" +
                        $"&{nameof(RecordItemDetailViewModel.ProductUnitPrice)}={product.UnitPrice}" +
                        $"&{nameof(RecordItemDetailViewModel.ProductGroupName)}={product.Group.Name}");
                    ;
                }
            }
            catch (IMHttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Http Exception", $"Status Code: {ex.HttpStatusCode}\r\nMessage: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", $"Error:\r\nMessage: {ex.Message}", "OK");
            }
        }

        public void Search(string searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var products = _productsAll.Where(p => p.Name.ToLower().Contains(searchText.ToLower()));
                _productsFiltered.Clear();
                products.ForEach(p => _productsFiltered.Add(p));
            }
            else
            {
                _productsFiltered.Clear();
                _productsAll.ForEach(p => _productsFiltered.Add(p));
            }

            //OnPropertyChanged("ProductsFiltered");
        }

        #endregion

        #region Global Properties / Methods

        public Command SelectTabCommand { get; }

        public bool ScanTabVisible
        {
            get => _scanTabVisible;
            set => SetProperty(ref _scanTabVisible, value);
        }

        public bool EnterTabVisible
        {
            get => _enterTabVisible;
            set => SetProperty(ref _enterTabVisible, value);
        }

        public bool SelectTabVisible
        {
            get => _selectTabVisible;
            set => SetProperty(ref _selectTabVisible, value);
        }

        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        public Record SelectedRecord
        {
            get => _selectedRecord;
            set
            {
                SetProperty(ref _selectedRecord, value);

                if (value != null)
                {
                    RecordId = value.Id;
                    Month = value.Month;
                    Year = value.Year;
                }
            }
        }

        public Mode Mode
        {
            get => _mode;
            set => SetProperty(ref _mode, value);
        }

        public string RecordId
        {
            get => _recordId;
            set => SetProperty(ref _recordId, value);
        }

        public byte Month
        {
            get => _month;
            set => SetProperty(ref _month, value);
        }

        public short Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        public string MonthYear
        {
            get => _monthYear;
            set => SetProperty(ref _monthYear, value);
        }

        public double ScreenWidth
        {
            get => Screen.Width;
        }

        public double ScreenHeight
        {
            get => Screen.Height;
        }

        public void SelectTab(string tabId)
        {
            switch (tabId)
            {
                case "1":
                    EnterTabVisible = false;
                    SelectTabVisible = false;
                    ScanTabVisible = true;
                    break;
                case "2":
                    ScanTabVisible = false;
                    SelectTabVisible = false;
                    EnterTabVisible = true;
                    break;
                case "3":
                    ScanTabVisible = false;
                    EnterTabVisible = false;
                    SelectTabVisible = true;
                    break;
            }
        }

        public async Task PopulateRecordsAsync()
        {
            try
            {
                Records = Records ?? new ObservableCollection<Record>();

                Records.Clear();

                var records = await _apiService.GetRecordsAsync();
                if (records.Any())
                {
                    records.ForEach(r => Records.Add(r));

                    SelectedRecord = string.IsNullOrWhiteSpace(RecordId) ?
                        Records[0] : Records.FirstOrDefault(i => i.Id == RecordId);

                    MonthYear = SelectedRecord.MonthYear;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ProcessUpcAsync(string upc)
        {
            try
            {
                var product = await _apiService.GetProductByUpcAsync(upc);
                if (!(product is null))
                {
                    var recordItem = await _apiService.GetRecordItemAsync(RecordId, product.Id);

                    Mode mode = recordItem is null ? Mode.Create : Mode.Update;
                    var recordItemId = recordItem is null ? Guid.Empty.ToString() : recordItem.Id.ToString();

                    // BeginInvokeOnMainThread is being used here as a work around regarding
                    // an issue that sometimes crops up with ZXing (the scanner package)
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Shell.Current.GoToAsync($"{nameof(RecordItemDetailView)}?" +
                        $"{nameof(RecordItemDetailViewModel.RecordItemId)}={recordItemId}" +
                            $"&{nameof(RecordItemDetailViewModel.RecordId)}={RecordId}" +
                            $"&{nameof(RecordItemDetailViewModel.RecordMonth)}={Month}" +
                            $"&{nameof(RecordItemDetailViewModel.RecordYear)}={Year}" +
                            $"&{nameof(RecordItemDetailViewModel.Mode)}={mode}" +
                            $"&{nameof(RecordItemDetailViewModel.ProductId)}={product.Id}" +
                            $"&{nameof(RecordItemDetailViewModel.ProductName)}={product.Name}");
                    });
                }
                else
                {
                    // BeginInvokeOnMainThread is being used here as a work around regarding
                    // an issue that sometimes crops up with ZXing (the scanner package)
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Shell.Current.GoToAsync($"{nameof(ProductAddView)}?" +
                                                $"{nameof(ProductAddViewModel.Upc)}={upc}");

                    });
                }
            }
            catch (IMHttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Http Exception", $"Status Code: {ex.HttpStatusCode}\r\nMessage: {ex.Message}", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", $"Error:\r\nMessage: {ex.Message}", "OK");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();

            if (query.TryGetValue("Mode", out var paramMode))
            {
                if (Enum.TryParse(paramMode, true, out Mode mode))
                {
                    Mode = mode;
                }
            }

            if (query.TryGetValue("RecordId", out var paramId))
            {
                if (Guid.TryParse(paramId, out var guidRecordItemId))
                {
                    if (guidRecordItemId != Guid.Empty)
                    {
                        RecordId = paramId;
                    }
                }
            }

            if (query.TryGetValue("Month", out var paramMonth))
            {
                if (Byte.TryParse(paramMonth, out var byteMonth))
                {
                    Month = byteMonth;
                }
            }

            if (query.TryGetValue("Year", out var paramYear))
            {
                if (short.TryParse(paramYear, out var shortYear))
                {
                    Year = shortYear;
                }
            }

            if (!_constructed)
            {
                _constructed = true;
                return;
            }

            SearchProductText = "";

            _ = PopulateRecordsAsync();
            _ = PopulateProductListAsync();
        }
        #endregion
    }
}
