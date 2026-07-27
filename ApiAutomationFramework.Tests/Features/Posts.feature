@posts
Feature: Posts API - JSONPlaceholder
    As a QA Automation Engineer
    I want to test the Posts API endpoints
    So that I can ensure CRUD operations work correctly

    Background:
        Given the Posts API is available

    @smoke @posts
    Scenario: Get all posts returns successful response
        When I request all posts
        Then the response status code should be 200
        And the response should contain a list of posts
        And the posts list should have 100 items

    @smoke @posts
    Scenario: Get single post by valid ID
        When I request post with id 1
        Then the response status code should be 200
        And the post should have a valid id
        And the post title should not be empty
        And the post body should not be empty
        And the post should belong to a user

    @posts @regression
    Scenario: Get posts by user ID returns only that users posts
        When I request posts by user with id 1
        Then the response status code should be 200
        And all returned posts should belong to user 1
        And the posts list should not be empty

    @posts @crud @regression
    Scenario: Create a new post with valid data
        Given I have a valid create post request
        When I send a POST request to create the post
        Then the response status code should be 201
        And the created post should have an id
        And the created post title should match the request title

    @posts @crud @regression
    Scenario: Update an existing post
        Given I have a valid update post request
        When I send a PUT request to update post with id 1
        Then the response status code should be 200

    @posts @crud @regression @skipInProduction
    Scenario: Delete a post
        When I send a DELETE request for post with id 1
        Then the response status code should be 200

    @negative @posts
    Scenario: Get post with non-existent ID returns 404
        When I request post with id 99999
        Then the response status code should be 404