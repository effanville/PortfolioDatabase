using System;

using Effanville.Common.Structure.DataStructures;
using Effanville.FPD.Logic.TemplatesAndStyles;
using Effanville.FPD.Logic.Tests.Context;
using Effanville.FPD.Logic.Tests.UserInteractions;
using Effanville.FPD.Logic.ViewModels.Common;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace Effanville.FPD.Logic.Tests.Steps;

[Binding]
public class TimeListViewModelSteps
{
    private readonly TimeListTestContext _testContext;

    public TimeListViewModelSteps(TimeListTestContext testContext, IUiStyles styles)
    {
        _testContext = testContext;
    }

    private void UpdateEvent(DailyValuation oldValue, DailyValuation newValue)
        => _testContext.UpdateCalled = true;

    private void DeleteEvent(DailyValuation valuation)
        => _testContext.DeleteCalled = true;

    [AfterScenario]
    public void Reset() => _testContext.Reset();

    [StepDefinition("I have a TimeListViewModel with name (.*) and no data")]
    public void InstantiateEmptyViewModel(string name)
        => InstantiateBasicViewModel(name, 0);

    [StepDefinition("I have a TimeListViewModel with name (.*) and (.*) entries")]
    public void InstantiateBasicViewModel(string name, int numberEntries)
    {
        TimeList timeList = new TimeList();
        DateTime date = new DateTime(2022, 1, 1);
        for (int index = 0; index < numberEntries; index++)
        {
            date = date.AddDays(index);
            timeList.SetData(date, index);
        }

        _testContext.ModelData = timeList;
        _testContext.ViewModel = new TimeListViewModel(timeList, name, _testContext.Styles, DeleteEvent, UpdateEvent);
    }

    [StepDefinition("It is brought into focus")]
    public void NavigateToViewModel()
        => _testContext.ViewModel.UpdateData(_testContext.ModelData, false);

    [Then(@"the user can view the length of the data is (.*)")]
    public void AssertViewModelHasEntryCount(int numberEntries)
        => Assert.That(_testContext.ViewModel.Valuations.Count, Is.EqualTo(numberEntries));

    [Then(@"I can see the name is (.*)")]
    public void ThenTheNameIsDisplayedAs(string name)
        => Assert.That(_testContext.ViewModel.Header, Is.EqualTo(name));

    [Then(@"The update event is called")]
    public void ThenTheUpdateEventIsCalled()
        => Assert.That(_testContext.UpdateCalled, Is.EqualTo(true));

    [Then("the delete event is called")]
    public void ThenTheDeleteEventIsCalled()
        => Assert.That(_testContext.DeleteCalled, Is.EqualTo(true));

    [Then(@"the (.*) value has date (.*) and value (.*)")]
    public void ThenTheValueHasDateAndValue(int p0, DateTime p1, int p2)
    {
        DailyValuation valuation = _testContext.ViewModel.Valuations[p0 - 1];
        Assert.Multiple(() =>
        {
            Assert.That(valuation.Day, Is.EqualTo(p1));
            Assert.That(valuation.Value, Is.EqualTo(p2));
        });
    }

    [When(@"I edit the (.*) entry to date (.*) and value (.*)")]
    public void WhenIEditTheEntryToDateAndValue(int p0, DateTime p1, decimal p2)
        => _testContext.ViewModel.EditValuation(
            _testContext.ViewModel.Valuations[p0 - 1],
            new DailyValuation(p1, p2));

    [When(@"I remove the (.*) entry from the list")]
    public void WhenIRemoveTheEntryFromTheList(int p0)
        => _testContext.ViewModel.DeleteValuation(_testContext.ViewModel.Valuations[p0 - 1]);

    [When(@"I add an entry with date (.*) and value (.*)")]
    public void WhenIAddAnEntryWithDateAndValue(DateTime p0, int p1)
        => _testContext.ViewModel.AddValuation(new DailyValuation(p0, p1));
}