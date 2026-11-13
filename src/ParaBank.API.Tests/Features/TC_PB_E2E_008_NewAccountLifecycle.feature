Feature: TC_PB_E2E_008 New Account Lifecycle
  As a ParaBank user
  I want to create new bank accounts
  So that they function correctly from the moment of creation

  Scenario: New savings account creation and basic operations
    Given the user logs in with username "john" and password "demo"
    When the user retrieves their account list
    Then the account list is not empty
    When the user creates a savings account funded by the first account
    Then the new account appears in the customer's account list
    And deposits can be made to the new account
    And the new account balance reflects the deposit
