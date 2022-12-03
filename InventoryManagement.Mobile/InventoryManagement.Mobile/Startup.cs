using InventoryManagement.Mobile.Services;
using InventoryManagement.Mobile.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace InventoryManagement.Mobile
{
    public static class Startup
    {
        private static IServiceProvider serviceProvider;

        public static void ConfigureServices()
        {
            var services = new ServiceCollection();

            // add services
            services.AddHttpClient<IApiService, ApiService>();

            // AudioPlayer
            services.AddSingleton<AudioPlayer>();

            // ViewModels

            // Home Tabs
            services.AddTransient<HomeViewModel>();

            // Record
            services.AddTransient<RecordsViewModel>();
            services.AddTransient<RecordAddViewModel>();

            // Record Item
            services.AddTransient<RecordItemsViewModel>();
            services.AddTransient<RecordItemDetailViewModel>();

            // Product
            services.AddTransient<ProductsViewModel>();
            services.AddTransient<ProductDetailViewModel>();
            services.AddTransient<ProductAddViewModel>();

            // Group
            services.AddTransient<GroupAddViewModel>();
            services.AddTransient<GroupsViewModel>();
            services.AddTransient<GroupDetailViewModel>();

            // Report
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<GroupTotalsByRecordViewModel>();
            services.AddTransient<GroupTotalsByRecordSubsetViewModel>();

            // Settings
            services.AddTransient<SettingsViewModel>();

            serviceProvider = services.BuildServiceProvider();
        }

        public static T Resolve<T>() => serviceProvider.GetService<T>();
    }
}
