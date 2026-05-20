Feature: CreateListing
In order to sell an item
As a user
I want to create a listing with valid details

@create-listing
Scenario Outline: User creates a listing with valid details
    Given a listing create request with title <title>, categoryId <categoryId>, condition <condition>, description <description>, price <price>, status <status>
    When the user creates the listing
    Then the listing should be created successfully

Examples:
    | title  | categoryId | condition | description        | price  | status |
    | Laptop |          1 | New       | A brand new laptop | 999.99 | Active |
    | Phone  |          2 | Used      | Gently used phone  | 199.99 | Active |