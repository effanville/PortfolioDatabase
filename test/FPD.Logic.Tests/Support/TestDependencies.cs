using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

using Autofac;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.Download;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FPD.Logic.Configuration;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Asset;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Security;

using SpecFlow.Autofac;

namespace Effanville.FPD.Logic.Tests.Support;

public static class TestDependencies
{
    [ScenarioDependencies]
    public static void CreateDependencies(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(TestDependencies).Assembly).SingleInstance();
        builder.RegisterInstance(TestSetupHelper.SetupDefaultStyles());
        builder.RegisterInstance<IFileSystem>(new MockFileSystem());
        builder.RegisterInstance(TestSetupHelper.SetupReportLogger());
        builder.Register(
            b => TestSetupHelper.SetupGlobalsMock(
                b.Resolve<IFileSystem>(),
                null,
                null,
                b.Resolve<IReportLogger>()));
        builder.RegisterInstance(TestSetupHelper.SetupUpdater<IPortfolio>());
        builder.RegisterInstance(TestSetupHelper.SetupProvider());
        builder.RegisterInstance(TestSetupHelper.SetupDownloader());
        builder.RegisterInstance<IConfiguration>(new UserConfiguration());
        builder.Register(
            b => TestSetupHelper.SetupViewModelFactory(
                b.Resolve<IUiStyles>(),
                b.Resolve<UiGlobals>(),
                b.Resolve<IUpdater<IPortfolio>>(),
                b.Resolve<IPortfolioDataDownloader>(),
                b.Resolve<IConfiguration>(),
                b.Resolve<IAccountStatisticsProvider>()));

        builder.RegisterType<ViewModelTestContext<ErrorReports, ReportingWindowViewModel>>();
        builder.RegisterType<ViewModelTestContext<IPortfolio, HtmlViewerViewModel>>();
        builder.RegisterType<ViewModelTestContext<IPortfolio, BasicDataViewModel>>();
        builder.RegisterType<ViewModelTestContext<IValueList, SelectedSingleDataViewModel>>();
        builder.RegisterType<ViewModelTestContext<IPortfolio, ValueListWindowViewModel>>();
        builder.RegisterType<ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>>();
        builder.RegisterType<ViewModelTestContext<ISecurity, SelectedSecurityViewModel>>();
    }

}