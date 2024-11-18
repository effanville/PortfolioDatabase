Feature: ValueListWindowViewModel
A view model of a collection of ValueList should behave with the
manners expected.

    Scenario Outline: 001: Can load successfully with no data
        Given I have a ValueListWindowViewModel with type <account> and no data
        And the ValueListWindowViewModel is brought into focus
        Then I can see the ValueListWindowViewModel type is <account>
        And the user can see the number of ValueListWindowViewModel tabs is 1
        And the user can see the number of VLWVM data name entries is 0

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 002: Can load successfully with data
        Given I have a ValueListWindowViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And the ValueListWindowViewModel is brought into focus
        Then I can see the ValueListWindowViewModel type is <account>
        And the user can see the number of ValueListWindowViewModel tabs is 1
        And the user can see the number of VLWVM data name entries is 1

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 003: Can Update data successfully
        Given I have a ValueListWindowViewModel with type <account> and no data
        And the ValueListWindowViewModel is brought into focus
        Then I can see the ValueListWindowViewModel type is <account>
        And the user can see the number of ValueListWindowViewModel tabs is 1
        And the user can see the number of VLWVM data name entries is 0
        When VLWVM new names are added to the database
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
        And the ValueListWindowViewModel is brought into focus
        Then I can see the ValueListWindowViewModel type is <account>
        And the user can see the number of ValueListWindowViewModel tabs is 1
        And the user can see the number of VLWVM data name entries is 1

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 004: Can update data and old tab is removed
        Given I have a ValueListWindowViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
          | <account> | Fidelity | Latest  |          |     |         |
        And the ValueListWindowViewModel is brought into focus
        Then I can see the ValueListWindowViewModel type is <account>
        And the user can see the number of ValueListWindowViewModel tabs is 1
        And the user can see the number of VLWVM data name entries is 2
        When the user loads a VLWVM tab from name
          | Company  | Name    |
          | Barclays | Current |
        And the ValueListWindowViewModel is brought into focus
        Then I can see the ValueListWindowViewModel type is <account>
        And the user can see the number of ValueListWindowViewModel tabs is 2
        And the user can see the number of VLWVM data name entries is 2
        And the user selects the VLWVM tab index 2 with name
          | Company  | Name    |
          | Barclays | Current |
        When VLWVM names are removed from the database
          | Account   | Company  | Name    |
          | <account> | Barclays | Current |
        And the ValueListWindowViewModel is brought into focus
        Then the user can see the number of ValueListWindowViewModel tabs is 1

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |

    Scenario Outline: 005: Can add tab to collection successfully
        Given I have a ValueListWindowViewModel with type <account> and data
          | Account   | Company  | Name    | Currency | Url | Sectors |
          | <account> | Barclays | Current |          |     |         |
          | <account> | Fidelity | Current |          |     |         |
        And the ValueListWindowViewModel is brought into focus
        Then I can see the ValueListWindowViewModel type is <account>
        And the user can see the number of ValueListWindowViewModel tabs is 1
        And the user can see the number of VLWVM data name entries is 2
        When the user loads a VLWVM tab from name
          | Company  | Name    |
          | Barclays | Current |
        And the ValueListWindowViewModel is brought into focus
        Then I can see the ValueListWindowViewModel type is <account>
        And the user can see the number of ValueListWindowViewModel tabs is 2
        And the user can see the number of VLWVM data name entries is 2
        And the user selects the VLWVM tab index 2 with name
          | Company  | Name    |
          | Barclays | Current |

        Examples:
          | account     |
          | Security    |
          | BankAccount |
          | Currency    |
          | Benchmark   |
          | Asset       |
          | Pension     |