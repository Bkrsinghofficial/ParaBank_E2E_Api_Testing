Feature: TC_PB_E2E_007 No Side Effects
  As a ParaBank user
  I want failed operations to not modify my account data
  So that my balance and history remain accurate after errors

  Background:
    Given the user logs in with username "john" and password "demo"
    And the user retrieves their account list

  Scenario: Failed overdraft withdrawal leaves balance unchanged
    When the user records the current balance
    And the user attempts an invalid withdrawal
    And the user checks the balance again
    Then the balance should be unchanged

  Scenario: Account data integrity is maintained after failed operation
    When the user records the current balance
    And the user attempts an invalid withdrawal
    Then the account data is not corrupted

  Scenario: Multiple failed operations do not accumulate errors
    When the user records the current balance
    And the user attempts an invalid withdrawal
    And the user attempts an invalid withdrawal
    And the user checks the balance again
    Then the balance should be unchanged
