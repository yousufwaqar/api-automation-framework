@advanced @patterns
Feature: Advanced Design Patterns Demonstration
    As a QA Automation Engineer
    I want to demonstrate advanced design patterns
    So that the framework supports complex enterprise scenarios

    Background:
        Given the User API is available

    # ═══════════════════════════════════════════════════
    # FACTORY PATTERN SCENARIOS
    # ═══════════════════════════════════════════════════

    @factory @smoke
    Scenario: Create user using Factory Pattern with valid data
        Given I create a valid user using the test data factory
        When I send a POST request to create the user using the prepared request
        Then the response status code should be 201
        And the response should contain the created user

    @factory @regression
    Scenario: Create user with special characters using Factory Pattern
        Given I create a user with special characters using the factory
        When I send a POST request to create the user using the prepared request
        Then the response status code should be 201
        And the response should contain the created user

    @factory @negative @regression
    Scenario: Create user with XSS payload is rejected by API
        Given I create a user with XSS payload using the factory
        When I send a POST request to create the user using the prepared request
        Then the response status code should be 403
        And the response body should be empty or contain error info

    # ═══════════════════════════════════════════════════
    # FACADE PATTERN SCENARIOS
    # ═══════════════════════════════════════════════════

    @facade @regression
    Scenario: Execute complete user lifecycle using Facade Pattern
        When I execute the full user lifecycle through the facade
        Then all lifecycle operations should be successful
        And the create operation should return status 201
        And the update operation should return status 200

    @facade @regression
    Scenario: Login and fetch users using Facade Pattern
        When I login and fetch users through the facade with valid credentials
        Then the login should be successful
        And the users list should not be empty

    # ═══════════════════════════════════════════════════
    # BUILDER PATTERN SCENARIOS
    # ═══════════════════════════════════════════════════

    @builder @smoke
    Scenario: Create admin user using Builder Pattern
        Given I build an admin user request with name "John Admin"
        When I send a POST request to create the user using the prepared request
        Then the response status code should be 201
        And the created user job should be "Administrator"

    @builder @regression
    Scenario: Create user with fluent builder chain
        Given I build a user request with random name and job "Senior Engineer"
        When I send a POST request to create the user using the prepared request
        Then the response status code should be 201
        And the created user job should be "Senior Engineer"

    # ═══════════════════════════════════════════════════
    # SELECTOR PATTERN SCENARIOS
    # ═══════════════════════════════════════════════════

    @selector @regression
    Scenario: Filter users by email domain using Selector Pattern
        When I request all users on page 1
        Then I should be able to filter users by "reqres.in" email domain
        And the filtered users list should contain at least 1 user

    @selector @posts @regression
    Scenario: Select posts with multiple criteria using Selector Pattern
        Given the Posts API is available
        When I request all posts
        Then I should be able to select posts by user 1
        And the selected posts should all belong to user 1