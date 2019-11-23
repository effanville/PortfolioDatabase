using GUIFinanceStructures;
using GUISupport;
using ReportingStructures;
using SecurityHelperFunctions;
using System;
using System.Windows.Input;

namespace FinanceWindowsViewModels.SecurityEdit
{
    public class NameCompanyEditViewModel : PropertyChangedBase
    {
        private string fSelectedCompanyEdit;
        public string SelectedCompanyEdit
        {
            get { return fSelectedCompanyEdit; }
            set { fSelectedCompanyEdit = value; OnPropertyChanged(); }
        }

        private string fSelectedNameEdit;
        public string SelectedNameEdit
        {
            get { return fSelectedNameEdit; }
            set { fSelectedNameEdit = value; OnPropertyChanged(); }
        }

        private NameComp fSelectedName;

        private bool fEditing;

        public bool Editing
        {
            get { return fEditing; }
            set { fEditing = value; OnPropertyChanged(); }
        }

        public bool NotEditing
        {
            get { return !fEditing; }
            set { fEditing = !value; OnPropertyChanged(); }
        }

        public ICommand CreateSecurityButtonCommand { get; }
        private void ExecuteCreateSecurityButton(Object obj)
        {
            if (SelectedNameEdit != null || SelectedCompanyEdit != null)
            {
                SecurityEditor.TryAddSecurity(SelectedNameEdit, SelectedCompanyEdit);
            }
            else
            {
                ErrorReports.AddError("Both Name and company given were null");
            }
            
            UpdateMainWindow(true);
            SubWindowView("default", false);
        }

        public ICommand EditSecurityNameCommand { get; }
        private void ExecuteEditSecurityName(Object obj)
        {
            if (fSelectedName != null)
            {
                SecurityEditor.TryEditSecurityName(fSelectedName.Name, fSelectedName.Company, SelectedNameEdit, SelectedCompanyEdit);

            }

            UpdateMainWindow(true);
            SubWindowView("default", false);
        }

        public void UpdateNameCompEditViewData(string newNameEdit, string newCompanyEdit, NameComp newName)
        {
            fSelectedName = newName;
            SelectedNameEdit = newNameEdit;
            SelectedCompanyEdit = newCompanyEdit;
        }

        Action<bool> UpdateMainWindow;
        Action<string> windowToView;
        Action<string, bool> SubWindowView;

        public NameCompanyEditViewModel(Action<bool> updateWindow, Action<string> pageViewChoice, Action<string, bool> subWindowToView, string newNameEdit, string newCompanyEdit, NameComp newName)
        {
            UpdateMainWindow = updateWindow;
            windowToView = pageViewChoice;
            SubWindowView = subWindowToView;
            SelectedCompanyEdit = newCompanyEdit;
            SelectedNameEdit = newNameEdit;
            fSelectedName = newName;
            CreateSecurityButtonCommand = new BasicCommand(ExecuteCreateSecurityButton);
            EditSecurityNameCommand = new BasicCommand(ExecuteEditSecurityName);
        }
    }
}
