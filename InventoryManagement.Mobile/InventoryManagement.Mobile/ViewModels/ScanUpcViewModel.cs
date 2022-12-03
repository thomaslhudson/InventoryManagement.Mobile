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

namespace InventoryManagement.Mobile.ViewModels
{
    public class ScanUpcViewModel : BaseViewModel, IQueryAttributable
    {
        #region Fields
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private bool _constructed;
        private Mode _mode;
        private bool _recordPickerEnabled = true;
        private string _recordId;
        private byte _month;
        private short _year;
        private bool _isAnalyzing;
        private bool _isScanning;
        private Result _scanResult;
        private string _entryUpc;
        private bool _enterUpcVisible;
        private string _productName;
        //private decimal _amount;
        private bool _scannerViewIsVisible;
        private bool _scanBarcodeButtonEnabled = true;
        private bool _ManualInputIsVisible;
        private string _selectedManualInputType;
        private Color _enterManuallyButtonBGColor;
        private Color _enterManuallyButtonTextColor;
        private Color _showScannerButtonBGColor;
        private Color _showScannerButtonTextColor;
        private Record _selectedRecord;
        private string _searchProductText;
        private bool _productsListVisible;
        private ObservableCollection<Record> _records;
        private ObservableCollection<Product> _products = new ObservableCollection<Product>();
        #endregion

        public ScanUpcViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            _ = PopulateRecordsAsync();
            _ = PopulateProductListAsync();

            ScanBarcodeCommand = new Command(() => ScanBarcode());
            ProcessScanResultCommand = new Command(async () => await ProcessScanResultAsync());

            //EnterManuallyCommand = ShowScannerCommand = new Command<string>((tab) => { ToggleScannerAndManualView(tab); });
            ToggleTabsCommand = new Command<string>((tab) => { ToggleTabs(tab); });

            SelectProductCommand = new Command<Product>(async (product) => { await SelectProduct(product); });

            SubmitUpcCommand = new Command<string>(async (entryUpc) => { await ProcessUpcAsync(entryUpc); }, ValidateSubmitUpc);
            PropertyChanged += (_, __) => SubmitUpcCommand.ChangeCanExecute();

            ScannerViewIsVisible = true;
            ShowScannerButtonBGColor = Color.White;
            ShowScannerButtonTextColor = Color.FromHex("#2196F3");

            EnterManuallyButtonBGColor = Color.FromHex("#2196F3");
            EnterManuallyButtonTextColor = Color.White;
        }

        public Command ScanBarcodeCommand { get; set; }
        public Command ProcessScanResultCommand { get; set; }
        public Command ToggleTabsCommand { get; set; }
        //public Command EnterManuallyCommand { get; set; }
        //public Command ShowScannerCommand { get; set; }
        public Command SelectProductCommand { get; set; }
        public Command SubmitUpcCommand { get; set; }

        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
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

        public Result ScanResult
        {
            get => _scanResult;
            set => SetProperty(ref _scanResult, value);
        }

        public string EntryUpc
        {
            get => _entryUpc;
            set => SetProperty(ref _entryUpc, value);
        }

        public bool RecordPickerEnabled
        {
            get => _recordPickerEnabled;
            set => SetProperty(ref _recordPickerEnabled, value);
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

        public string ProductName
        {
            get => _productName;
            set => SetProperty(ref _productName, value);
        }

        //public decimal Amount
        //{
        //    get => _amount;
        //    set => SetProperty(ref _amount, value);
        //}

        public bool ScannerViewIsVisible
        {
            get => _scannerViewIsVisible;
            set => SetProperty(ref _scannerViewIsVisible, value);
        }

        public bool ManualInputIsVisible
        {
            get => _ManualInputIsVisible;
            set => SetProperty(ref _ManualInputIsVisible, value);
        }

        public bool ScanBarcodeButtonEnabled
        {
            get => _scanBarcodeButtonEnabled;
            set => SetProperty(ref _scanBarcodeButtonEnabled, value);
        }

        public string SelectedManualInputType
        {
            get => _selectedManualInputType;
            set
            {
                SetProperty(ref _selectedManualInputType, value);
                if (!(value is null))
                {
                    if (value == "ByUpc")
                    {
                        ProductsListVisible = false;
                        EnterUpcVisible = true;
                        return;
                    }

                    if (value == "ByProduct")
                    {
                        EnterUpcVisible = false;
                        ProductsListVisible = true;
                        return;
                    }
                }
            }
        }

        public bool ProductsListVisible
        {
            get => _productsListVisible;
            set => SetProperty(ref _productsListVisible, value);
        }

        public bool EnterUpcVisible
        {
            get => _enterUpcVisible;
            set => SetProperty(ref _enterUpcVisible, value);
        }

        public Color EnterManuallyButtonBGColor
        {
            get => _enterManuallyButtonBGColor;
            set => SetProperty(ref _enterManuallyButtonBGColor, value);
        }

        public Color EnterManuallyButtonTextColor
        {
            get => _enterManuallyButtonTextColor;
            set => SetProperty(ref _enterManuallyButtonTextColor, value);
        }

        public Color ShowScannerButtonBGColor
        {
            get => _showScannerButtonBGColor;
            set => SetProperty(ref _showScannerButtonBGColor, value);
        }

        public Color ShowScannerButtonTextColor
        {
            get => _showScannerButtonTextColor;
            set => SetProperty(ref _showScannerButtonTextColor, value);
        }

        public string SearchProductText
        {
            get => _searchProductText;
            set => SetProperty(ref _searchProductText, value);
        }

        public void ToggleTabs(string tab)
        {
            if (tab == "ScanUPC" && ScannerViewIsVisible || tab == "ManualInput" && ManualInputIsVisible)
            {
                return;
            }

            var scanner = ScannerViewIsVisible = !ScannerViewIsVisible;
            var manual = ManualInputIsVisible = !ScannerViewIsVisible;

            ShowScannerButtonBGColor = scanner ? Color.White : Color.FromHex("#2196F3");
            ShowScannerButtonTextColor = scanner ? Color.FromHex("#2196F3") : Color.White;
            EnterManuallyButtonBGColor = manual ? Color.White : Color.FromHex("#2196F3");
            EnterManuallyButtonTextColor = manual ? Color.FromHex("#2196F3") : Color.White;
        }

        private async Task SelectProduct(Product product)
        {
            try
            {
                if (!(product is null))
                {
                    var recordItem = await _apiService.GetRecordItemAsync(RecordId, product.Id);

                    Mode mode = recordItem is null ? Mode.Create : Mode.Update;
                    var recordItemId = recordItem is null ? Guid.Empty.ToString() : recordItem.Id.ToString();

                    //// BeginInvokeOnMainThread is being used here as a work around regarding
                    //// an issue that sometimes crops up with ZXing (the scanner package)
                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                        await Shell.Current.GoToAsync($"{nameof(RecordItemDetailView)}?" +
                            $"{nameof(RecordItemDetailViewModel.RecordItemId)}={recordItemId}" +
                            $"&{nameof(RecordItemDetailViewModel.RecordId)}={RecordId}" +
                            $"&{nameof(RecordItemDetailViewModel.RecordMonth)}={Month}" +
                            $"&{nameof(RecordItemDetailViewModel.RecordYear)}={Year}" +
                            $"&{nameof(RecordItemDetailViewModel.Mode)}={mode}" +
                            $"&{nameof(RecordItemDetailViewModel.ProductId)}={product.Id}" +
                            $"&{nameof(RecordItemDetailViewModel.ProductName)}={product.Name}");
                    //});
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

        private bool ValidateSubmitUpc(string entryUpc)
        {
            if (ManualInputIsVisible)
            {
                return SelectedRecord != null
                    && !string.IsNullOrWhiteSpace(entryUpc)
                    && entryUpc.Length == 12;
            }

            return false;
        }

        public async Task ProcessScanResultAsync()
        {
            IsAnalyzing = false;
            IsScanning = false;

            ScanBarcodeButtonEnabled = !ScanBarcodeButtonEnabled;

            var productUpc = ScanResult.Text;

            await ProcessUpcAsync(productUpc);
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

                    if (_mode == Mode.Create)
                    {
                        RecordPickerEnabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task PopulateProductListAsync()
        {
            _products.Clear();

            try
            {
                var products = await _apiService.GetProductsAsync();
                if (products != null && products.Any())
                {
                    products.ForEach(p => _products.Add(p));
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

        public void ScanBarcode()
        {
            IsAnalyzing = true;
            IsScanning = true;

            ScanBarcodeButtonEnabled = !ScanBarcodeButtonEnabled;
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

            _ = PopulateRecordsAsync();
            _ = PopulateProductListAsync();
        }
    }
}