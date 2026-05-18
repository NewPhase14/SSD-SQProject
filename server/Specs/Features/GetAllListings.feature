Feature: GetAllListings
In order to browse items
As any user
I want to retrieve all listings

@get-all-listings
Scenario: Returns all listings when present
    Given the repository has the following listings:
        | id        | userId | categoryId | condition | title  | description | price | status |
        | listing-1 | user-1 |          1 | New       | Item A | desc A      | 10.00 | Active |
        | listing-2 | user-2 |          2 | Used      | Item B | desc B      | 20.00 | Active |
    When the user requests all listings
    Then the user should receive 2 listings

Scenario: No listings returns a not found error
    Given the repository has no listings
    When the user requests all listings
    Then the user should see "No listings found"