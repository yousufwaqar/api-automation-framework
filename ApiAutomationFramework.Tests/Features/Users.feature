@users
Feature: User API - CRUD Operations
    As a QA Automation Engineer
    I want to test the User API endpoints
    So that I can ensure the API behaves correctly

    Background:
        Given the User API is available

    @smoke @regression
    Scenario: Get all users from page 1 returns successful response
        When I request all users on page 1
        Then the response status code should be 200
        And the response should contain a list of users
        And the response should have pagination metadata
        And the total users count should be greater than 0

    @smoke @regression
    Scenario: Get existing user by valid ID returns correct user data
        When I request user with id 2
        Then the response status code should be 200
        And the response should match the User DTO schema
        And the user first name should be "Janet"
        And the user last name should be "Weaver"
        And the user email should not be empty
        And the user avatar URL should be a valid URL

    @regression
    Scenario Outline: Get users from multiple pages
        When I request all users on page <page>
        Then the response status code should be 200
        And the response should contain users on page <page>

        Examples:
            | page |
            | 1    |
            | 2    |

    @smoke @regression @crud
    Scenario: Create a new user with valid data returns 201
        Given I have a valid create user request with name "John Smith" and job "Engineer"
        When I send a POST request to create the user
        Then the response status code should be 201
        And the response should contain the created user
        And the created user name should be "John Smith"
        And the created user job should be "Engineer"
        And the created user should have an id
        And the created user should have a createdAt timestamp

    @regression @crud
    Scenario: Create user with randomly generated data succeeds
        Given I have a randomly generated create user request
        When I send a POST request to create the user
        Then the response status code should be 201
        And the response should contain the created user

    @regression @crud
    Scenario: Update existing user with valid data returns 200
        Given I have a valid update user request with name "Updated Name" and job "Updated Job"
        When I send a PUT request to update user with id 2
        Then the response status code should be 200
        And the response should contain the updated user
        And the updated user name should be "Updated Name"
        And the updated user job should be "Updated Job"

    @regression @crud
    Scenario: Patch existing user returns 200
        Given I have a valid update user request with name "Patched Name" and job "Patched Job"
        When I send a PATCH request to update user with id 2
        Then the response status code should be 200

    @regression @crud @skipInProduction
    Scenario: Delete existing user returns 204
        When I send a DELETE request for user with id 2
        Then the response status code should be 204
        And the response body should be empty

    @negative @regression
    Scenario: Get user with non-existent ID returns 404
        When I request user with id 9999
        Then the response status code should be 404
        And the response body should be empty or contain error info

    @negative @regression
    Scenario: Get users from invalid page returns empty data
        When I request all users on page 9999
        Then the response status code should be 200
        And the user list should be empty