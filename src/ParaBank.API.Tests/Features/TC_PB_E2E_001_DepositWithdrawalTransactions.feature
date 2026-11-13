Feature: TC_PB_E2E_001 Deposit Withdrawal Transactions
  As a ParaBank user
  I want to deposit and withdraw money
  So that my transaction history accurately reflects my activity

  Scenario: Full customer journey - login, transact, and retrieve by date range
    Given the user logs in with username "john" and password "demo"
    When the user retrieves their account list
    Then the account list is not empty
    And the customerId is saved for later use

    When the user deposits 100 to the first account
    Then the deposit returns status 200
    And the user withdraws 40 from the first account
    Then the withdrawal returns status 200

    When the user retrieves the account balance
    Then the balance should equal the starting balance plus 100 minus 40

    When the user retrieves transactions for today
    Then the response status is 200
    And the transaction list contains a Credit of 100
    And the transaction list contains a Debit of 40
