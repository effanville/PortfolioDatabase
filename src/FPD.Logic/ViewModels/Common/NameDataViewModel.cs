using System.ComponentModel;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.NamingStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;

namespace Effanville.FPD.Logic.ViewModels.Common
{
    public sealed class NameDataViewModel : StyledViewModelBase<NameData>, IEditableObject
    {
        private readonly Account _typeOfAccount;
        private NameData _preEditSelectedName;
        private readonly IDataStoreUpdater<IPortfolio> _updater;

        private bool _isUpdated;
        private string _company;
        private string _name;
        private string _url;
        private string _currency;
        private string _sectors;
        private string _notes;

        /// <summary>
        /// Is the row a new row.
        /// </summary>
        public bool IsNew { get; init; }

        /// <summary>
        /// Is there editing of the Row ongoing.
        /// </summary>
        public bool IsEditing { get; private set; }

        public bool IsUpdated
        {
            get => _isUpdated;
            set => SetAndNotify(ref _isUpdated, value);
        }

        /// <summary>
        /// The primary name (the company name)
        /// </summary>
        public string Company
        {
            get => _company;
            set => SetAndNotify(ref _company, value);
        }

        /// <summary>
        /// The secondary name.
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        /// <summary>
        /// Website associated to account.
        /// </summary>
        public string Url
        {
            get => _url;
            set => SetAndNotify(ref _url, value);
        }

        /// <summary>
        /// Any currency name.
        /// </summary>
        public string Currency
        {
            get => _currency;
            set => SetAndNotify(ref _currency, value);
        }

        /// <summary>
        /// Sectors associated to account.
        /// </summary>
        public string Sectors
        {
            get => _sectors;
            set => SetAndNotify(ref _sectors, value);
        }

        /// <summary>
        /// Sectors associated to account.
        /// </summary>
        public string Notes
        {
            get => _notes;
            set => SetAndNotify(ref _notes, value);

        }

        public NameDataViewModel(string header,
            NameData modelData,
            bool isUpdated,
            Account accType,
            IDataStoreUpdater<IPortfolio> updater,
            UiGlobals displayGlobals, IUiStyles styles)
            : base(header, modelData, displayGlobals, styles)
        {
            _typeOfAccount = accType;
            _updater = updater;
            IsUpdated = isUpdated;
            if (modelData == null)
            {
                return;
            }

            Company = modelData.Company;
            Name = modelData.Name;
            Url = modelData.Url;
            Currency = modelData.Currency;
            Sectors = modelData.SectorsFlat;
            Notes = modelData.Notes;
        }

        /// <inheritdoc/>
        public void BeginEdit()
        {
            IsEditing = true;
            _preEditSelectedName = ModelData?.Copy();
        }

        /// <inheritdoc/>
        public void CancelEdit()
        {
            IsEditing = false;
            _preEditSelectedName = null;
        }

        /// <inheritdoc/>
        public void EndEdit()
        {
            if (!IsEditing)
            {
                return;
            }

            NameData name = new NameData(
                Company,
                Name,
                Currency,
                Url,
                notes: Notes)
            {
                SectorsFlat = Sectors
            };
            _updater.PerformUpdate(null,
                new UpdateRequestArgs<IPortfolio>(true,
                    programPortfolio =>
                        programPortfolio.TryEditName(_typeOfAccount, _preEditSelectedName, name)));
            ModelData = name;
            IsEditing = false;
        }

        public override string ToString() => ModelData.ToString();
    }
}