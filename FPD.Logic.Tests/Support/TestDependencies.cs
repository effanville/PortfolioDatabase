using System;
using System.IO.Abstractions.TestingHelpers;

using Autofac;

using Effanville.Common.Structure.DataEdit;
using Effanville.Common.Structure.Reporting;
using Effanville.Common.UI;
using Effanville.FinancialStructures.Database;
using Effanville.FinancialStructures.FinanceStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.ViewModels;
using Effanville.FPD.Logic.ViewModels.Asset;
using Effanville.FPD.Logic.ViewModels.Common;
using Effanville.FPD.Logic.ViewModels.Security;

using Moq;

using SpecFlow.Autofac;

namespace Effanville.FPD.Logic.Tests.Support;

public static class TestDependencies
{
    [ScenarioDependencies]
    public static void CreateDependencies(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(TestDependencies).Assembly).SingleInstance();
        builder.RegisterInstance(SetupDefaultStyles());
        builder.RegisterInstance(SetupReportLogger());
        builder.RegisterInstance(SetupGlobalsMock());
        builder.RegisterInstance(SetupUpdater());
        builder.Register(
            b => SetupViewModelFactory(
                b.Resolve<UiStyles>(),
                b.Resolve<UiGlobals>(),
                b.Resolve<IUpdater<IPortfolio>>()));

        builder.RegisterType<ViewModelTestContext<ErrorReports, ReportingWindowViewModel>>();
        builder.RegisterType<ViewModelTestContext<IPortfolio, HtmlViewerViewModel>>();
        builder.RegisterType<ViewModelTestContext<IPortfolio, BasicDataViewModel>>();
        builder.RegisterType<ViewModelTestContext<IValueList, SelectedSingleDataViewModel>>();
        builder.RegisterType<ViewModelTestContext<IPortfolio, ValueListWindowViewModel>>();
        builder.RegisterType<ViewModelTestContext<IAmortisableAsset, SelectedAssetViewModel>>();
        builder.RegisterType<ViewModelTestContext<ISecurity, SelectedSecurityViewModel>>();
    }

    private static UiStyles SetupDefaultStyles()
        => new UiStyles(isLightTheme: false);

    internal static IDispatcher SetupDispatcher()
    {
        Mock<IDispatcher> dispatcherMock = new Mock<IDispatcher>();
        _ = dispatcherMock.Setup(x => x.Invoke(It.IsAny<Action>())).Callback((Action a) => a());

        _ = dispatcherMock.Setup(x => x.BeginInvoke(It.IsAny<Action>())).Callback((Action a) => a());
        return dispatcherMock.Object;
    }

    private static IUpdater<IPortfolio> SetupUpdater()
        => new SynchronousUpdater<IPortfolio>();

    private static UiGlobals SetupGlobalsMock()
        => new UiGlobals(
            null,
            SetupDispatcher(),
            new MockFileSystem(),
            null, null,
            SetupReportLogger());

    private static IViewModelFactory SetupViewModelFactory(UiStyles styles, UiGlobals globals,
        IUpdater<IPortfolio> updater)
        => new ViewModelFactory(styles, globals, updater);

    private static IReportLogger SetupReportLogger()
    {
        var reportLogger = new LogReporter(LogAction, saveInternally: true);
        return reportLogger;

        void LogAction(ReportSeverity sev, ReportType error, string loc, string msg)
        {
        }
    }
}