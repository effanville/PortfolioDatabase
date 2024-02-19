using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// Wraps a base view model with a account type record.
    /// </summary>
    public abstract class DataDisplayViewModelBase : StyledClosableViewModelBase<IPortfolio, IPortfolio>
    {
        /// <summary>
        /// The user configuration for this view model.
        /// </summary>
        protected IConfiguration UserConfiguration;

        /// <summary>
        /// The Account type the view model stores data pertaining to.
        /// </summary>
        public Account DataType
        {
            get;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected DataDisplayViewModelBase(UiGlobals globals, UiStyles styles, IConfiguration config, IPortfolio database, string header, Account dataType, bool closable = false)
            : base(header, database, globals, styles, closable)
        {
            UserConfiguration = config;
            DataType = dataType;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected DataDisplayViewModelBase(UiGlobals globals, UiStyles styles, IPortfolio database, string title, Account dataType, bool closable = false)
            : base(title, database, globals, styles, closable)
        {
            DataType = dataType;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected DataDisplayViewModelBase(UiGlobals globals, UiStyles styles, IPortfolio database, string title, bool closable = false)
            : base(title, database, globals, styles, closable)
        {
            DataType = Account.All;
        }
    }
}
