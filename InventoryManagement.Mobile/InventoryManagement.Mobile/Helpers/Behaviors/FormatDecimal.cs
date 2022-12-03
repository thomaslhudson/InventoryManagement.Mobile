using Xamarin.Forms;

namespace InventoryManagement.Mobile.Helpers.Behaviors
{
    public class FormatDecimal : Behavior<Entry>
    {
        public static BindableProperty NumberOfDecimalPlacesProp = BindableProperty.Create(nameof(NumberOfDecimalPlaces), typeof(int), typeof(FormatDecimal), 2);

        public int NumberOfDecimalPlaces
        {
            get
            {
                return (int)GetValue(NumberOfDecimalPlacesProp);
            }
            set
            {
                SetValue(NumberOfDecimalPlacesProp, value);
            }
        }

        protected override void OnAttachedTo(Entry entry)
        {
            entry.TextChanged += OnEntryTextChanged;
            base.OnAttachedTo(entry);
        }

        protected override void OnDetachingFrom(Entry entry)
        {
            entry.TextChanged -= OnEntryTextChanged;
            base.OnDetachingFrom(entry);
        }

        void OnEntryTextChanged(object sender, TextChangedEventArgs args)
        {
            if (sender is Entry entry)
            {
                if (args.NewTextValue.Contains("."))
                {
                    if (args.NewTextValue.Length - 1 - args.NewTextValue.IndexOf(".") > NumberOfDecimalPlaces)
                    {
                        var s = args.NewTextValue.Substring(0, args.NewTextValue.IndexOf(".") + NumberOfDecimalPlaces + 1);
                        entry.Text = string.Format("{0:C,0.00}", s);
                        entry.SelectionLength = s.Length;
                    }
                }
            }
        }
    }
}
