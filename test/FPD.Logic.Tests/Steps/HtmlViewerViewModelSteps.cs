using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.ViewModels.Common;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class HtmlViewerViewModelSteps
{
    private readonly ViewModelTestContext<IPortfolio, HtmlViewerViewModel> _testContext;

    public HtmlViewerViewModelSteps(ViewModelTestContext<IPortfolio, HtmlViewerViewModel> testContext)
    {
        _testContext = testContext;
    }

    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [Given(@"I have a HtmlViewerViewModel with name (.*) and no webpage")]
    public void GivenIHaveAHtmlViewerViewModelWithNoWebpage(string name)
        => GivenIHaveAHtmlViewerViewModelWithNoWebpage(name, null);

    [Given(@"I have a HtmlViewerViewModel with name (.*) and webpage (.*)")]
    public void GivenIHaveAHtmlViewerViewModelWithNoWebpage(string name, string webpage)
        => _testContext.ViewModel = new HtmlViewerViewModel(null, null, name, webpage);

    [Given(@"the HtmlViewerViewModel is brought into focus")]
    public void GivenTheHtmlViewerViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(modelData: null, force: false);

    [Then(@"the name is (.*)")]
    public void ThenTheNameIs(string name)
        => Assert.That(_testContext.ViewModel.Header, Is.EqualTo(name));

    [Then(@"there is no url selected")]
    public void ThenThereIsNoUrlSelected()
        => Assert.That(_testContext.ViewModel.HtmlPath, Is.Null);

    [Then(@"the url is (.*)")]
    public void ThenTheUrlIs(string url)
        => Assert.That(_testContext.ViewModel.HtmlPath.AbsoluteUri, Is.EqualTo(url));

    [When(@"the url is changed to (.*)")]
    public void WhenTheUrlIsChangedToHttpWwwYahooCom(string htmlPath)
        => _testContext.ViewModel.UrlTextPath = htmlPath;
}