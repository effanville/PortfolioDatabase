Feature: DataNamesViewModel
A view model of DataNames should behave in a certain way.

    Scenario Outline: 001 Can load and view empty data
        Given I have a DataNamesViewModel with type <account> and no data
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 0

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 002 Can load and view data
        Given I have a DataNamesViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 1

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 003 Can load and update data
        Given I have a DataNamesViewModel with type <account> and no data
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 0
        When new names are added to the database
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 1
        And the dataNames portfolio has only 1 of type <account>

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 004 Can load and open data
        Given I have a DataNamesViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 1
        When I select the names row with data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And I click on the open data button
        Then the action to open the tab is called.

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 005 Can create new data
        Given I have a DataNamesViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url                   | Sectors |
          | <account> | Barclays | Current | HKD      | http://www.google.com | UK,US   |
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 1
        When I add a name with data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Fidelity | Current | USD      |     | US      |
        And the DataNamesViewModel is brought into focus
        Then the user can see the number of names is 2
        And the user can see the DataNames are
          | Account   | Company  | Name    | Currency | Url                   | Sectors |
          | <account> | Barclays | Current | HKD      | http://www.google.com | UK,US   |
          | <account> | Fidelity | Current | USD      |                       | US      |
        And the dataNames portfolio has only 2 of type <account>

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 006 Do not create matching new data
        Given I have a DataNamesViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 1
        When I add a name with data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And the DataNamesViewModel is brought into focus
        Then the user can see the number of names is 1
        And the dataNames portfolio has only 1 of type <account>

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 007 Can edit existing data
        Given I have a DataNamesViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current | GBP      |     |         |
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 1
        When I edit the 1 name data to
          | Account   | Company  | Name    | Currency | Url            | Sectors |
          | <account> | Barclays | History | HKD      | www.google.com |         |
        Then the user can see the number of names is 1
        And the user can see the DataNames are
          | Account   | Company  | Name    | Currency | Url            | Sectors |
          | <account> | Barclays | History | HKD      | www.google.com |         |
        And the dataNames portfolio has only 1 of type <account>

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 008 Can remove existing data
        Given I have a DataNamesViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
          | <account> | Fidelity | Current |          |     |         |
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 2
        When I remove the 2 data name
        Then the user can see the number of names is 1
        And the user can see the DataNames are
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And the dataNames portfolio has only 1 of type <account>

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 009: Can successfully download data
        Given I have a DataNamesViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
          | <account> | Fidelity | Current |          |     |         |
        And the DataNamesViewModel is brought into focus
        Then I can see the DataNamesViewModel type is <account>
        And I can see the DNVW has header Accounts
        And the user can see the number of names is 2
        When I select the names row with data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And I download the data for the selected DataName
        Then I can see that the data has been downloaded

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |