using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using InventoryManagement.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace InventoryManagement.Mobile.ViewModels
{
    public class GroupsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private bool _constructed;
        private ObservableCollection<Group> _groups = new ObservableCollection<Group>();

        public GroupsViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            _ = PopulateGroupsAsync();

            DeleteGroupCommand = new Command<Group>(async i => await DeleteGroupAsync(i));
            AddGroupCommand = new Command(async () => await GoToAddGroupViewAsync());
            SelectGroupCommand = new Command<string>(async (groupId) => { await SelectGroupAsync(groupId); });
        }

        public Command DeleteGroupCommand { get; }
        public Command AddGroupCommand { get; }
        public Command SelectGroupCommand { get; set; }

        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        private async Task DeleteGroupAsync(Group i)
        {
            //await _apiService.DeleteGroup(i);
            //PopulateGroupsAsync();
            //throw new NotImplementedException();
            await Application.Current.MainPage.DisplayAlert("Information", "Functionality not implemented", "OK");
        }

        private async Task GoToAddGroupViewAsync()
        {
            await Shell.Current.GoToAsync(nameof(GroupAddView));
        }

        private async Task SelectGroupAsync(string groupId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Group Id cannot be null", "OK");
                return;
            }

            try
            {
                await Shell.Current.GoToAsync($"{nameof(GroupDetailView)}?{nameof(GroupDetailViewModel.Id)}={groupId}");
            }
            catch (Exception ex) 
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        public async Task PopulateGroupsAsync()
        {
            Groups.Clear();

            try
            {
                var groups = await _apiService.GetGroupsAsync();
                groups.ForEach(g => Groups.Add(g));
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

            if (!_constructed)
            {
                _constructed = true;
                return;
            }

            _ = PopulateGroupsAsync();
        }
    }
}
