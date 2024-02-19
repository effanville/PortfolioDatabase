using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.Tests.Context;

using FPD.Logic.ViewModels;

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
        => _testContext.ViewModel.UpdateData(modelData: null);

    [Then(@"the name is (.*)")]
    public void ThenTheNameIs(string name) 
        => Assert.AreEqual(name, _testContext.ViewModel.Header);

    [Then(@"there is no url selected")]
    public void ThenThereIsNoUrlSelected()
        => Assert.IsNull(_testContext.ViewModel.HtmlPath);

    [Then(@"the url is (.*)")]
    public void ThenTheUrlIs(string url)
        => Assert.AreEqual(url, _testContext.ViewModel.HtmlPath.AbsoluteUri);

    [When(@"the url is changed to (.*)")]
    public void WhenTheUrlIsChangedToHttpWwwYahooCom(string htmlPath) 
        => _testContext.ViewModel.UrlTextPath = htmlPath;
}