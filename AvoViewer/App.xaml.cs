using Avalonia;
using Avalonia.Markup.Xaml;

namespace AvoViewer
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
   }
}