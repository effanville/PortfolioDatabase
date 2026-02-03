Feature: MainWindowViewModel

    Scenario Outline: 001: Can load successfully with no data
        Given I have a MainWindowViewModel with no data
        Then the user can see the number of MainWindowViewModel tabs is 10
        And the user can see a tab with type <account>

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 002: Can load successfully with data
        Given I have a MainWindowViewModel with data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        Then the user can see the number of MainWindowViewModel tabs is 10
        And the user can see a tab with type <account>
        And the user can navigate to the tab with type <account>
        And the user can see the number of data name entries is 1
        
        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 003: Can Update data successfully
        Given I have a MainWindowViewModel with no data
        Then the user can see a tab with type <account>
        And the user can navigate to the tab with type <account>
        And the user can see the number of data name entries is 0
        When MVM new names are added to the database
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        Then the user can see the number of data name entries is 1

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |
          
    Scenario Outline: 004: Can update data and old tab is removed
        Given I have a MainWindowViewModel with data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
          | <account> | Fidelity | Latest  |          |     |         |
        Then the user can see a tab with type <account>
        And the user can see the number of MainWindowViewModel tabs is 10
        And the user can navigate to the tab with type <account>
        And the user can see the number of data name entries is 2
        When the user loads a tab from name
          | Company  | Name    |
          | Barclays | Current |
        Then the user can see the number of MainWindowViewModel tabs is 11
        And the user can navigate to the tab with type <account>
        And the user can see the number of data name entries is 2
        And the user can navigate to the tab with name
          | Account   | Company  | Name    |
          | <account> | Barclays | Current |
        When MVM names are removed from the database
          | Account   | Company  | Name    |
          | <account> | Barclays | Current |
        Then the user can see the number of MainWindowViewModel tabs is 10

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 005: Can add tab to collection successfully
        Given I have a MainWindowViewModel with data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
          | <account> | Fidelity | Current |          |     |         |
        Then the user can see a tab with type <account>
        And the user can see the number of MainWindowViewModel tabs is 10
        And the user can navigate to the tab with type <account>
        And the user can see the number of data name entries is 2
        When the user loads a tab from name
          | Company  | Name    |
          | Barclays | Current |
        Then the user can see the number of MainWindowViewModel tabs is 11
        And the user can navigate to the tab with name
          | Account   | Company  | Name    |
          | <account> | Barclays | Current |

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |
