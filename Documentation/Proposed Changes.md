# Unit Testing Suite

1. A precursor to the below is writing a collection of unit tests to document the behaviour of the classes and methods and, in the case of statistics functions, to ensure that the output is what should be expected 
2. Require basic tests for all methods that do non trivial logic,
3. tests for mathematics functions that do stuff
4. this should include basic creating of all objects, and should include real world examples with data from database, though anonymised 
5. create examples of test databases with sufficient data to check that values are correct
6. This will serve as a check that things are not changing fundamentally when additions and refactors are carried out in the following items
in theory most of these tests shouldn't change much, at least if the creation of various objects is abstracted to a separate place to the tests

# Event based updates

1. Editing processes output events
2. Create a custom event arg

Public portfolioEventArgs : EventArgs
{
Public bool IsSecurity
Public bool IsSector
……….
}

3. Editing events outputs event args specifying where the database has been edited. This enables updating of specific things listening based on what has changed:
    E.g. the security edit page only needs to update if changes in a security have been triggered. Bank account changes do not alter that page.
4. Require wiring of these events at creation, I believe this is already done so sufficiently
5. If one updates a grid, then this will update the database, which then updates the grid again. This scenario should be stopped as it is not desirable to update in a circular manner like this?
6. Each editing process needs to output such an event
a. Decision as to the level required to output events
b. Each editing process should only fire the event once, i.e. if you edit a security name data, you should not fire an edit event on editing the company, name etc, these should be bundled in one changed event.
7. Following from 6, it would make sense to the events to be raised at a level within the API call, as opposed to at the level of the actual reporting. This suggests a double hierarchy of events, where the lower level report to the API, then the api reports publicly what has happened. This way all changes are faithfully reported, but outside the database one does not necessarily see all detail. 
8. Consider whether this event based system also needs all logging of data changes or if this is unnecessary. Potentially use the event system to replace the report logging, or use them collectively (if report saying edit, then raise event that data changed)
9. Consider specifying what data is to be updated. is it names, or is it values for the history, or something else

# Rework of Database access

1. instead of static extension methods, restructure to have Iportfolio with method calls and xml descriptions
2. Portfolio is then a large partially defined class with the implementations
3. Organise as it currently is with separate methods in separate files to ease understandability and to keep file size down
4. folder structure review whether better organisation is possible
5. Make calls consistent
public returntype SomeFunction(accountType whatitis, some data, LogReporter reporter = null)
{
do different things based on account type
}
6. these calls should not require a reporter to be present. methods should not fail if this is the case

# Standardise Security Statistics and increase number

1. Create a base class for a statistic from a security
  ``  Public SecurityStatistic : Statistic
{
    Public SecurityStatistic(Type)
{}
Public virtual Calculate(ISecurity security);
}``
And then a statistic becomes e.g.
`` Public LatestValue : SecurityStatistic
{
Public override Calculate(ISecurity security)
{
    Value = security.LatestValue();
}
}``

2. This then allows for a custom collection of statistics on the security, e.g.

<code> Public SecurityStatistics<br>
{<br>
Public NameData securityNameData<br>
{get;set;}<br><br>
    Public List<SecurityStatistic> statistics<br>
{<br>
    get;<br>
set;<br>
} = new List<SecurityStatistic>();<br><br>
Public SecurityStatistics(ISecurity security, StatisticTypes[] statsToGenerate )<br>
{<br>
    foreach(var stat in statsToGenerate)<br>
{<br>
    statistics.Add();<br>
}<br>
}<br>
}</code>

And then one can have the output of statistics based upon what is in them, and this can be customised based upon what is in the available list of statistics
3. This system can be employed for all objects, so statistics types can be of this form.

4. One could have a base class of a statistic general enough for all bank accounts, securities etc
    ``Public Statistic
{
    Public StatisticType whatTheStatisticIs
{
Public get;
Protected set;
}
Public double Value
{
Public get;
Protected set;
}
}``

5. At some level (Statistic or SecurityStatistic) the type of the statistic should be recorded, and this type should be used, and the ToString method could return this type.
6. Using this, reporting statistics would largely be the same
7. Generation of statistics type would be very different. Some user input would specify the types required for the statistics, and then the list would be generated, before outputting.
8. Admissible statistics would be taken from a list generated from a StatisticsType enum
9. Providing headers comes from looking at stats list and the type enum
10. Providing values looks only at the values. 
11. A default minimal list of statistics should be set for the display
12. This should enable a list of unlimited statistics to be generated, without a slow down from mindlessly calculating all statistics when only 1 is required.
13. Can this be generic if the computation is passed into an api that has one function call for all types of account for each statistic 
Q: do these statistics need the Security/account name somewhere?

# Bug Fixes
1. sector recent change not displayed
2. update display a bit sketchy
3. Changing database doesn't clear all data
