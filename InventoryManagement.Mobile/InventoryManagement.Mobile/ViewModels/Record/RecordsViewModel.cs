using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using InventoryManagement.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class RecordsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private ObservableCollection<Record> _records;

        public RecordsViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;
            
            Records = new ObservableCollection<Record>();
            AddRecordCommand = new Command(async () => await GoToAddRecordViewAsync());
            SelectRecordCommand = new Command<Record>(async (r) => { await SelectRecordAsync(r); });
            DeleteRecordCommand = new Command<Record>(async (r) => { await DeleteRecordAsync(r); });
        }

        public ICommand AddRecordCommand { get; }
        public ICommand SelectRecordCommand { get; set; }
        public ICommand DeleteRecordCommand { get; set; }
        public ObservableCollection<Record> Records
        {
            get => _records;
            set => SetProperty(ref _records, value);
        }

        private async Task GoToAddRecordViewAsync()
        {
            await Shell.Current.GoToAsync(nameof(RecordAddView));
        }

        public async Task DeleteRecordAsync(Record r)
        {
            //try
            //{
            //    await _apiService.DeleteRecordAsync(r);
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            //PopulateRecordsAsync();
            await Application.Current.MainPage.DisplayAlert("Not Implemented", "Records cannot be deleted at this time", "OK");
        }

        private async Task SelectRecordAsync(Record record)
        {
            await Shell.Current.GoToAsync($"{nameof(RecordItemsView)}?" +
                $"{nameof(RecordItemsViewModel.RecordId)}={record.Id}" +
                $"&{nameof(RecordItemsViewModel.Month)}={record.Month}" +
                $"&{nameof(RecordItemsViewModel.Year)}={record.Year}");
        }

        public async Task PopulateRecordsAsync()
        {
            try
            {
                Records.Clear();

                var records = await _apiService.GetRecordsAsync();
                foreach (var record in records)
                {
                    Records.Add(record);
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
            await PopulateRecordsAsync();
        }
    }
}
