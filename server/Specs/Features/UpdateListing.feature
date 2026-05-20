Feature: UpdateListing
In order to keep my listing up-to-date
As a user
I want to update an existing listing

@update-listing
Scenario: Owner updates a listing successfully
    Given an existing listing with id "listing-1" owned by "user-1"
    And an update request:
        | id        | categoryId | condition | title      | description      | price  | status |
        | listing-1 |          2 | Used      | Updated PC | Slightly used PC | 799.99 | Active |
    When the user "user-1" updates the listing
    Then the listing should be updated successfully

Scenario: Non-owner cannot update a listing
    Given an existing listing with id "listing-2" owned by "user-2"
    And an update request:
        | id        | categoryId | condition | title      | description | price  | status |
        | listing-2 |          3 | New       | Other Item | A nice item | 199.99 | Active |
    When the user "user-1" updates the listing
    Then the user should see an update error "You are not the owner of this listing"