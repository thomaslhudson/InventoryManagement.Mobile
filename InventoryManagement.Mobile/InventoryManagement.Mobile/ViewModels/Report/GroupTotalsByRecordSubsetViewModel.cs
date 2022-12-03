using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static InventoryManagement.Mobile.ViewModels.BaseViewModel;

namespace InventoryManagement.Mobile.ViewModels
{
    internal class GroupTotalsByRecordSubsetViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private string _recordId;
        private string _monthYear;
        private bool _isVisibleGroups = false;
        private Record _selectedRecord;
        private string _groupsInList = "";
        private ObservableCollection<Group> _groups = new ObservableCollection<Group>();
        private ObservableCollection<Record> _records = new ObservableCollection<Record>();
        private ObservableCollection<GroupTotalsByRecord> reportData = new ObservableCollection<GroupTotalsByRecord>();

        public GroupTotalsByRecordSubsetViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            ShowGroupsCommand = new Command(ShowGroups);
        }

        public Command ShowGroupsCommand { get; }

        public double ScreenWidth
        {
            get => Screen.Width;
        }

        public double ScreenHeight
        {
            get => Screen.Height;
        }

        public string RecordId
        {
            get => _recordId;
            set => SetProperty(ref _recordId, value);
        }

        public string MonthYear
        {
            get => _monthYear;
            set => SetProperty(ref _monthYear, value);
        }

        public bool IsVisibleGroups
        {
            get => _isVisibleGroups;
            set => SetProperty(ref _isVisibleGroups, value);
        }

        public string GroupsInList
        {
            get => _groupsInList;
            set => SetProperty(ref _groupsInList, value);
        }

        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        public ObservableCollection<GroupTotalsByRecord> ReportData
        {
            get => reportData;
            set => SetProperty(ref reportData, value);
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
                    if (!string.IsNullOrWhiteSpace(GroupsInList))
                    {
                        _ = GetReportDataAsync(value.Id);
                    }
                    
                }
            }
        }

        public void ShowGroups() 
        {
            IsVisibleGroups = !IsVisibleGroups;
        }

        public async Task PopulateGroupsAsync() 
        {
            try
            {
                Groups.Clear();

                var groups = await _apiService.GetGroupsAsync();
                if (groups.Any())
                {
                    groups.ForEach(r => Groups.Add(r));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task PopulateRecordsAsync()
        {
            try
            {
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

        public async Task GetReportDataAsync(string recordId)
        {
            if (string.IsNullOrWhiteSpace(recordId))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Record id is missing", "OK");
                return;
            }

            try
            {
                var reportData = await _apiService.GetGroupTotalsByRecordAsync(recordId);
                if (reportData != null)
                {
                    ReportData.Clear();
                    reportData.FirstOrDefault(r => string.IsNullOrWhiteSpace(r.GroupName)).GroupName = "Grand Total";
                    reportData.ForEach(r => ReportData.Add(r));
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

        public async void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();

            if (query.TryGetValue("RecordId", out var recordId))
            {
                RecordId = recordId;
            }

            await PopulateGroupsAsync();
            await PopulateRecordsAsync();
        }
    }
}
