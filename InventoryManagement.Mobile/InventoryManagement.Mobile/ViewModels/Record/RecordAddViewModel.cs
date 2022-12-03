using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class RecordAddViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private readonly Dictionary<string, int> MonthKeyValue = new Dictionary<string, int>
        {
            { "January", 1 }, { "February", 2 }, { "March", 3 }, { "April", 4 },
            { "May", 5 }, { "June", 6 }, { "July", 7 }, { "August", 8 },
            { "September", 9 }, { "October", 10 }, { "November", 11 }, { "December", 12 }
        };
        private KeyValuePair<string, int> _selectedMonth;
        private ObservableCollection<int> _yearList;
        private int _selectedYear;

        public RecordAddViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            CreateCommand = new Command(async () => await AddRecordItemAsync(), ValidateSave);
            CancelCommand = new Command(Cancel);
            PropertyChanged += (_, __) => CreateCommand.ChangeCanExecute();
            YearList = new ObservableCollection<int>();

            PopulateYearList();
        }

        public Command CreateCommand { get; }
        public Command CancelCommand { get; }

        public List<KeyValuePair<string, int>> MonthKeyValueList { get => MonthKeyValue.ToList(); }

        public ObservableCollection<int> YearList
        {
            get => _yearList;
            set => SetProperty(ref _yearList, value);
        }

        public KeyValuePair<string, int> SelectedMonth
        {
            get => _selectedMonth;
            set => _selectedMonth = value;
        }

        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }

        public void PopulateYearList()
        {
            var currentYear = DateTime.Now.Year;
            currentYear -= 1;
            for (int i = 0; i <= 10; i++)
            {
                YearList.Add(currentYear);
                currentYear += 1;
            }
        }

        private bool ValidateSave()
        {
            return SelectedMonth.Value > 0
                && SelectedYear > 0;
        }

        private async Task AddRecordItemAsync()
        {
            if (!byte.TryParse(SelectedMonth.Value.ToString(), out var month))
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Could not parse the Month", "OK");
                return;
            }

            if (!short.TryParse(SelectedYear.ToString(), out var year))
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Could not parse the Year", "OK");
                return;
            }

            Record record = new Record()
            {
                Id = Guid.Empty.ToString(),
                Month = month,
                Year = year
            };

            try
            {
                await _apiService.PostRecordAsync(record);
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

        private async void Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();
        }
    }
}
