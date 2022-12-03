using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace InventoryManagement.Mobile.ViewModels
{
    public class ProductDetailViewModel : BaseViewModel, IQueryAttributable
    {
        #region
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private string _productId;
        private string _name;
        #region Comment about _unitPrice
        //Using the string data type for '_unitPrice' and 'UnitPrice' solves an issue when the back space button
        //is used to remove a single zero from the Entry control (using the back space button to remove a single
        //zero from the Entry control does not fire any event when the bound Property is a decimal
        #endregion
        private string _unitPrice;
        private string _upc;
        private bool _isActive;
        private bool _updateButtonEnabled;
        private string _groupId;
        private Group _selectedGroup;
        private ObservableCollection<Group> _groups;
        private Product _originalProduct;
        #endregion

        public ProductDetailViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;
            _audioPlayer = audioPlayer;

            Groups = new ObservableCollection<Group>();

            UpdateCommand = new Command(async () => await UpdateProductAsync(), ValidateSave);
            PropertyChanged += (_, __) => UpdateCommand.ChangeCanExecute();

            CancelCommand = new Command(() => Shell.Current.GoToAsync(".."));
        }

        public Command UpdateCommand { get; }
        public Command CancelCommand { get; }

        public string ProductId
        {
            get => _productId;
            set => SetProperty(ref _productId, value);
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
            }
        }

        public string Upc
        {
            get => _upc;
            set
            {
                SetProperty(ref _upc, value);
            }
        }

        public bool IsActive
        {
            get => _isActive;
            set
            {
                SetProperty(ref _isActive, value);
            }
        }

        public string UnitPrice
        {
            get => _unitPrice;
            set
            {
                SetProperty(ref _unitPrice, value);
            }
        }

        public string GroupId
        {
            get => _groupId;
            set => SetProperty(ref _groupId, value);
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
                SetProperty(ref _selectedGroup, value);
                GroupId = _selectedGroup.Id;
            }
        }

        public bool UpdateButtonEnabled
        {
            get => _updateButtonEnabled;
            set => SetProperty(ref _updateButtonEnabled, value);
        }

        private async void Init(string Id)
        {
            try
            {
                var getGroups = GetGroupsAsync();
                var getProduct = GetProductAsync(Id);

                await Task.WhenAll(getGroups, getProduct);

                var groups = getGroups.Result;
                if (!(groups is null))
                {
                    Groups.Clear();
                    foreach (var group in groups)
                    {
                        Groups.Add(group);
                    }
                }

                var product = getProduct.Result;
                if (!(product is null))
                {
                    ProductId = product.Id;
                    Name = product.Name;
                    UnitPrice = Math.Round(product.UnitPrice, 2).ToString();
                    Upc = product.Upc;
                    IsActive = product.IsActive;
                    GroupId = product.GroupId;
                    SelectedGroup = product.Group;

                    _originalProduct = product;
                }
            }
            catch (AggregateException ex)
            {
                foreach (var iex in ex.Flatten().InnerExceptions)
                {
                    await Application.Current.MainPage.DisplayAlert("Exception", iex.Message, "OK");
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

        private bool ValidateSave()
        {
            if (_originalProduct is null)
            {
                return false;
            }

            var orgUnitPrice = Math.Round(_originalProduct.UnitPrice, 2).ToString();
            bool isValid = _originalProduct.Name != Name
                || orgUnitPrice != UnitPrice
                || _originalProduct.Upc != Upc
                || _originalProduct.IsActive != IsActive
                || _originalProduct.GroupId != GroupId
                || !_originalProduct.Group.Equals(SelectedGroup);

            return isValid;
        }

        public async Task<Product> GetProductAsync(string id)
        {
            return await _apiService.GetProductByIdAsync(id);
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync()
        {
            return await _apiService.GetGroupsAsync();
        }

        private async Task UpdateProductAsync()
        {
            decimal? unitPrice = decimal.TryParse(UnitPrice, out decimal outUnitPrice) ? outUnitPrice : (decimal?)null;
            if (unitPrice is null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Could not parse Unit Price {UnitPrice}", "OK");
                return;
            }

            try
            {
                var product = new Product
                {
                    Id = ProductId,
                    Name = Name,
                    Upc = Upc,
                    UnitPrice = (decimal)unitPrice,
                    IsActive = IsActive,
                    GroupId = GroupId,
                    Group = SelectedGroup
                };

                await _apiService.PutProductAsync(product);
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

            if (query.TryGetValue("ProductId", out var paramId))
            {
                Init(paramId);
            }
        }
    }
}