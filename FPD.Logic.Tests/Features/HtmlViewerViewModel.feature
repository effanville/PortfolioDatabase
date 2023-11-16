Feature: HtmlViewerViewModel
Interactions for the HtmlViewer

    Scenario: 001 Create the view and navigate to it
        Given I have a HtmlViewerViewModel with name Bob and no webpage
        And the HtmlViewerViewModel is brought into focus
        Then the name is Bob
        And there is no url selected

    Scenario: 002 Create the view with file and navigate to it
        Given I have a HtmlViewerViewModel with name Bob and webpage http://www.google.com
        And the HtmlViewerViewModel is brought into focus
        Then the name is Bob
        And the url is http://www.google.com/

    Scenario: 003 Create the view with file and then change location
        Given I have a HtmlViewerViewModel with name Bob and webpage http://www.google.com
        And the HtmlViewerViewModel is brought into focus
        Then the name is Bob
        And the url is http://www.google.com/
        When the url is changed to http://www.yahoo.com
        Then the url is http://www.yahoo.com/

    Scenario: 004 Create the view with file and then change location to invalid url
        Given I have a HtmlViewerViewModel with name Bob and webpage http://www.google.com
        And the HtmlViewerViewModel is brought into focus
        Then the name is Bob
        And the url is http://www.google.com/
        When the url is changed to "hello-tom"
        Then the url is http://www.google.com/