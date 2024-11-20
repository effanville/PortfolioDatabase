using Effanville.FinancialStructures.Database;
using Effanville.FPD.Logic.Tests.TestHelpers;
using Effanville.FPD.Logic.ViewModels;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class BasicDayViewModelSteps
{
    private readonly Context.ViewModelTestContext<IPortfolio, BasicDataViewModel> _testContext;

    public BasicDayViewModelSteps(Context.ViewModelTestContext<IPortfolio, BasicDataViewModel> testContext)
    {
        _testContext = testContext;
    }

    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [Given(@"I have a BasicDataViewModel with empty portfolio")]
    public void GivenIHaveABasicDataViewModelWithEmptyPortfolio()
    {
        IPortfolio portfolio = PortfolioFactory.GenerateEmpty();
        _testContext.ModelData = portfolio;
        _testContext.ViewModel = new BasicDataViewModel(
            _testContext.Globals,
            _testContext.Styles,
            _testContext.ModelData);
    }

    [Given(@"the BasicDataViewModel is brought into focus")]
    public void GivenTheBasicDataViewModelIsBroughtIntoFocus()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData, false);

    [Then(@"the BasicDataViewModel has (.*) notes")]
    public void ThenTheBasicDataViewModelHasNotes(int p0)
        => Assert.That(_testContext.ViewModel.Notes.Count, Is.EqualTo(0));

    [Then(@"the BasicDataViewModel has no data")]
    public void ThenTheBasicDataViewModelHasNoData()
        => Assert.That(_testContext.ViewModel.HasValues, Is.False);

    [Then(@"the BasicDataViewModel Name text is (.*)")]
    public void ThenTheBasicDataViewModelNameTextIsUnsavedDatabase(string text)
        => Assert.That(_testContext.ViewModel.PortfolioNameText, Is.EqualTo(text));

    [Given(@"I have a BasicDataViewModel with basic portfolio")]
    public void GivenIHaveABasicDataViewModelWithBasicPortfolio()
    {
        IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
        _testContext.ModelData = portfolio;
        _testContext.ViewModel = new BasicDataViewModel(
            _testContext.Globals,
            _testContext.Styles,
            _testContext.ModelData);
    }

    [When(@"the BasicDataViewModel has database updated to basic portfolio")]
    public void WhenTheBasicDataViewModelHasDatabaseUpdatedToBasicPortfolio()
    {
        IPortfolio portfolio = TestSetupHelper.CreateBasicDataBase();
        _testContext.ModelData = portfolio;
        GivenTheBasicDataViewModelIsBroughtIntoFocus();
    }

    [Then(@"the BasicDataViewModel has data")]
    public void ThenTheBasicDataViewModelHasData()
        => Assert.That(_testContext.ViewModel.HasValues, Is.True);

    [Then(@"the BasicDataViewModel has SecurityTotalText (.*)")]
    public void ThenTheBasicDataViewModelHasSecurityTotalText(string p0)
        => Assert.That(_testContext.ViewModel.SecurityTotalText, Is.EqualTo(p0));

    [Then(@"the BasicDataViewModel has SecurityAmountText (.*)")]
    public void ThenTheBasicDataViewModelHasSecurityAmountText(string p0)
        => Assert.That(_testContext.ViewModel.SecurityAmountText, Is.EqualTo(p0));

    [Then(@"the BasicDataViewModel has BankAccountTotalText (.*)")]
    public void ThenTheBasicDataViewModelHasBankAccountTotalText(string p0)
        => Assert.That(_testContext.ViewModel.BankAccountTotalText, Is.EqualTo(p0));

    [Then(@"the BasicDataViewModel has BankAccountAmountText (.*)")]
    public void ThenTheBasicDataViewModelHasBankAccountAmountTextTotalValue(string p0)
        => Assert.That(_testContext.ViewModel.BankAccountAmountText, Is.EqualTo(p0));
}