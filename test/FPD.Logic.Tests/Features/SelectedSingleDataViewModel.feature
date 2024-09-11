Feature: SelectedSingleDataViewModel
Simple calculator for adding two numbers

    Scenario Outline: 001: Can open empty SelectedSingleDataViewModel
        Given I have a SelectedSingleDataViewModel with account <Account> and name Barclays-Current and no data
        And the SelectedSingleDataViewModel is brought into focus
        Then the SelectedSingleDataViewModel has 0 valuation displayed

        Examples:
          | Account     |
          | Security    |
          | Asset       |
          | BankAccount |
          | Currency    |
          | Pension     |
          | Benchmark   |

    Scenario Outline: 002: Can open SelectedSingleDataViewModel
        Given I have a SelectedSingleDataViewModel with account <Account> and name Barclays-Current and data
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-02-01 | 7     |
        And the SelectedSingleDataViewModel is brought into focus
        Then the SelectedSingleDataViewModel has 2 valuation displayed
        And the SelectedSingleDataViewModel values are
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-02-01 | 7     |

        Examples:
          | Account |
    #| Security    |
    | Asset |
    | BankAccount |
    | Currency |
    #| Pension     |
    | Benchmark |

    Scenario Outline: 003: Can open and add data
        Given I have a SelectedSingleDataViewModel with account <Account> and name Barclays-Current and no data
        And the SelectedSingleDataViewModel is brought into focus
        Then the SelectedSingleDataViewModel has 0 valuation displayed
        When I add SelectedSingleDataViewModel data with
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-02-01 | 7     |
        And the SelectedSingleDataViewModel is brought into focus
        Then the SelectedSingleDataViewModel values are
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-02-01 | 7     |

        Examples:
          | Account |
    #| Security    |
    | Asset |
    | BankAccount |
    | Currency |
    #| Pension     |
    | Benchmark |

    Scenario Outline: 004: Can open and edit data
        Given I have a SelectedSingleDataViewModel with account <Account> and name Barclays-Current and data
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-02-01 | 7     |
        And the SelectedSingleDataViewModel is brought into focus
        Then the SelectedSingleDataViewModel has 2 valuation displayed
        When I edit the SSDVM 1 entry to date 2022-02-04 and value 55
        And the SelectedSingleDataViewModel is brought into focus
        Then the SelectedSingleDataViewModel values are
          | Date       | Value |
          | 2022-02-04 | 55    |
          | 2023-02-01 | 7     |

        Examples:
          | Account |
    #| Security    |
    | Asset |
    | BankAccount |
    | Currency |
    #| Pension     |
    | Benchmark |

    Scenario Outline: 005: Can open and delete data
        Given I have a SelectedSingleDataViewModel with account <Account> and name Barclays-Current and data
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-02-01 | 7     |
          | 2023-04-01 | 9     |
        And the SelectedSingleDataViewModel is brought into focus
        Then the SelectedSingleDataViewModel has 3 valuation displayed
        When I remove the SSDVM 2 entry from the list
        And the SelectedSingleDataViewModel is brought into focus
        Then the SelectedSingleDataViewModel values are
          | Date       | Value |
          | 2023-01-01 | 5     |
          | 2023-04-01 | 9     |

        Examples:
          | Account |
        #| Security    |
          | Asset |
    | BankAccount |
    | Currency |
    #| Pension     |
    | Benchmark |
    
  Scenario Outline: 006: Can open from csv
    Given I have a SelectedSingleDataViewModel with account <Account> and name Barclays-Current and data
      | Date       | Value |
      | 2023-01-01 | 5     |
      | 2023-02-01 | 7     |
      | 2023-04-01 | 9     |
    And the SelectedSingleDataViewModel is brought into focus
    Then the SelectedSingleDataViewModel has 3 valuation displayed
    When I remove the SSDVM 2 entry from the list
    And the SelectedSingleDataViewModel is brought into focus
    Then the SelectedSingleDataViewModel values are
      | Date       | Value |
      | 2023-01-01 | 5     |
      | 2023-04-01 | 9     |

    Examples:
      | Account |
  #| Security    |
  | Asset |
  | BankAccount |
  | Currency |
  #| Pension     |
  | Benchmark |
  
  Scenario Outline: 007: Can write to csv
    Given I have a SelectedSingleDataViewModel with account <Account> and name Barclays-Current and data
      | Date       | Value |
      | 2023-01-01 | 5     |
      | 2023-02-01 | 7     |
      | 2023-04-01 | 9     |
    And the SelectedSingleDataViewModel is brought into focus
    Then the SelectedSingleDataViewModel has 3 valuation displayed
    When I remove the SSDVM 2 entry from the list
    And the SelectedSingleDataViewModel is brought into focus
    Then the SelectedSingleDataViewModel values are
      | Date       | Value |
      | 2023-01-01 | 5     |
      | 2023-04-01 | 9     |

    Examples:
      | Account |
  #| Security    |
  | Asset |
  | BankAccount |
  | Currency |
  #| Pension     |
  | Benchmark |