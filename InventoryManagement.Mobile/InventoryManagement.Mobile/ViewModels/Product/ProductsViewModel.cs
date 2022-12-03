using InventoryManagement.Mobile.Models;
using InventoryManagement.Mobile.Services;
using InventoryManagement.Mobile.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Runtime.InteropServices;

namespace InventoryManagement.Mobile.ViewModels
{
    public class ProductsViewModel : BaseViewModel, IQueryAttributable
    {
        private readonly IApiService _apiService;
        private readonly AudioPlayer _audioPlayer;
        private bool _constructed;
        private string _searchText = string.Empty;
        private string _selectedStatus = "Active";
        private readonly List<Product> _products = new List<Product>();
        private ObservableCollection<Product> _productsFiltered = new ObservableCollection<Product>();
        private ObservableCollection<Group> _groups = new ObservableCollection<Group>();
        private Group _selectedGroup;
        private bool _searchBarEnabled;
        private string _filterButtonImageSource = "FiltersShow.png";
        private bool _filtersVisible = false;
        private Group _savedSelectedGroup;

        public ProductsViewModel(IApiService apiService, AudioPlayer audioPlayer)
        {
            _apiService = apiService;

            _audioPlayer = audioPlayer;
            _audioPlayer.Play();

            _ = PopulateGroupsList();
            _ = PopulateProductListAsync();

            ToggleFiltersCommand = new Command(c => ToggleFilters());
            ToggleIsActiveCommand = new Command<Product>(async p => await ToggleIsActive(p));
            AddProductCommand = new Command(async () => await AddProduct());
            SelectProductCommand = new Command<string>(async (productId) => { await SelectProduct(productId); });
        }

        public Command ToggleFiltersCommand { get; }
        public Command ToggleIsActiveCommand { get; }
        public Command AddProductCommand { get; }
        public Command SelectProductCommand { get; set; }

        public ObservableCollection<Product> ProductsFiltered
        {
            get => _productsFiltered;
            set => SetProperty(ref _productsFiltered, value);
        }

        public ObservableCollection<Group> Groups
        {
            get => _groups;
            set => SetProperty(ref _groups, value);
        }

        public bool SearchBarEnabled
        {
            get => _searchBarEnabled;
            set => SetProperty(ref _searchBarEnabled, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                Search(value);
            }
        }

        public Group SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                SetProperty(ref _selectedGroup, value);
                if (_products.Any())
                {
                    FilterProductsList();
                }
            }
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                SetProperty(ref _selectedStatus, value);
                if (_products.Any())
                {
                    FilterProductsList();
                }
            }
        }

        public string FilterButtonImageSource
        {
            get => _filterButtonImageSource;
            set => SetProperty(ref _filterButtonImageSource, value);
        }

        public bool FiltersVisible
        {
            get => _filtersVisible;
            set => SetProperty(ref _filtersVisible, value);
        }

        private Group ShowAllGroup
        {
            get => new Group { Id = "1", Name = "Show All Groups" };
        }

        public void ToggleFilters()
        {
            switch (FiltersVisible)
            {
                case true:
                    FiltersVisible = false;
                    FilterButtonImageSource = "FiltersShow.png";
                    break;
                case false:
                    FiltersVisible = true;
                    FilterButtonImageSource = "FiltersHide.png";
                    break;
            }
        }

        private async Task ToggleIsActive(Product p)
        {
            // This executes in response to the 'Swipe' actions
            p.IsActive = !p.IsActive;
            await _apiService.PutProductAsync(p);
            await PopulateProductListAsync();
            return;
        }

        private async Task AddProduct()
        {
            await Shell.Current.GoToAsync(nameof(ProductAddView));
        }

        private async Task SelectProduct(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            _savedSelectedGroup = SelectedGroup;

            await Shell.Current.GoToAsync($"{nameof(ProductDetailView)}?{nameof(ProductDetailViewModel.ProductId)}={id}");
        }

        public async Task PopulateProductListAsync()
        {
            _products.Clear();

            try
            {
                var products = await _apiService.GetProductsAsync();
                if (products != null && products.Any())
                {
                    products.ForEach(p => _products.Add(p));
                    FilterProductsList();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No products were retrieved", "OK");
                    return;
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

        public async Task PopulateGroupsList()
        {
            try
            {
                var groups = await _apiService.GetGroupsAsync();
                if (groups.Any())
                {
                    Groups.Clear();
                    Groups.Add(ShowAllGroup);
                    groups.ForEach(p => Groups.Add(p));
                    if (Groups.Any())
                    {
                        if (_savedSelectedGroup is null)
                        {
                            SelectedGroup = Groups[0];
                        }
                        else
                        {
                            SelectedGroup = _savedSelectedGroup;
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No groups were retrieved", "OK");
                    return;
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

        public void FilterProductsList()
        {
            if (!_products.Any())
            {
                Application.Current.MainPage.DisplayAlert("Error", "Error filtering the Product list", "OK");
                return;
            }

            ProductsFiltered.Clear();

            var selGroup = SelectedGroup;
            List<Product> filteredProducts = _products;

            try
            {
                if (selGroup != null && !selGroup.Equals(ShowAllGroup))
                {
                    filteredProducts = filteredProducts.Where(p => p.Group.Equals(selGroup)).ToList();
                }

                if (SelectedStatus == "Active")
                {
                    filteredProducts = filteredProducts.Where(p => p.IsActive == true).ToList();
                }
                else if (SelectedStatus == "Inactive")
                {
                    filteredProducts = filteredProducts.Where(p => p.IsActive == false).ToList();
                }

                filteredProducts.ForEach(p => ProductsFiltered.Add(p));
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }

            SearchBarEnabled = ProductsFiltered.Any();
        }

        public void Search(string searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                List<Product> products;
                switch (SelectedStatus)
                {
                    case "Active":
                        products = _products.Where(p => p.IsActive == true).Where(p => p.Name.ToLower().Contains(searchText.ToLower())).ToList();
                        break;
                    case "Inactive":
                        products = _products.Where(p => p.IsActive == false).Where(p => p.Name.ToLower().Contains(searchText.ToLower())).ToList();
                        break;
                    default:
                        products = _products.Where(p => p.Name.ToLower().Contains(searchText.ToLower())).ToList();
                        break;
                }

                ProductsFiltered.Clear();
                products.ForEach(p => ProductsFiltered.Add(p));
            }
            else
            {
                ProductsFiltered.Clear();
                switch (SelectedStatus)
                {
                    case "Active":
                        _products.Where(p => p.IsActive == true).ForEach(p => ProductsFiltered.Add(p));
                        break;
                    case "Inactive":
                        _products.Where(p => p.IsActive == false).ForEach(p => ProductsFiltered.Add(p));
                        break;
                    default:
                        _products.ForEach(p => ProductsFiltered.Add(p));
                        break;
                }
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

            SearchText = "";

            _ = PopulateGroupsList();
            _ = PopulateProductListAsync();
        }
    }
}
