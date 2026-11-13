Feature: TC_PB_E2E_010 Contract And Performance
  As a ParaBank API consumer
  I want the API to meet contract and performance standards
  So that it integrates reliably with client applications

  Background:
    Given the user logs in with username "john" and password "demo"
    And the user retrieves their account list

  Scenario: Account list endpoint responds within SLA
    When the user retrieves the account list and measures response time
    Then the response time is under 5 seconds
    And the response is valid JSON
    And the response status is successful

  Scenario: Account list response contains required contract fields
    When the user retrieves the account list and measures response time
    Then the account list response contains id, customerId, type and balance fields

  Scenario: Transaction endpoint responds within SLA
    When the user retrieves transactions and measures response time
    Then the response time is under 5 seconds
    And the response is valid JSON

  Scenario: Account list endpoint consistently responds under load
    When the user retrieves the account list and measures response time
    Then the response time is under 10 seconds

  Scenario: Transaction list returns valid JSON contract
    When the user retrieves transactions and measures response time
    Then the response status is successful
    And the response is valid JSON
