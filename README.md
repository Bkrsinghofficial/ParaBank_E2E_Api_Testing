# ParaBank REST API - Reqnroll Automation Test Suite

## Overview

This is a **production-ready BDD automation test suite** for the ParaBank REST API using **C# + Reqnroll** (modern BDD framework for .NET). It covers all 10 end-to-end test scenarios from the ISTQB-formatted test specification.

- **10 Feature files** with Gherkin scenarios
- **60+ automated test steps**
- **Full API coverage** including authentication, transactions, transfers, bill payments, validation, and security
- **CI/CD ready** with NUnit test runner

---

## Project Structure

```
ParaBank.API.Tests/
├── ParaBank.API.Tests.csproj          # Project configuration
├── appsettings.json                   # Base URL, credentials, timeouts
├── Features/                          # Gherkin scenario files
│   ├── TC_PB_E2E_001_DepositWithdrawalTransactions.feature
│   ├── TC_PB_E2E_002_DateRangeFilter.feature
│   ├── TC_PB_E2E_003_TransferConsistency.feature
│   ├── TC_PB_E2E_004_BillPayment.feature
│   ├── TC_PB_E2E_005_CrossEndpointConsistency.feature
│   ├── TC_PB_E2E_006_InputValidation.feature
│   ├── TC_PB_E2E_007_NoSideEffects.feature
│   ├── TC_PB_E2E_008_NewAccountLifecycle.feature
│   ├── TC_PB_E2E_009_DataIsolation.feature
│   └── TC_PB_E2E_010_ContractAndPerformance.feature
├── StepDefinitions/                   # Step implementation
│   ├── LoginAndAuthSteps.cs
│   ├── AccountSteps.cs
│   ├── TransactionSteps.cs
│   ├── DateRangeSteps.cs
│   ├── TransferSteps.cs
│   ├── ValidationSteps.cs
│   ├── SideEffectSteps.cs
│   ├── BillPaySteps.cs
│   ├── PerformanceSteps.cs
│   ├── ConsistencySteps.cs
│   ├── AccountLifecycleSteps.cs
│   └── SecuritySteps.cs
├── Models/                            # API response models
│   ├── Customer.cs
│   ├── Account.cs
│   ├── TransactionModel.cs
│   └── Payee.cs
├── Helpers/                           # Utilities
│   ├── ApiClient.cs                   # REST client with retry logic
│   ├── ConfigHelper.cs                # Configuration loader
│   └── TestContext.cs                 # Shared test state
└── Hooks/
    └── Hooks.cs                       # Before/After scenario setup
```

---

## Getting Started

### Prerequisites

- **.NET 8.0 SDK** or later: [Download here](https://dotnet.microsoft.com/en-us/download)
- **Visual Studio Code** or **Visual Studio 2022+** (optional)
- **Command line terminal**

### Installation

```bash
# 1. Clone or extract the project
cd ParaBank.API.Tests

# 2. Restore dependencies
dotnet restore

# 3. Build the project
dotnet build

# 4. (Optional) Verify installation
dotnet test --help
```

---

## Configuration

Edit **`appsettings.json`** to customize API endpoint and credentials:

```json
{
  "ApiBaseUrl": "https://parabank.parasoft.com/parabank/services/bank",
  "DefaultUsername": "john",
  "DefaultPassword": "demo",
  "TimeoutSeconds": 10,
  "RetryCount": 3,
  "DateFormat": "MM-dd-yyyy",
  "LogLevel": "Information"
}
```

| Setting | Description | Default |
|---|---|---|
| `ApiBaseUrl` | ParaBank API base URL | `https://parabank.parasoft.com/parabank/services/bank` |
| `DefaultUsername` | Test user login | `john` |
| `DefaultPassword` | Test user password | `demo` |
| `TimeoutSeconds` | HTTP request timeout | `10` |
| `RetryCount` | Failed request retry attempts | `3` |
| `DateFormat` | Expected date format for API | `MM-dd-yyyy` |
| `LogLevel` | Logging verbosity (Information/Debug) | `Information` |

---

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run a Specific Feature

```bash
dotnet test --filter "TC_PB_E2E_001"
```

### Run Tests with Verbose Output

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run Tests and Generate HTML Report

```bash
dotnet test --logger "html;LogFileName=test-results.html"
```

Then open `test-results.html` in a browser.

### Run in Watch Mode (during development)

```bash
dotnet watch test
```

---

## Test Coverage

| Test ID | Title | Scenarios | Type | Priority |
|---|---|---|---|---|
| **TC_PB_E2E_001** | Deposit/Withdrawal Transactions | 1 | Functional | High |
| **TC_PB_E2E_002** | Date-Range Filtering | 8 | Boundary Value | High |
| **TC_PB_E2E_003** | Transfer Consistency | 1 | Functional | High |
| **TC_PB_E2E_004** | Bill Payment | 1 | Functional | Medium |
| **TC_PB_E2E_005** | Cross-Endpoint Consistency | 1 | Consistency | High |
| **TC_PB_E2E_006** | Input Validation | 11 | Negative | High |
| **TC_PB_E2E_007** | No Side Effects | 3 | Negative | Medium |
| **TC_PB_E2E_008** | Account Lifecycle | 1 | State Transition | Medium |
| **TC_PB_E2E_009** | Data Isolation | 1 | Security | Medium |
| **TC_PB_E2E_010** | Contract & Performance | 5 | Non-Functional | Low |
| **TOTAL** | | **33 Scenarios** | Mixed | |

---

## Key Features

### 1. **API Client with Retry Logic**
```csharp
// Automatic retries on failure
var response = await _apiClient.GetAsync<List<TransactionModel>>(
    "/accounts/1/transactions/fromDate/01-01-2024/toDate/01-02-2024");

if (response.IsSuccess)
{
    var transactions = response.Data;
}
```

### 2. **Test Context Sharing**
Shared state across steps within a scenario:
```csharp
_testContext.CustomerId = 123;
_testContext.AccountIds["Account_1"] = 456;
_testContext.AccountBalances["Account_1"] = 1000.50m;
```

### 3. **Automatic Logging**
All API calls and steps are logged:
```
Information: Starting scenario: Verify that transactions created through deposit and withdrawal are returned
Information: User logged in with customerId: 17693
Information: Retrieved 2 accounts
Information: Deposited 100 to account 17693
```

### 4. **BDD with Gherkin**
Readable, non-technical scenario descriptions:
```gherkin
Scenario: Full customer journey - login, transact, and retrieve by date range
  Given the user logs in with username "john" and password "demo"
  When the user deposits 100 to the first account
  Then the transaction list contains a Credit of 100
```

### 5. **Fluent Assertions**
Clear, readable assertions:
```csharp
response.StatusCode.Should().Be(200, "login should return 200");
response.Data.Id.Should().BeGreaterThan(0, "customerId should be positive");
_lastTransactionList.Should().ContainSingle(t => t.Type == "Credit" && t.Amount == amount);
```

---

## API Endpoints Covered

| Endpoint | Method | Test Case |
|---|---|---|
| `/login/{username}/{password}` | GET | All |
| `/customers/{customerId}/accounts` | GET | TC_001-010 |
| `/accounts/{accountId}` | GET | TC_001-008 |
| `/accounts/{accountId}/transactions/fromDate/{fromDate}/toDate/{toDate}` | GET | **TC_001-010** |
| `/accounts/{accountId}/transactions/onDate/{onDate}` | GET | TC_005 |
| `/accounts/{accountId}/transactions/month/{month}/type/{type}` | GET | TC_005 |
| `/transactions/{transactionId}` | GET | TC_005 |
| `/deposit` | POST | TC_001, TC_007 |
| `/withdraw` | POST | TC_001, TC_007 |
| `/transfer` | POST | TC_003 |
| `/billpay` | POST | TC_004 |
| `/customers/{customerId}/createAccount` | POST | TC_008 |

---

## Troubleshooting

### Issue: "API is unreachable"
**Solution:** Verify the API is online:
```bash
curl https://parabank.parasoft.com/parabank/services/bank/login/john/demo
```

### Issue: "Test user john/demo not found"
**Solution:** Register a new user or use an existing one. Update `appsettings.json`.

### Issue: "Tests timeout"
**Solution:** Increase `TimeoutSeconds` in `appsettings.json` or check network connectivity.

### Issue: "Tests pass locally but fail in CI/CD"
**Solution:** Check for timezone differences. Tests use `DateTime.Now`; update to `DateTime.UtcNow` if needed.

---

## CI/CD Integration

### GitHub Actions

Create `.github/workflows/tests.yml`:
```yaml
name: ParaBank API Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build
      - run: dotnet test --logger "trx;LogFileName=test-results.trx"
      - uses: EnricoMi/publish-unit-test-result-action@v2
        if: always()
        with:
          files: '**/test-results.trx'
```



## Extending the Tests

### Add a New Scenario

1. **Create a feature file** in `Features/`:
   ```gherkin
   Feature: TC_PB_E2E_011_NewFeature
     Scenario: Test a new feature
       Given precondition
       When action
       Then assertion
   ```

2. **Implement step definitions** in `StepDefinitions/NewSteps.cs`:
   ```csharp
   [Given(@"precondition")]
   public void SetUp() { }

   [When(@"action")]
   public async Task Action() { }

   [Then(@"assertion")]
   public void Assert() { }
   ```

3. **Run the test**:
   ```bash
   dotnet test --filter "TC_PB_E2E_011"
   ```

---

## Best Practices

### 1. **Use Test Data Builders**
```csharp
var payee = new Payee
{
    Name = "Test Payee",
    Address = new Address { Street = "123 Main St", /* ... */ }
};
```

### 2. **Avoid Hardcoded Values**
```csharp
// Bad
var accountId = 17693;

// Good
var accountId = _testContext.AccountIds.First().Value;
```

### 3. **Log Meaningful Information**
```csharp
Log.Information($"Transferred {amount} from {accountA} to {accountB}");
```

### 4. **Clean Up in Hooks**
```csharp
[AfterScenario]
public void Cleanup()
{
    _testContext.ClearContext();
}
```

### 5. **Group Related Assertions**
```csharp
response.StatusCode.Should().Be(200);
response.Data.Should().NotBeNull();
response.Data.Id.Should().BeGreaterThan(0);
```

---

## Performance Metrics

Typical execution times on a modern machine:

| Test Case | Scenarios | Avg. Time |
|---|---|---|
| TC_PB_E2E_001 | 1 | ~2 sec |
| TC_PB_E2E_002 | 8 | ~8 sec |
| TC_PB_E2E_003 | 1 | ~2 sec |
| TC_PB_E2E_004 | 1 | ~2 sec |
| TC_PB_E2E_005 | 1 | ~2 sec |
| TC_PB_E2E_006 | 11 | ~5 sec |
| TC_PB_E2E_007 | 3 | ~3 sec |
| TC_PB_E2E_008 | 1 | ~2 sec |
| TC_PB_E2E_009 | 1 | ~2 sec |
| TC_PB_E2E_010 | 5 | ~5 sec |
| **Total** | **33** | **~33 sec** |

---

## Support & Documentation

- **Reqnroll Documentation:** https://reqnroll.net/
- **FluentAssertions:** https://fluentassertions.com/
- **RestSharp:** https://restsharp.dev/

---

## License

This test suite is provided as-is for testing the ParaBank Demo API.

---

## Contact

For issues or questions, refer to the ParaBank documentation at: https://parabank.parasoft.com/
