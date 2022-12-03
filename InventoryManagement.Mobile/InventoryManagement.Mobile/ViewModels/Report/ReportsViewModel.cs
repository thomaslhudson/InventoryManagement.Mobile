using InventoryManagement.Mobile.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xamarin.Forms;
using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Internals;
using InventoryManagement.Mobile.Views.Report;

namespace InventoryManagement.Mobile.ViewModels
{
    public class ReportsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private string _recordId;
        private ObservableCollection<Record> _records;
        private string _monthYear;
        private Record _selectedRecord;

        public ReportsViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            GroupTotalsByRecordCommand = new Command(async () => await ShowGroupTotalsByRecordAsync());
            GroupTotalsByRecordSubsetCommand = new Command(async () => await ShowGroupTotalsByRecordSubsetAsync());
        }

        public Command GroupTotalsByRecordCommand { get; }
        public Command GroupTotalsByRecordSubsetCommand { get; }

        public double ScreenWidth
        {
            get => Screen.Width;
        }

        public string RecordId
        {
            get => _recordId;
            set => SetProperty(ref _recordId, value);
        }

        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        public string MonthYear
        {
            get => _monthYear;
            set => SetProperty(ref _monthYear, value);
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
                }
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
                    SelectedRecord = string.IsNullOrWhiteSpace(RecordId) ? Records[0] : Records.FirstOrDefault(i => i.Id == RecordId);
                    MonthYear = SelectedRecord.MonthYear;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ShowGroupTotalsByRecordAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(GroupTotalsByRecordView)}?{nameof(GroupTotalsByRecordViewModel.RecordId)}={RecordId}");
        }

        public async Task ShowGroupTotalsByRecordSubsetAsync()
        {
            await Shell.Current.GoToAsync($"{nameof(GroupTotalsByRecordSubsetView)}?{nameof(GroupTotalsByRecordSubsetViewModel.RecordId)}={RecordId}");
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();

            _ = PopulateRecordsAsync();
        }
    }
}
