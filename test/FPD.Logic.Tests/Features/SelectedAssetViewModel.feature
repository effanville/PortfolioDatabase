Feature: SelectedAssetViewModel

    Scenario: 001: Can open empty SelectedAssetViewModel
        Given I have a SelectedAssetViewModel with name Barclays-Current and no data
        And the SelectedAssetViewModel is brought into focus
        Then the SelectedAssetViewModel has 0 values displayed
        Then the SelectedAssetViewModel has 0 debt values displayed
        Then the SelectedAssetViewModel has 0 payment values displayed

    Scenario: 002: Can open SelectedAssetViewModel
        Given I have a SelectedAssetViewModel with name Barclays-Current and data
          | Type  | Date       | Value |
          | Value | 2023-01-01 | 5     |
          | Value | 2023-02-01 | 7     |
        And the SelectedAssetViewModel is brought into focus
        Then the SelectedAssetViewModel has 2 values displayed
        Then the SelectedAssetViewModel has 0 debt values displayed
        Then the SelectedAssetViewModel has 0 payment values displayed

    Scenario: 003: Can open SelectedAssetViewModel
        Given I have a SelectedAssetViewModel with name Barclays-Current and data
          | Type    | Date       | Value |
          | Value   | 2023-01-01 | 5     |
          | Value   | 2023-02-01 | 7     |
          | Debt    | 2023-01-01 | 77    |
          | Debt    | 2023-03-01 | 45    |
          | Debt    | 2023-04-01 | 46    |
          | Payment | 2023-01-01 | 5     |
          | Payment | 2023-02-01 | 7     |
        And the SelectedAssetViewModel is brought into focus
        Then the SelectedAssetViewModel has 2 values displayed
        Then the SelectedAssetViewModel has 3 debt values displayed
        Then the SelectedAssetViewModel has 2 payment values displayed

    Scenario: 004: Can add value SelectedAssetViewModel
        Given I have a SelectedAssetViewModel with name Barclays-Current and data
          | Type    | Date       | Value |
          | Value   | 2023-01-01 | 5     |
          | Value   | 2023-02-01 | 7     |
          | Debt    | 2023-01-01 | 77    |
          | Debt    | 2023-03-01 | 45    |
          | Debt    | 2023-04-01 | 46    |
          | Payment | 2023-01-01 | 5     |
          | Payment | 2023-02-01 | 7     |
        And the SelectedAssetViewModel is brought into focus
        Then the SelectedAssetViewModel has 2 values displayed
        Then the SelectedAssetViewModel has 3 debt values displayed
        Then the SelectedAssetViewModel has 2 payment values displayed
        When I add SAVM data with
          | Date       | Value |
          | 2023-04-01 | 6     |
        Then the SelectedAssetViewModel has 3 values displayed
        Then the SelectedAssetViewModel has 3 debt values displayed
        Then the SelectedAssetViewModel has 2 payment values displayed
        And the SAVM values are
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-02-01 | 7     |
          | 2023-04-01 | 6     |

    Scenario: 005: Can add debt value SelectedAssetViewModel
        Given I have a SelectedAssetViewModel with name Barclays-Current and data
          | Type    | Date       | Value |
          | Value   | 2023-01-01 | 5     |
          | Value   | 2023-02-01 | 7     |
          | Debt    | 2023-01-01 | 77    |
          | Debt    | 2023-03-01 | 45    |
          | Debt    | 2023-04-01 | 46    |
          | Payment | 2023-01-01 | 5     |
          | Payment | 2023-02-01 | 7     |
        And the SelectedAssetViewModel is brought into focus
        Then the SelectedAssetViewModel has 2 values displayed
        Then the SelectedAssetViewModel has 3 debt values displayed
        Then the SelectedAssetViewModel has 2 payment values displayed
        When I add SAVM debt data with
          | Date       | Value |
          | 2023-05-01 | 6     |
        Then the SelectedAssetViewModel has 2 values displayed
        Then the SelectedAssetViewModel has 4 debt values displayed
        Then the SelectedAssetViewModel has 2 payment values displayed
        And the SAVM debt values are
          | Date       | Value |
          | 2023-01-01 | 77    |
          | 2023-03-01 | 45    |
          | 2023-04-01 | 46    |
          | 2023-05-01 | 6     |

    Scenario: 006: Can add payment SelectedAssetViewModel
        Given I have a SelectedAssetViewModel with name Barclays-Current and data
          | Type    | Date       | Value |
          | Value   | 2023-01-01 | 5     |
          | Value   | 2023-02-01 | 7     |
          | Debt    | 2023-01-01 | 77    |
          | Debt    | 2023-03-01 | 45    |
          | Debt    | 2023-04-01 | 46    |
          | Payment | 2023-01-01 | 5     |
          | Payment | 2023-02-01 | 7     |
        And the SelectedAssetViewModel is brought into focus
        Then the SelectedAssetViewModel has 2 values displayed
        Then the SelectedAssetViewModel has 3 debt values displayed
        Then the SelectedAssetViewModel has 2 payment values displayed
        When I add SAVM payment data with
          | Date       | Value |
          | 2023-04-01 | 6     |
        Then the SelectedAssetViewModel has 2 values displayed
        Then the SelectedAssetViewModel has 3 debt values displayed
        Then the SelectedAssetViewModel has 3 payment values displayed
        And the SAVM payment values are
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-02-01 | 7     |
          | 2023-04-01 | 6     |
          
  Scenario: 007: Can edit Values value SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I edit the SAVM 2 entry to date 2023-02-01 and value 2
    Then the SelectedAssetViewModel has 2 values displayed
    And the SelectedAssetViewModel has 3 debt values displayed
    And the SelectedAssetViewModel has 2 payment values displayed
    And the SAVM values are
      | Date       | Value |
      | 2023-01-01 | 5     |
      | 2023-02-01 | 2     |
      
  Scenario: 008: Can edit Values date SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I edit the SAVM 2 entry to date 2023-03-01 and value 7
    Then the SelectedAssetViewModel has 2 values displayed
    And the SelectedAssetViewModel has 3 debt values displayed
    And the SelectedAssetViewModel has 2 payment values displayed
    And the SAVM values are
      | Date       | Value |
      | 2023-01-01 | 5     |
      | 2023-03-01 | 7     |
      
  Scenario: 009: Can edit Debt value SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I edit the SAVM debt 2 entry to date 2023-03-01 and value 2
    Then the SelectedAssetViewModel has 2 values displayed
    And the SelectedAssetViewModel has 3 debt values displayed
    And the SelectedAssetViewModel has 2 payment values displayed
    And the SAVM debt values are
      | Date       | Value |
      | 2023-01-01 | 77    |
      | 2023-03-01 | 2     |
      | 2023-04-01 | 46    |
      
  Scenario: 010: Can edit Debt date SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I edit the SAVM debt 2 entry to date 2023-03-05 and value 45
    Then the SelectedAssetViewModel has 2 values displayed
    And the SelectedAssetViewModel has 3 debt values displayed
    And the SelectedAssetViewModel has 2 payment values displayed
    And the SAVM debt values are
      | Date       | Value |
      | 2023-01-01 | 77    |
      | 2023-03-05 | 45    |
      | 2023-04-01 | 46    |
      
  Scenario: 011: Can edit Payment value SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I edit the SAVM payment 2 entry to date 2023-02-01 and value 2
    Then the SelectedAssetViewModel has 2 values displayed
    And the SelectedAssetViewModel has 3 debt values displayed
    And the SelectedAssetViewModel has 2 payment values displayed
    And the SAVM payment values are
      | Date       | Value |
      | 2023-01-01 | 5     |
      | 2023-02-01 | 2     |
      
  Scenario: 012: Can edit Payment date SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I edit the SAVM payment 2 entry to date 2023-03-01 and value 7
    Then the SelectedAssetViewModel has 2 values displayed
    And the SelectedAssetViewModel has 3 debt values displayed
    And the SelectedAssetViewModel has 2 payment values displayed
    And the SAVM payment values are
      | Date       | Value |
      | 2023-01-01 | 5     |
      | 2023-03-01 | 7     |
      
  Scenario: 013: Can delete value SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I remove the SAVM 2 entry from the list
    Then the SelectedAssetViewModel has 1 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    And the SAVM values are
      | Date       | Value |
      | 2023-01-01 | 5     |
      
  Scenario: 014: Can delete debt value SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I remove the SAVM debt 2 entry from the list
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 2 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    And the SAVM debt values are
      | Date       | Value |
      | 2023-01-01 | 77    |
      | 2023-04-01 | 46    |
      
  Scenario: 015: Can delete payment value SelectedAssetViewModel
    Given I have a SelectedAssetViewModel with name Barclays-Current and data
      | Type    | Date       | Value |
      | Value   | 2023-01-01 | 5     |
      | Value   | 2023-02-01 | 7     |
      | Debt    | 2023-01-01 | 77    |
      | Debt    | 2023-03-01 | 45    |
      | Debt    | 2023-04-01 | 46    |
      | Payment | 2023-01-01 | 5     |
      | Payment | 2023-02-01 | 7     |
    And the SelectedAssetViewModel is brought into focus
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 2 payment values displayed
    When I remove the SAVM payment 2 entry from the list
    Then the SelectedAssetViewModel has 2 values displayed
    Then the SelectedAssetViewModel has 3 debt values displayed
    Then the SelectedAssetViewModel has 1 payment values displayed
    And the SAVM payment values are
      | Date       | Value |
      | 2023-01-01 | 5     |