Feature: CreateListing
In order to sell an item
As a user
I want to create a listing with valid details

@create-listing
Scenario: User creates a listing with valid details
    Given a listing create request:
        | title  | categoryId | condition | description        | price  | status |
        | Laptop |          1 | New       | A brand new laptop | 999.99 | Active |
    When the user creates the listing
    Then the listing should be created successfully