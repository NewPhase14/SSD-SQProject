Feature: DeleteListing
In order to remove a listing that I no longer want to sell
As a seller
I want to delete my listing

@delete-listing
Scenario: Owner deletes a listing successfully
    Given the "listing-1" with "user-1"
    When the user presses delete on listing
    Then the listing should be deleted successfully

@delete-listing
Scenario: Non-owner cannot delete a listing
    Given the "listing-1" with "user-1"
    When a different user presses delete on listing
    Then the delete should be rejected