using System;
using GuiSupport;

namespace FinanceWindowsViewModels
{
    public class StatsCreatorWindowViewModel : PropertyChangedBase
    {
        Action<bool> UpdateMainWindow;
        Action<string> windowToView;

        public StatsCreatorWindowViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            windowToView = pageViewChoice;
            UpdateMainWindow = updateWindow;
        }
    }
}
