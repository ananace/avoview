using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace AvoViewer
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}