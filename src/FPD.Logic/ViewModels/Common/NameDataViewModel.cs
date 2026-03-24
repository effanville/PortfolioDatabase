using System;
using System.ComponentModel;

using Effanville.Common.UI;
using Effanville.Common.UI.ViewModelBases;
using Effanville.FinancialStructures.NamingStructures;

namespace Effanville.FPD.Logic.ViewModels.Common
{
    public sealed class NameDataViewModel : ViewModelBase<NameData>, IEditableObject
    {
        private readonly Action<NameData, NameData> _updateCallback;
        private NameData _preEditSelectedName;

        private bool _isUpdated;
        private string _broker;
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
        /// The broker for this instrument
        /// </summary>
        public string Broker
        {
            get => _broker;
            set => SetAndNotify(ref _broker, value);
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

        /// <summary>
        /// Empty constructor. Required for WPF to load rows with this as a view model.
        /// </summary>
        public NameDataViewModel()
            : base(null, null, null)
        { }

        public NameDataViewModel(string header,
            NameData modelData,
            bool isUpdated,
            Action<NameData, NameData> updateCallback,
            UiGlobals displayGlobals)
            : base(header, modelData, displayGlobals)
        {
            _updateCallback = updateCallback;
            IsUpdated = isUpdated;
            if (modelData == null)
            {
                return;
            }

            Broker = modelData.Broker;
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
                Broker = Broker,
                SectorsFlat = Sectors
            };
            _updateCallback(_preEditSelectedName, name);
            ModelData = name;
            IsEditing = false;
        }

        public override string ToString() => ModelData.ToString();
    }
}