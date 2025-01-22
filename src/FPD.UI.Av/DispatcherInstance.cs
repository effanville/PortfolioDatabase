using Effanville.Common.UI;

namespace Effanville.FPD.AvaloniaUI
{
    public class DispatcherInstance : IDispatcher
    {
        private readonly Avalonia.Threading.Dispatcher _dispatcher;
        public DispatcherInstance()
        {
            _dispatcher = Avalonia.Threading.Dispatcher.UIThread;
        }

        public void BeginInvoke(Action action) =>
            _dispatcher.InvokeAsync(action);
        public void Invoke(Action action) =>
        _dispatcher.Invoke(action);
    }
}