Feature: TC_PB_E2E_004 Bill Payment
  As a ParaBank user
  I want to pay bills from my account
  So that payments are recorded and traceable

  Scenario: Bill payment is processed and returns correct payee details
    Given the user logs in with username "john" and password "demo"
    When the user retrieves their account list
    Then the account list is not empty
    When the user pays a bill of 75 to payee "Electric Company"
    Then the bill payment returns status 200
    And the bill payment result contains the payee name
    And the bill payment amount matches the requested amount
