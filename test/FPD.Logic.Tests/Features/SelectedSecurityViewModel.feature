Feature: SelectedSecurityViewModel

    Scenario Outline: 001: Can open empty SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and no data
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 0 unitprices displayed
        Then the SelectedSecurityViewModel has 0 trades displayed

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 002: Can open SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 0 trades displayed

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 003: Can open SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 004: Can add data SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        When I add SelectedSecurityViewModel data with
          | Date       | UnitPrice |
          | 2023-03-01 | 7         |
        Then the SelectedSecurityViewModel has 3 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        And the SSVM values are
          | Date       | UnitPrice |
          | 2023-01-01 | 5         |
          | 2023-02-01 | 7         |
          | 2023-03-01 | 7         |

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 005: Can edit data SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        When I edit the SSVM 1 entry to date 2023-01-01 and value 50
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        And the SSVM values are
          | Date       | UnitPrice |
          | 2023-01-01 | 50        |
          | 2023-02-01 | 7         |

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 006: Can edit data date SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        When I edit the SSVM 1 entry to date 2023-01-15 and value 5
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        And the SSVM values are
          | Date       | UnitPrice |
          | 2023-01-15 | 5         |
          | 2023-02-01 | 7         |

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 007: Can edit data date and value SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        When I edit the SSVM 1 entry to date 2023-01-15 and value 50
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        And the SSVM values are
          | Date       | UnitPrice |
          | 2023-01-15 | 50        |
          | 2023-02-01 | 7         |

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 008: Can delete value open SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | UnitPrice | 2023-03-01 | 7.2       |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 3 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        When I remove the SSVM 2 entry from the list
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        And the SSVM values are
          | Date       | UnitPrice |
          | 2023-01-01 | 5         |
          | 2023-03-01 | 7.2       |

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 009: Can add trade data SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 1 trades displayed
        When I add SelectedSecurityViewModel trade data for Barclays-Current with
          | Type  | Date       | UnitPrice | TradeType | NumShares | Costs |
          | Trade | 2023-05-01 | 7         | Buy       | 45        | 2     |
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        Then the SelectedSecurityViewModel has 2 trades displayed
        And the SSVM trade values are
          | Type  | Date       | UnitPrice | TradeType | NumShares | Costs |
          | Trade | 2023-02-01 | 7         | Buy       | 45        | 2     |
          | Trade | 2023-05-01 | 7         | Buy       | 45        | 2     |

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 010: Can edit trade data SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
          | Trade     | 2023-05-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        And the SelectedSecurityViewModel has 2 trades displayed
        When I edit SelectedSecurityViewModel trade data 2 to
          | Type  | Date       | UnitPrice | TradeType | NumShares | Costs |
          | Trade | 2023-06-01 | 8         | Buy       | 45        | 2     |
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        And the SelectedSecurityViewModel has 2 trades displayed
        And the SSVM trade values are
          | Type  | Date       | UnitPrice | TradeType | NumShares | Costs |
          | Trade | 2023-02-01 | 7         | Buy       | 45        | 2     |
          | Trade | 2023-06-01 | 8         | Buy       | 45        | 2     |

        Examples:
          | account  |
          | Pension  |
          | Security |

    Scenario Outline: 011: Can delete trade data SelectedSecurityViewModel
        Given I have a SelectedSecurityViewModel with account <account> and name Barclays-Current and data
          | Type      | Date       | UnitPrice | TradeType | NumShares | Costs |
          | UnitPrice | 2023-01-01 | 5         |           |           |       |
          | UnitPrice | 2023-02-01 | 7         |           |           |       |
          | Trade     | 2023-02-01 | 7         | Buy       | 45        | 2     |
          | Trade     | 2023-05-01 | 7         | Buy       | 45        | 2     |
        And the SelectedSecurityViewModel is brought into focus
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        And the SelectedSecurityViewModel has 2 trades displayed
        When I delete SelectedSecurityViewModel trade data 2
        Then the SelectedSecurityViewModel has 2 unitprices displayed
        And the SelectedSecurityViewModel has 1 trades displayed
        And the SSVM trade values are
          | Type  | Date       | UnitPrice | TradeType | NumShares | Costs |
          | Trade | 2023-02-01 | 7         | Buy       | 45        | 2     |

        Examples:
          | account  |
          | Pension  |
          | Security |