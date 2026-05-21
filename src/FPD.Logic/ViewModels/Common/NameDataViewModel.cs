using Effanville.Common.UI;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.FPD.Logic.ViewModels.Common;

public sealed class NameDataViewModel : ViewModelBase<NameData>
{
    /// <summary>
    /// Is there editing of the Row ongoing.
    /// </summary>
    public bool IsEditing { get; private set; }

    public bool IsUpdated
    {
        get;
        set => SetAndNotify(ref field, value);
    }

    /// <summary>
    /// Empty constructor. Required for WPF to load rows with this as a view model.
    /// </summary>
    public NameDataViewModel()
        : base(null, null, null)
    { }

    public NameDataViewModel(string header,
        NameData modelData,
        bool isUpdated,
        UiGlobals displayGlobals)
        : base(header, modelData, displayGlobals) => IsUpdated = isUpdated;

    public override string ToString() => ModelData.ToString();
}