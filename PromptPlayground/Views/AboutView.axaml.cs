using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

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
    }
}
