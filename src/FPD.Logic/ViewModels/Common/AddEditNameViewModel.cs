using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

using Effanville.Common.UI;
using Effanville.Common.UI.Commands;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.FPD.Logic.ViewModels.Common;

public class AddEditNameViewModel : ClosableViewModelBase<NameData>, INotifyDataErrorInfo
{
    private readonly Dictionary<string, string> _errorsByPropertyName = [];
    private readonly Func<NameData, Task> _onAddComplete;
    public string CommandName { get; set => SetAndNotify(ref field, value); }


    /// <summary>
    /// Whether a company column should be displayed.
    /// </summary>
    public bool DisplayCompany { get; }

    /// <summary>
    /// Whether a broker column should be displayed
    /// </summary>
    public bool DisplayBroker { get; }
    public string Broker
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Broker = field;
        }
    }

    public string Company
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Company = field;
            Validate();
        }
    }

    public string Name
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Name = field;
            Validate();
        }
    }

    public string Ticker
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Ticker = field;
        }
    }

    public string Ric
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Ric = field;
        }
    }

    public string Sedol
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Ticker = field;
        }
    }

    public string Isin
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Isin = field;
        }
    }

    public string Exchange
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Exchange = field;
        }
    }

    public string Url
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Url = field;
        }
    }

    public string Currency
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Currency = field;
        }
    }

    public string Sectors
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.SectorsFlat = field;
        }
    }

    public string Notes
    {
        get;
        set
        {
            if (SetAndNotify(ref field, value))
                ModelData.Notes = field;
        }
    }

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public AddEditNameViewModel(string command, bool displayCompany, bool displayBroker, NameData model, Func<NameData, Task> onAddComplete, UiGlobals globals)
        : base("Add/Edit entry", model, globals, true)
    {
        CommandName = command;
        DisplayCompany = displayCompany;
        DisplayBroker = displayBroker;
        _onAddComplete = onAddComplete;
        CompleteCommand = new RelayCommandAsync(ExecuteAdd);
        Broker = ModelData.Broker;
        Company = ModelData.Company;
        Name = ModelData.Name;
        Url = ModelData.Url;
        Currency = ModelData.Currency;
        Sectors = ModelData.SectorsFlat;
        Notes = ModelData.Notes;
        Validate();
    }

    public AddEditNameViewModel(string command, bool displayCompany, bool displayBroker, Func<NameData, Task> onAddComplete, UiGlobals globals)
        : this(command, displayCompany, displayBroker, new(), onAddComplete, globals)
    {
    }

    public ICommand CompleteCommand { get; }

    public bool HasErrors => _errorsByPropertyName.Count > 0;

    private async Task ExecuteAdd()
    {
        await _onAddComplete(ModelData);
        OnRequestClose(EventArgs.Empty);
    }

    public IEnumerable GetErrors(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return null;

        return _errorsByPropertyName.TryGetValue(propertyName, out string value)
                ? value
                : null;
    }

    private void Validate()
    {
        if (string.IsNullOrEmpty(Company) && string.IsNullOrEmpty(Name))
        {
            _errorsByPropertyName[nameof(Company)] = "Cannot have both Company and Name null or empty";
            _errorsByPropertyName[nameof(Name)] = "Cannot have both Company and Name null or empty";
            OnErrorsChanged(nameof(Company));
            OnErrorsChanged(nameof(Name));
        }
        else
        {
            if (_errorsByPropertyName.Remove(nameof(Name)))
            {
                OnErrorsChanged(nameof(Name));
            }
            if (_errorsByPropertyName.Remove(nameof(Company)))
            {
                OnErrorsChanged(nameof(Company));
            }
        }
    }
    private void OnErrorsChanged(string propertyName)
        => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
}
