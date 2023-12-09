Feature: ReportingViewModel
Ensure the reporting view model works as expected

    Scenario Outline: 001: Can successfully initialise
        Given I have a ReportingViewModel with default level <level> and no reports
        And the ReportingViewModel is brought into focus
        Then I can see that the RVM has 0 reports

        Examples:
          | level    |
          | Critical |
          | Useful   |
          | Detailed |

    Scenario: 002: Can successfully add a report
        Given I have a ReportingViewModel with default level Useful and no reports
        And the ReportingViewModel is brought into focus
        Then I can see that the RVM has 0 reports
        When reports are added to the RVM with data
          | Severity | Type  | Location | Message        |
          | Useful   | Error | Unknown  | Is this added? |
        Then I can see that the RVM has 1 reports
        And the reports in the RVM display have data
          | Severity | Type  | Location | Message        |
          | Useful   | Error | Unknown  | Is this added? |
        And the reports in the RVM have data
          | Severity | Type  | Location | Message        |
          | Useful   | Error | Unknown  | Is this added? |

    Scenario Outline: 003: Can successfully add a report and filter
        Given I have a ReportingViewModel with default level <level> and no reports
        And the ReportingViewModel is brought into focus
        Then I can see that the RVM has 0 reports
        When reports are added to the RVM with data
          | Severity | Type  | Location | Message        |
          | Useful   | Error | Unknown  | Is this added? |
        Then I can see that the RVM display has <numReports> reports
        Then I can see that the RVM has 1 reports

        Examples:
          | level    | numReports |
          | Critical | 0          |
          | Useful   | 1          |
          | Detailed | 1          |

    Scenario: 004: Can successfully clear reports
        Given I have a ReportingViewModel with default level Useful and reports
          | Severity | Type  | Location | Message             |
          | Useful   | Error | Unknown  | Is this added?      |
          | Useful   | Error | Unknown  | Is this also added? |
        And the ReportingViewModel is brought into focus
        Then I can see that the RVM has 2 reports
        When reports are cleared from the RVM
        Then I can see that the RVM has 0 reports
        And I can see that the RVM display has 0 reports 

    Scenario: 005: Can successfully clear specific report
        Given I have a ReportingViewModel with default level Useful and reports
          | Severity | Type  | Location | Message             |
          | Useful   | Error | Unknown  | Is this added?      |
          | Useful   | Error | Unknown  | Is this also added? |
        And the ReportingViewModel is brought into focus
        Then I can see that the RVM has 2 reports
        When the 1 report is cleared from the RVM
        Then I can see that the RVM has 1 reports
        And the reports in the RVM display have data
          | Severity | Type  | Location | Message             |
          | Useful   | Error | Unknown  | Is this also added? |
        And the reports in the RVM have data
          | Severity | Type  | Location | Message             |
          | Useful   | Error | Unknown  | Is this also added? |