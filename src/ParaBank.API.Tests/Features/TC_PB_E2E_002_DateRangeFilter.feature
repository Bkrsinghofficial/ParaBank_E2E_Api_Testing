Feature: TC_PB_E2E_002 Date Range Filtering
  As a ParaBank user
  I want to filter transactions by date range
  So that I can review activity within a specific period

  Background:
    Given the user logs in with username "john" and password "demo"
    And the user retrieves their account list

  Scenario: Single date returns only transactions for that date
    When the user retrieves transactions for today
    Then the response status is 200
    And the transaction list is not empty

  Scenario: Range includes all dates within boundaries
    When the user retrieves transactions from "01-01-2024" to "12-31-2024"
    Then all transactions fall within the requested date range

  Scenario: Boundaries are inclusive
    When the user retrieves transactions from "01-01-2024" to "01-01-2024"
    Then the date range boundaries are inclusive

  Scenario: Empty range returns empty array
    When the user retrieves transactions from "01-01-2000" to "01-02-2000"
    Then the transaction list is empty

  Scenario: Future date range returns empty array
    When the user retrieves transactions from "01-01-2099" to "12-31-2099"
    Then the transaction list is empty

  Scenario: Reversed date range handling
    When the user retrieves transactions from "12-31-2024" to "01-01-2024"
    Then the transaction list is empty

  Scenario: Month and year boundary spanning
    When the user retrieves transactions from "12-31-2023" to "01-01-2024"
    Then all transactions fall within the requested date range

  Scenario: No duplicate transactions
    When the user retrieves transactions for today
    Then no duplicate transactions are returned
