using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class GroupDetailViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private string _Id;
        private string _name;
        private bool _saveButtonEnabled;
        private Group _originalGroup;

        public GroupDetailViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            UdpateCommand = new Command(async () => await UpdateGroupAsync(), ValidateSave);
            PropertyChanged += (_, __) => UdpateCommand.ChangeCanExecute();

            CancelCommand = new Command(() => Shell.Current.GoToAsync(".."));
        }

        public Command CancelCommand { get; }
        public Command UdpateCommand { get; }

        public string Id
        {
            get => _Id;
            set => SetProperty(ref _Id, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool SaveButtonEnabled
        {
            get => _saveButtonEnabled;
            set => SetProperty(ref _saveButtonEnabled, value);
        }

        private bool ValidateSave()
        {
            if (_originalGroup is null) return false;

            return _originalGroup.Name != Name;
        }

        public async Task LoadGroupAsync(string groupId)
        {
            try
            {
                var group = await _apiService.GetGroupAsync(groupId);
                if (groupId != null)
                {
                    Name = group.Name;
                    _originalGroup = group;
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

        public async Task UpdateGroupAsync()
        {
            try
            {
                var group = new Group
                {
                    Id = Id,
                    Name = Name
                };

                await _apiService.PutGroupAsync(group);

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

        public async void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();

            if (query.TryGetValue("Id", out var paramId))
            {
                Id = paramId;

                await LoadGroupAsync(Id);
            }
        }

    }
}
