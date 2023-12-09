using Common.UI;
using Common.UI.ViewModelBases;

using FPD.Logic.TemplatesAndStyles;

namespace FPD.Logic.ViewModels.Common;

public abstract class StyledViewModelBase<TModel, TUpdate> : ViewModelBase<TModel, TUpdate>
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
    protected StyledViewModelBase(string header, UiGlobals globals, UiStyles styles) 
        : base(header, globals)
    {
        _styles = styles;
    }

    protected StyledViewModelBase(string header, TModel modelData, UiGlobals displayGlobals, UiStyles styles) 
        : base(header, modelData, displayGlobals)
    {
        _styles = styles;
    }
}

public abstract class StyledViewModelBase<T> : ViewModelBase<T> where T : class
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

    protected StyledViewModelBase(string header, UiGlobals globals, UiStyles styles) 
        : base(header, globals)
    {
        _styles = styles;
    }

    protected StyledViewModelBase(string header, T modelData, UiGlobals displayGlobals, UiStyles styles) 
        : base(header, modelData, displayGlobals)
    {
        _styles = styles;
    }
}