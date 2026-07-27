@auth
Feature: Authentication API
    As a QA Automation Engineer
    I want to test the Authentication endpoints
    So that I can verify authentication works correctly

    @smoke @auth
    Scenario: Login with valid credentials returns token
        Given I have valid login credentials
        When I send a login request
        Then the response status code should be 200
        And the response should contain an authentication token
        And the token should not be empty

    @negative @auth
    Scenario: Login with invalid password returns 400
        Given I have invalid login credentials
        When I send a login request
        Then the response status code should be 400
        And the response should contain an error message

    @negative @auth
    Scenario: Login without password field returns 400
        Given I have login credentials with missing password
        When I send a login request
        Then the response status code should be 400
        And the error message should indicate missing password

    @auth @regression
    Scenario: Login and use token for authenticated request
        Given I successfully login with valid credentials
        And I have the authentication token
        When I request users with the authentication token
        Then the response status code should be 200