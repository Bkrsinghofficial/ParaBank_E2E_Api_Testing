Feature: TC_PB_E2E_005 Cross Endpoint Consistency
  As a ParaBank API consumer
  I want all transaction endpoints to return the same data
  So that I can rely on any endpoint interchangeably

  Scenario: All transaction endpoints return consistent data for today
    Given the user logs in with username "john" and password "demo"
    When the user retrieves their account list
    Then the account list is not empty
    When the user deposits 25 to the first account
    Then the deposit returns status 200
    When the user retrieves transactions via all endpoints for today
    Then the date-range and on-date endpoints return the same transaction count
    And the single-transaction endpoint returns the correct transaction
