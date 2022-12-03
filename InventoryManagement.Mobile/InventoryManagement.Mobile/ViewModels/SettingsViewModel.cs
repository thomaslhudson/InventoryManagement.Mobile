using InventoryManagement.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class SettingsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly AudioPlayer _audioPlayer;
        private string _apiIpAddress;
        private string _orgApiIpAddress = null;
        private bool _saveButtonEnabled = false;

        public SettingsViewModel(AudioPlayer audioPlayer)
        {
            CancelCommand = new Command(async () => await CancelAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            ClearIPAddressCommand = new Command(() => ClearIpAddress());
            _audioPlayer = audioPlayer;
        }

        public ICommand ClearIPAddressCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }

        public string ApiIpAddress
        {
            get => _apiIpAddress;
            set
            {
                SetProperty(ref _apiIpAddress, value);
                ConfigureSaveButtonEnabled(value);
            }
        }

        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set => SetProperty(ref _saveButtonEnabled, value);
        }

        private void PopulateSettings()
        {
            _orgApiIpAddress = Preferences.Get(PreferenceKey.ApiBaseUri.ToString(), string.Empty);
            ApiIpAddress = _orgApiIpAddress;

        }

        private void ConfigureSaveButtonEnabled(string apiIpAddress)
        {
            if (string.IsNullOrWhiteSpace(apiIpAddress))
            {
                SaveButtonEnabled = false;
                return;
            }

            SaveButtonEnabled = _orgApiIpAddress != ApiIpAddress;
        }

        private void ClearIpAddress()
        {
            Preferences.Remove(PreferenceKey.ApiBaseUri.ToString());
            ApiIpAddress = "";
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync($"//{nameof(HomeView)}");
        }

        private async Task SaveAsync()
        {
            Preferences.Set(PreferenceKey.ApiBaseUri.ToString(), ApiIpAddress);
            await Shell.Current.GoToAsync($"//{nameof(HomeView)}");
        }

        public async Task PingApiServer()
        {
            if (string.IsNullOrWhiteSpace(ApiIpAddress))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Api Address cannot be empty", "OK");
            }

            Regex ipPattern = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            MatchCollection ipAddress = ipPattern.Matches(ApiIpAddress);
            Console.WriteLine(ipAddress[0]);

            Ping myPing = new Ping();
            PingReply reply = myPing.Send(ipAddress[0].ToString(), 1000);
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();
            PopulateSettings();
        }
    }
}
