using System;

using Common.Structure.DataStructures;

using FPD.Logic.TemplatesAndStyles;
using FPD.Logic.Tests.Context;
using FPD.Logic.Tests.UserInteractions;
using FPD.Logic.ViewModels.Common;

using NUnit.Framework;

using TechTalk.SpecFlow;

namespace FPD.Logic.Tests.Steps;

[Binding]
public class TimeListViewModelSteps
{
    private readonly TimeListTestContext _testContext;

    public TimeListViewModelSteps(TimeListTestContext testContext, UiStyles styles)
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
        var timeList = new TimeList();
        var date = new DateTime(2022, 1, 1);
        for (int index = 0; index < numberEntries; index++)
        {
            date = date.AddDays(index);
            timeList.AddOrEditData(date, date, index);
        }

        _testContext.ModelData = timeList;
        _testContext.ViewModel = new TimeListViewModel(timeList, name, _testContext.Styles, DeleteEvent, UpdateEvent);
    }

    [StepDefinition("It is brought into focus")]
    public void NavigateToViewModel() 
        => _testContext.ViewModel.UpdateData(_testContext.ModelData);

    [Then(@"the user can view the length of the data is (.*)")]
    public void AssertViewModelHasEntryCount(int numberEntries) 
        => Assert.AreEqual(numberEntries, _testContext.ViewModel.Valuations.Count);

    [Then(@"I can see the name is (.*)")]
    public void ThenTheNameIsDisplayedAs(string name) 
        => Assert.AreEqual(name, _testContext.ViewModel.Header);

    [Then(@"The update event is called")]
    public void ThenTheUpdateEventIsCalled() 
        => Assert.AreEqual(true, _testContext.UpdateCalled);

    [Then("the delete event is called")]
    public void ThenTheDeleteEventIsCalled() 
        => Assert.AreEqual(true, _testContext.DeleteCalled);
    
    [Then(@"the (.*) value has date (.*) and value (.*)")]
    public void ThenTheValueHasDateAndValue(int p0, DateTime p1, int p2)
    {
        var valuation = _testContext.ViewModel.Valuations[p0 - 1];
        Assert.Multiple(() =>
        {
            Assert.AreEqual(p1, valuation.Day);
            Assert.AreEqual(p2, valuation.Value);
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