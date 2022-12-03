using InventoryManagement.Mobile.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InventoryManagement.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GroupTotalsByRecordView : ContentPage
    {
        public GroupTotalsByRecordView()
        {
            InitializeComponent();
            BindingContext = Startup.Resolve<GroupTotalsByRecordViewModel>();
        }
    }
}