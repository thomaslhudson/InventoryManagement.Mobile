using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class GroupAddViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private string _name;
        private bool _addButtonEnabled;

        public GroupAddViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            CreateCommand = new Command(async () => await AddGroupAsync(), ValidateSave);
            PropertyChanged += (_, __) => CreateCommand.ChangeCanExecute();

            CancelCommand = new Command(async () => await Shell.Current.GoToAsync(".."));
        }

        public Command CancelCommand { get; }
        public Command CreateCommand { get; }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
            }
        }

        public bool AddButtonEnabled
        {
            get => _addButtonEnabled;
            set => SetProperty(ref _addButtonEnabled, value);
        }

        private bool ValidateSave()
        {
            return !string.IsNullOrWhiteSpace(_name);
        }

        public async Task AddGroupAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Name cannot be empty", "OK");
                return;
            }

            try
            {
                var group = new Group { Id = Guid.NewGuid().ToString(), Name = Name };

                await _apiService.PostGroupAsync(group);

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

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();
        }
    }
}
