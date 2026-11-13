Feature: TC_PB_E2E_009 Data Isolation
  As a ParaBank security officer
  I want users to only see their own data
  So that customer financial data remains private

  Scenario: User cannot access another user's account data
    Given the user logs in with username "john" and password "demo"
    And the user retrieves their account list
    And a second user exists with username "jane" and password "demo"
    Then the customer IDs of the two users are different
    When the first user tries to access the second user's accounts
    Then the API does not return the other user's private data
