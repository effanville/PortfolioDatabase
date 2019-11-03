using GUIAccessorFunctions;
using System;
using System.Windows.Forms;
using System.Windows.Input;
using GlobalHeldData;
using FinanceWindows;
using GUISupport;

namespace FinanceWindowsViewModels
{
    public class OptionsPanelViewModel : PropertyChangedBase
    {
        public ICommand OpenSecurityEditWindowCommand { get; }

        public ICommand OpenBankAccountEditWindowCommand { get; }

        public ICommand OpenSectorEditWindowCommand { get; }

        public ICommand OpenStatsCreatorWindowCommand { get; }

        public ICommand SaveDatabaseCommand { get; }

        public void ExecuteSecurityEditWindow(Object obj)
        {
            windowToView("SecurityEditWindow");
        }

        public void ExecuteBankAccEditWindow(Object obj)
        {
            windowToView("BankAccEditWindow");
        }
        public void ExecuteSectorEditWindow(Object obj)
        {
            windowToView("SectorEditWindow");
        }
        public void ExecuteStatsCreatorWindow(Object obj)
        {
            windowToView("StatsCreatorWindow");
        }

        public void ExecuteSaveDatabase(Object obj)
        {
            SaveFileDialog saving = new SaveFileDialog();
            if (saving.ShowDialog() == DialogResult.OK)
            {
                //if (!File.Exists(saving.FileName))
                {
                    GlobalData.fDatabaseFilePath = saving.FileName;
                }
            }

            DatabaseAccessor.SavePortfolio();
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;

        public OptionsPanelViewModel(Action<bool> updateWindow, Action<string> pageViewChoice)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;

            OpenSecurityEditWindowCommand = new BasicCommand(ExecuteSecurityEditWindow);
            OpenBankAccountEditWindowCommand = new BasicCommand(ExecuteBankAccEditWindow);
            OpenSectorEditWindowCommand = new BasicCommand(ExecuteSectorEditWindow);
            OpenStatsCreatorWindowCommand = new BasicCommand(ExecuteStatsCreatorWindow);
            SaveDatabaseCommand = new BasicCommand(ExecuteSaveDatabase);
        }
    }
}
