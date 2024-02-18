using Effanville.Common.UI;
using Effanville.Common.UI.ViewModelBases;

using FPD.Logic.TemplatesAndStyles;

namespace FPD.Logic.ViewModels.Common;

public abstract class StyledClosableViewModelBase<TModel, TUpdate> : ClosableViewModelBase<TModel, TUpdate> 
    where TModel : class where TUpdate : class
{        
    private UiStyles _styles;

    /// <summary>
    /// The style object containing the style for the ui.
    /// </summary>
    public UiStyles Styles
    {
        get => _styles;
        set => SetAndNotify(ref _styles, value);
    }
        
    protected StyledClosableViewModelBase(string header, UiGlobals globals, UiStyles styles, bool closable) 
        : base(header, globals, closable)
    {
        _styles = styles;
    }

    protected StyledClosableViewModelBase(string header, TModel modelData, UiGlobals displayGlobals, UiStyles styles, bool closable) 
        : base(header, modelData, displayGlobals, closable)
    {
        _styles = styles;
    }
}