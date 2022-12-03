using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class ProductAddViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private bool _constructed;
        private string _name;
        private string _upc;
        #region Comment about _unitPrice
        //Using the string data type for '_unitPrice' and 'UnitPrice' solves an issue: removing a zero with the back
        //space button does not fire any event and therefore the 'canExecute: ValidateCreate' isn't called
        #endregion
        private string _unitPrice;
        private bool _isActive;
        private Group _group;
        private ObservableCollection<Group> _groups = new ObservableCollection<Group>();
        private Group _selectedGroup;

        public ProductAddViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            _ = PopulateGroupsAsync();

            CreateCommand = new Command(async () => await AddProductAsync(), ValidateCreate);
            PropertyChanged += (_, __) => CreateCommand.ChangeCanExecute();

            CancelCommand = new Command(CancelAsync);
        }

        public Command CreateCommand { get; }
        public Command CancelCommand { get; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Upc
        {
            get => _upc;
            set => SetProperty(ref _upc, value);
        }

        public string UnitPrice
        {
            get => _unitPrice;
            set => SetProperty(ref _unitPrice, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

        public Group Group
        {
            get => _group;
            set => SetProperty(ref _group, value);
        }

        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public Group SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (_selectedGroup != value)
                {
                    SetProperty(ref _selectedGroup, value);
                }
            }
        }

        private bool ValidateCreate()
        {
            return !string.IsNullOrWhiteSpace(_name)
                && !string.IsNullOrWhiteSpace(_upc)
                && !string.IsNullOrWhiteSpace(_unitPrice)
                && _upc.Length == 12
                && _selectedGroup != null;
        }

        public async Task PopulateGroupsAsync()
        {
            Groups.Clear();

            try
            {
                var groups = await _apiService.GetGroupsAsync();
                foreach (var group in groups)
                {
                    Groups.Add(group);
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

        private async Task AddProductAsync()
        {
            decimal? unitPrice = decimal.TryParse(UnitPrice, out decimal outUnitPrice) ? outUnitPrice : (decimal?)null;
            if (unitPrice is null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not parse Unit Price {UnitPrice}", "OK");
                return;
            }

            Product product = new Product()
            {
                Id = Guid.Empty.ToString(),
                Name = Name,
                Upc = Upc,
                IsActive = IsActive,
                UnitPrice = (decimal)unitPrice,
                GroupId = _selectedGroup.Id
            };

            try
            {
                await _apiService.PostProductAsync(product);
                await Shell.Current.GoToAsync("..");
            }
            catch (IMHttpRequestException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Exception", ex.Message, "OK");
            }
        }

        private async void CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        public void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            _audioPlayer.Play();

            if (query.TryGetValue("Upc", out string paramUpc))
            {
                if (!string.IsNullOrWhiteSpace(paramUpc))
                {
                    Upc = paramUpc;
                }
            }

            if (!_constructed)
            {
                _constructed = true;
                return;
            }

            _ = PopulateGroupsAsync();
        }
    }
}