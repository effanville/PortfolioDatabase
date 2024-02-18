using System;

using FPD.Logic.TemplatesAndStyles;

using FinancialStructures.Database;
using FinancialStructures.NamingStructures;

using System.ComponentModel;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.DisplayClasses;

namespace FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// Contains a row of <see cref="NameData"/> which can be edited and can be set as new.
    /// </summary>
    public sealed class RowData : SelectableEquatable<NameData>, IEditableObject, IEquatable<RowData>
    {
        private readonly Account _typeOfAccount;
        private NameData _preEditSelectedName;

        /// <summary>
        /// Is the row a new row.
        /// </summary>
        public bool IsNew
        {
            get;
            init;
        }

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles { get; set; }

        /// <summary>
        /// Function which updates the main data store.
        /// </summary>
        private readonly IUpdater<IPortfolio> _updater;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public RowData(NameData name, bool isThis, Account accType, IUpdater<IPortfolio> update, UiStyles styles)
            : base(name, isThis)
        {
            Styles = styles;
            _typeOfAccount = accType;
            _updater = update;
        }

        /// <summary>
        /// Construct an empty instance.
        /// </summary>
        public RowData()
        {
        }
        
        /// <inheritdoc/>
        public void BeginEdit() => _preEditSelectedName = Instance?.Copy();

        /// <inheritdoc/>
        public void CancelEdit() => _preEditSelectedName = null;
        
        /// <inheritdoc/>
        public void EndEdit()
        {
            NameData selectedInstance = Instance; //rowName.Instance;
            NameData name = new NameData(selectedInstance.Company, selectedInstance.Name, selectedInstance.Currency,
                selectedInstance.Url, selectedInstance.Sectors, selectedInstance.Notes);
            _updater.PerformUpdate(null,
                new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        programPortfolio.TryEditName(_typeOfAccount, _preEditSelectedName, name, null)));
        }

        /// <inheritdoc/>
        public bool Equals(RowData other) => base.Equals(other?.Instance);

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as RowData);

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Instance);
    }
}