using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using InventoryManagement.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class RecordItemsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private string _pageTitle = "Inventory Items";
        private string _searchText = string.Empty;
        private string _recordId;
        private byte _recordMonth;
        private short _recordYear;
        private bool _searchBarVisible;
        private ObservableCollection<RecordItem> _recordItemsFiltered = new ObservableCollection<RecordItem>();
        private List<RecordItem> _recordItems = new List<RecordItem>();

        public RecordItemsViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            DeleteRecordItemCommand = new Command<RecordItem>(async re => await DeleteRecordItemAsync(re));
            AddRecordItemCommand = new Command(async () => await AddRecordItemAsync());
            SelectRecordItemCommand = new Command<string>((recordItemId) => { SelectRecordItem(recordItemId); });
        }

        public ICommand DeleteRecordItemCommand { get; }
        public ICommand AddRecordItemCommand { get; }
        public ICommand SelectRecordItemCommand { get; set; }

        public ObservableCollection<RecordItem> RecordItemsFiltered
        {
            get => _recordItemsFiltered;
            set => SetProperty(ref _recordItemsFiltered, value);
        }

        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                Search(value);
            }
        }

        public string RecordId
        {
            get => _recordId;
            set => SetProperty(ref _recordId, value);
        }

        public byte Month
        {
            get => _recordMonth;
            set => SetProperty(ref _recordMonth, value);
        }

        public short Year
        {
            get => _recordYear;
            set => SetProperty(ref _recordYear, value);
        }

        public bool SearchBarVisible
        {
            get => _searchBarVisible;
            set => SetProperty(ref _searchBarVisible, value);
        }

        public void Search(string searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var recordItems = _recordItems.Where(re => re.ProductName.ToLower().Contains(searchText.ToLower())).ToList();
                RecordItemsFiltered.Clear();
                recordItems.ForEach(re => RecordItemsFiltered.Add(re));
                OnPropertyChanged("RecordItemsFiltered");
            }
            else
            {
                RecordItemsFiltered.Clear();
                _recordItems.ForEach(rep => RecordItemsFiltered.Add(rep));
                OnPropertyChanged("RecordItemsFiltered");
            }
        }

        public async Task PopulateRecordItemsAsync(string recordId)
        {
            if (string.IsNullOrWhiteSpace(recordId) || !Guid.TryParse(recordId, out var _))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Missing or malformed Record Guid\r\n\r\nCould not retrieve any Items", "OK");
                await Shell.Current.GoToAsync("..");
                return;
            }

            RecordItemsFiltered.Clear();
            _recordItems.Clear();

            try
            {
                var recordItems = await _apiService.GetRecordItemsAsync(recordId);

                SearchBarVisible = recordItems.Any();

                foreach (var recordItem in recordItems)
                {
                    RecordItemsFiltered.Add(recordItem);
                    _recordItems.Add(recordItem);
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

        public async Task DeleteRecordItemAsync(RecordItem re)
        {
            await Application.Current.MainPage.DisplayAlert("Information", "Functionality not implemented", "OK");
        }

        public async Task AddRecordItemAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(HomeView)}?" +
                $"{nameof(HomeViewModel.RecordId)}={RecordId}" +
                $"&{nameof(HomeViewModel.Month)}={Month}" +
                $"&{nameof(HomeViewModel.Year)}={Year}" +
                $"&{nameof(HomeViewModel.Mode)}={Mode.Create}");
        }

        private async void SelectRecordItem(string recordItemId)
        {
            if (!string.IsNullOrEmpty(recordItemId))
            {
                await Shell.Current.GoToAsync($"{nameof(RecordItemDetailView)}?" +
                    $"{nameof(RecordItemDetailViewModel.RecordItemId)}={recordItemId}" +
                    $"&{nameof(RecordItemDetailViewModel.Mode)}={Mode.Update}");
            }
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();

            if (query.TryGetValue("RecordId", out var paramRecordId))
            {
                RecordId = paramRecordId;
            }

            if (query.TryGetValue("Month", out var paramMonth))
            {
                if (byte.TryParse(paramMonth, out var byteMonth))
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

            if (!string.IsNullOrWhiteSpace(paramMonth) && !string.IsNullOrWhiteSpace(paramYear))
            {
                try
                {
                    var intMonth = Convert.ToInt32(paramMonth);

                    if (Enumerable.Range(1, 12).Contains(intMonth))
                    {
                        var monthName = CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(intMonth);
                        _pageTitle += $" - {monthName} / {paramYear}";
                    }
                    else
                    {
                        _pageTitle += $" - {paramMonth} / {paramYear}";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            _ = PopulateRecordItemsAsync(RecordId);
        }

    }
}
