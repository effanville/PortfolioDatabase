using System.Collections.Generic;
using System.Linq;

using Effanville.Common.Structure.DisplayClasses;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database.Export.Statistics;
using Effanville.FinancialStructures.Database.Statistics;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.ViewModels.Common;

namespace Effanville.FPD.Logic.ViewModels.Stats
{
    public sealed class ExportDataViewModel : StyledViewModelBase<TableOptions<Statistic>>
    {
        private Statistic _SortingField;
        private SortDirection _sortingDirection;
        private List<Selectable<Statistic>> _columnNames = new List<Selectable<Statistic>>();

        public string Name { get; }

        private bool _shouldDisplay;

        public bool ShouldDisplay
        {
            get => _shouldDisplay;
            set => SetAndNotify(ref _shouldDisplay, value);
        }

        /// <summary>
        /// The statistic to sort the data by.
        /// </summary>
        public Statistic SortingField
        {
            get => _SortingField;
            set => SetAndNotify(ref _SortingField, value);
        }

        /// <summary>
        /// The direction to sort thedata in.
        /// </summary>
        public SortDirection SortingDirection
        {
            get => _sortingDirection;
            set => SetAndNotify(ref _sortingDirection, value);
        }

        /// <summary>
        /// The possible columns for security export, and which ones are selected.
        /// </summary>
        public List<Selectable<Statistic>> ColumnNames
        {
            get => _columnNames;
            set => SetAndNotify(ref _columnNames, value);
        }

        public ExportDataViewModel(string name, TableOptions<Statistic> modelData, UiGlobals displayGlobals, IUiStyles styles, IReadOnlyList<Statistic> defaultColumns)
            : base($"{name} Sorting Properties", modelData, displayGlobals, styles)
        {
            Name = $"Show {name}";
            foreach (Statistic stat in defaultColumns)
            {
                ColumnNames.Add(new Selectable<Statistic>(stat, true));
            }

            ShouldDisplay = modelData.ShouldDisplay;
            SortingField = modelData.SortingField;
            SortingDirection = modelData.SortingDirection;
        }

        public GenerateOptions<Statistic> CreateOptions()
        {
            List<Statistic> selected = new List<Statistic>();
            foreach (Selectable<Statistic> column in ColumnNames)
            {
                if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                {
                    selected.Add(column.Instance);
                }
            }

            return new GenerateOptions<Statistic>(ShouldDisplay, selected.Union(new List<Statistic>() { SortingField }).ToList());
        }

        public TableOptions<Statistic> CreateTableOptions()
        {
            List<Statistic> selected = new List<Statistic>();
            foreach (Selectable<Statistic> column in ColumnNames)
            {
                if (column.Selected || column.Instance == Statistic.Company || column.Instance == Statistic.Name)
                {
                    selected.Add(column.Instance);
                }
            }

            return new TableOptions<Statistic>(ShouldDisplay,
                SortingField,
                SortingDirection,
                selected.Union(new List<Statistic>() { SortingField }).ToList());
        }
    }
}
