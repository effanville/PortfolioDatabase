Feature: BasicDataViewModel
Simple calculator for adding two numbers

    Scenario: 001: Empty portfolio displays empty data
        Given I have a BasicDataViewModel with empty portfolio
        And the BasicDataViewModel is brought into focus
        Then the BasicDataViewModel has no data
        And the BasicDataViewModel Name text is Unsaved database
        And the BasicDataViewModel has 0 notes

    Scenario: 002: Basic data portfolio displays data
        Given I have a BasicDataViewModel with basic portfolio
        And the BasicDataViewModel is brought into focus
        Then the BasicDataViewModel has data
        And the BasicDataViewModel Name text is TestFilePath
        And the BasicDataViewModel has 0 notes
        And the BasicDataViewModel has SecurityTotalText Total Securities: 1
        And the BasicDataViewModel has SecurityAmountText Total Value: £1.00
        And the BasicDataViewModel has BankAccountTotalText Total Bank Accounts: 1
        And the BasicDataViewModel has BankAccountAmountText Total Value: £1.00

    Scenario: 003: Basic data portfolio can update data
        Given I have a BasicDataViewModel with empty portfolio
        And the BasicDataViewModel is brought into focus
        Then the BasicDataViewModel has no data
        And the BasicDataViewModel Name text is Unsaved database
        And the BasicDataViewModel has 0 notes
        When the BasicDataViewModel has database updated to basic portfolio
        Then the BasicDataViewModel has data
        And the BasicDataViewModel Name text is TestFilePath
        And the BasicDataViewModel has 0 notes
        And the BasicDataViewModel has SecurityTotalText Total Securities: 1
        And the BasicDataViewModel has SecurityAmountText Total Value: £1.00
        And the BasicDataViewModel has BankAccountTotalText Total Bank Accounts: 1
        And the BasicDataViewModel has BankAccountAmountText Total Value: £1.00