Feature: TC_PB_E2E_003 Transfer Consistency
  As a ParaBank user
  I want to transfer money between my accounts
  So that balances are updated consistently on both sides

  Scenario: Money transfer reduces source and increases target by exact amount
    Given the user logs in with username "john" and password "demo"
    When the user retrieves their account list
    Then the account list is not empty
    When the user transfers 50 from the first account to the second account
    Then the transfer returns status 200
    And the source account balance is reduced by the transfer amount
    And the target account balance is increased by the transfer amount
