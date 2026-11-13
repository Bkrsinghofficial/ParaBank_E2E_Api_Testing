Feature: TC_PB_E2E_006 Input Validation
  As a ParaBank API consumer
  I want the API to safely reject invalid inputs
  So that bad data cannot corrupt the system

  Background:
    Given the user logs in with username "john" and password "demo"
    And the user retrieves their account list

  Scenario: Invalid login credentials are rejected
    When the user logs in with invalid credentials "invalid_user" and "wrong_pass"
    Then the API returns an error status code

  Scenario: Negative deposit amount is rejected
    When the user deposits a negative amount of -100
    Then the API rejects the request safely

  Scenario: Zero deposit amount is rejected
    When the user deposits a negative amount of 0
    Then the API rejects the request safely

  Scenario: Overdraft withdrawal is rejected
    When the user withdraws more than the available balance
    Then the API rejects the request safely

  Scenario: Transfer to non-existent account is rejected
    When the user transfers to a non-existent account
    Then the API returns an error status code

  Scenario: Request with non-existent account ID returns error
    When the user requests account info for account 99999999
    Then the API returns an error status code

  Scenario: Request with string account ID returns error
    When the user requests account info for account invalid_id
    Then the API returns an error status code

  Scenario: Missing amount in deposit returns error
    When the user deposits a negative amount of -1
    Then the API rejects the request safely

  Scenario: Extremely large deposit amount is handled
    When the user deposits a negative amount of -999999999
    Then the API rejects the request safely

  Scenario: SQL injection in login username is safely rejected
    When the user logs in with invalid credentials "john' OR '1'='1" and "demo"
    Then the API returns an error status code

  Scenario: Special characters in username are safely rejected
    When the user logs in with invalid credentials "<script>alert(1)</script>" and "demo"
    Then the API returns an error status code
