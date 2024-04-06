using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace PromptPlayground.Views
{
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
        }

        public void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ToggleTheme(object sender, RoutedEventArgs e)
        {
            var app = Application.Current;
            if (app is not null)
            {
                var theme = app.ActualThemeVariant;
                app.RequestedThemeVariant = theme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
            }
        }
    }
}
