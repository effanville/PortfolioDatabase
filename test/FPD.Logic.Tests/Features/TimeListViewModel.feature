Feature: TimeListViewModel
  A view model of a TimeList should behave with the 
  manners expected.
  
  Scenario: 001: Can load and view empty data
    Given I have a TimeListViewModel with name NiceView and no data
    And It is brought into focus
    Then I can see the name is NiceView
    And the user can view the length of the data is 0
    
  Scenario: 002: Can load and view some data
    Given I have a TimeListViewModel with name Mark and 10 entries
    And It is brought into focus
    Then I can see the name is Mark
    And the user can view the length of the data is 10
    
  Scenario: 003: Can add data to list
    Given I have a TimeListViewModel with name Sam and 3 entries
    And It is brought into focus
    When I add an entry with date 2023-04-01 and value 12
    Then The update event is called
    And the user can view the length of the data is 4
    And the 4 value has date 2023-04-01 and value 12
      
  Scenario: 004: Can edit the data
    Given I have a TimeListViewModel with name Sam and 3 entries
    And It is brought into focus
    When I edit the 2 entry to date 2023-04-01 and value 14
    Then The update event is called
    And the user can view the length of the data is 3
    And the 2 value has date 2023-04-01 and value 14
    
  Scenario: 005: Can delete data from view
    Given I have a TimeListViewModel with name Mark and 2 entries
    And It is brought into focus
    Then the user can view the length of the data is 2
    When I remove the 2 entry from the list
    Then the user can view the length of the data is 1
    And the delete event is called