using Effanville.Common.UI;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.Logic.ViewModels.Common;

public abstract class StyledViewModelBase<T> : ViewModelBase<T> where T : class
{
    private IUiStyles _styles;

    /// <summary>
    /// The style object containing the style for the ui.
    /// </summary>
    public IUiStyles Styles
    {
        get => _styles;
        set => SetAndNotify(ref _styles, value);
    }

    protected StyledViewModelBase(string header, UiGlobals globals, IUiStyles styles)
        : base(header, globals)
    {
        _styles = styles;
    }

    protected StyledViewModelBase(string header, T modelData, UiGlobals displayGlobals, IUiStyles styles)
        : base(header, modelData, displayGlobals)
    {
        _styles = styles;
    }
}