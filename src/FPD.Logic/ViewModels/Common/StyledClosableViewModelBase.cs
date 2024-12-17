using Effanville.Common.UI;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.Logic.ViewModels.Common;

public abstract class StyledClosableViewModelBase<TModel> : ClosableViewModelBase<TModel>
    where TModel : class
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

    protected StyledClosableViewModelBase(string header, UiGlobals globals, IUiStyles styles, bool closable)
        : base(header, globals, closable)
    {
        _styles = styles;
    }

    protected StyledClosableViewModelBase(string header, TModel modelData, UiGlobals displayGlobals, IUiStyles styles, bool closable)
        : base(header, modelData, displayGlobals, closable)
    {
        _styles = styles;
    }
}

public abstract class StyledClosableViewModelBase<TModel, TUpdate> : ClosableViewModelBase<TModel, TUpdate>
    where TModel : class where TUpdate : class
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

    protected StyledClosableViewModelBase(string header, UiGlobals globals, IUiStyles styles, bool closable)
        : base(header, globals, closable)
    {
        _styles = styles;
    }

    protected StyledClosableViewModelBase(string header, TModel modelData, UiGlobals displayGlobals, IUiStyles styles, bool closable)
        : base(header, modelData, displayGlobals, closable)
    {
        _styles = styles;
    }
}