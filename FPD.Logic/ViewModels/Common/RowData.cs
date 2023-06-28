using System;
using Common.Structure.DisplayClasses;
using FPD.Logic.TemplatesAndStyles;
using FinancialStructures.Database;
using FinancialStructures.NamingStructures;
using System.ComponentModel;

namespace FPD.Logic.ViewModels.Common
{
    /// <summary>
    /// Contains a row of <see cref="NameData"/> which can be edited and can be set as new.
    /// </summary>
    public sealed class RowData : SelectableEquatable<NameData>, IEditableObject, IEquatable<RowData>
    {
        private readonly Account TypeOfAccount;
        private NameData fPreEditSelectedName;

        /// <summary>
        /// Is the row a new row.
        /// </summary>
        public bool IsNew
        {
            get; set;
        }

        private UiStyles fStyles;

        /// <summary>
        /// The style object containing the style for the ui.
        /// </summary>
        public UiStyles Styles
        {
            get => fStyles;
            set => fStyles = value;
        }

        /// <summary>
        /// Function which updates the main data store.
        /// </summary>
        private readonly Action<Action<IPortfolio>> UpdateDataCallback;

        /// <summary>
        /// Construct an instance.
        /// </summary>
        public RowData(NameData name, bool isThis, Account accType, Action<Action<IPortfolio>> update, UiStyles styles)
            : base(name, isThis)
        {
            Styles = styles;
            TypeOfAccount = accType;
            UpdateDataCallback = update;
        }

        /// <summary>
        /// Construct an empty instance.
        /// </summary>
        public RowData()
        {
        }


        /// <inheritdoc/>
        public void BeginEdit()
        {
            fPreEditSelectedName = Instance?.Copy();
        }

        /// <inheritdoc/>
        public void CancelEdit()
        {
            fPreEditSelectedName = null;
        }

        /// <inheritdoc/>
        public void EndEdit()
        {
            NameData selectedInstance = Instance; //rowName.Instance;

            // maybe fired from editing stuff. Try that
            if (!string.IsNullOrEmpty(selectedInstance.Name) || !string.IsNullOrEmpty(selectedInstance.Company))
            {
                NameData name = new NameData(selectedInstance.Company, selectedInstance.Name, selectedInstance.Currency, selectedInstance.Url, selectedInstance.Sectors, selectedInstance.Notes);
                UpdateDataCallback(programPortfolio => programPortfolio.TryEditName(TypeOfAccount, fPreEditSelectedName, name, null));
            }
        }

        /// <inheritdoc/>
        public bool Equals(RowData other)
        {
            return base.Equals(other?.Instance);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as RowData);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Instance);
        }
    }
}
