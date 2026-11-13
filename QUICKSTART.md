# ParaBank API Test Suite - Quick Start

## 5-Minute Setup

### Step 1: Install .NET 8
```bash
# Download from: https://dotnet.microsoft.com/en-us/download
# Verify installation
dotnet --version   # Should show 8.0.x or higher
```

### Step 2: Restore Project
```bash
# Navigate to project directory
cd ParaBank.API.Tests

# Restore NuGet packages
dotnet restore
```

### Step 3: Build Project
```bash
dotnet build
```

### Step 4: Run All Tests
```bash
dotnet test
```

**Expected Output:**
```
Total tests: 33
Passed: 33
Failed: 0
Skipped: 0
Execution time: ~35 seconds
```

---

## Run Specific Tests

```bash
# Run only High-priority tests
dotnet test --filter "TC_PB_E2E_001|TC_PB_E2E_002|TC_PB_E2E_003|TC_PB_E2E_005|TC_PB_E2E_006"

# Run only date-range filtering tests
dotnet test --filter "TC_PB_E2E_002"

# Run only validation tests
dotnet test --filter "TC_PB_E2E_006"

# Run with verbose output
dotnet test --verbosity=detailed

# Run and generate HTML report
dotnet test --logger "html;LogFileName=report.html"
```

---

## Configuration (If Needed)

Edit `appsettings.json` if using a different ParaBank instance:

```json
{
  "ApiBaseUrl": "https://your-parabank-server/services/bank",
  "DefaultUsername": "your_user",
  "DefaultPassword": "your_password",
  "TimeoutSeconds": 10,
  "RetryCount": 3
}
```

---

## View Live Test Execution

Open another terminal and watch tests run:

```bash
dotnet watch test
```

Tests will re-run on any code change.

---

## Test Results

### What Each Test Does

| Test ID | What It Tests | Time |
|---|---|---|
| **TC_001** | Deposit & withdraw money, then check transaction history | 2s |
| **TC_002** | Date range filtering with boundary conditions | 8s |
| **TC_003** | Money transfer between accounts | 2s |
| **TC_004** | Bill payment tracking | 2s |
| **TC_005** | All transaction endpoints return same data | 2s |
| **TC_006** | API rejects bad input safely | 5s |
| **TC_007** | Invalid operations don't corrupt data | 3s |
| **TC_008** | New account creation works | 2s |
| **TC_009** | Users can't see other users' data | 2s |
| **TC_010** | Response format & performance | 5s |

---

## Example: Running One Feature

```bash
# Run only TC_002 (date filtering)
dotnet test --filter "TC_PB_E2E_002"
```

**Output:**
```
Starting test execution...
  TC_PB_E2E_002: Single date returns only transactions for that date ... PASSED
  TC_PB_E2E_002: Range includes all dates within boundaries ... PASSED
  TC_PB_E2E_002: Boundaries are inclusive ... PASSED
  TC_PB_E2E_002: Empty range returns empty array ... PASSED
  TC_PB_E2E_002: Future date range returns empty array ... PASSED
  TC_PB_E2E_002: Reversed date range handling ... PASSED
  TC_PB_E2E_002: Month and year boundary spanning ... PASSED
  TC_PB_E2E_002: No duplicate transactions ... PASSED

Test Execution Summary
  Total Tests: 8
  Passed: 8
  Failed: 0
  Duration: 8 seconds
```

---

## Troubleshooting

| Problem | Solution |
|---|---|
| **"dotnet: command not found"** | Install .NET SDK from https://dotnet.microsoft.com/download |
| **"Connection refused"** | Check ParaBank is online: `curl https://parabank.parasoft.com/parabank/api-docs/` |
| **"User not found"** | Change username in `appsettings.json` or register at ParaBank |
| **"Tests timeout"** | Increase `TimeoutSeconds` in `appsettings.json` to 30 |

---

## Next Steps

1. **Read Full Documentation:** See `README.md`
2. **Run Specific Features:** Use `--filter` flag
3. **Generate Reports:** Add `--logger "html"` flag
4. **Integrate with CI/CD:** Copy GitHub Actions config from README.md
5. **Extend Tests:** Add new features in `Features/` folder

---

## Project Files

```
ParaBank.API.Tests/
├── Features/                    ← Gherkin scenarios (read-friendly)
│   └── 10 .feature files
├── StepDefinitions/             ← Test implementation (C#)
│   └── 12 .cs files
├── Models/                      ← API response classes
│   └── 4 .cs files
├── Helpers/                     ← API client & utilities
│   └── 3 .cs files
├── Hooks/                       ← Setup/teardown
│   └── Hooks.cs
├── ParaBank.API.Tests.csproj   ← Project config
├── appsettings.json            ← API credentials
└── README.md                    ← Full documentation
```

---

## Key Commands Reference

```bash
# Build
dotnet build

# Run all tests
dotnet test

# Run specific test
dotnet test --filter "TC_PB_E2E_001"

# Verbose output
dotnet test --verbosity=detailed

# Generate HTML report
dotnet test --logger "html;LogFileName=test-results.html"

# Watch mode (auto-rerun on changes)
dotnet watch test

# List all tests
dotnet test --list-tests

# Stop on first failure
dotnet test --stop-on-failure
```

---

## Sample Test Scenario (Gherkin)

From `Features/TC_PB_E2E_001_DepositWithdrawalTransactions.feature`:

```gherkin
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
```

---

## CI/CD Integration (1 Minute)

### GitHub Actions
Create `.github/workflows/tests.yml`:
```yaml
name: Tests
on: [push]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - run: dotnet test
```

Push to GitHub, tests run automatically!

---

## Support

- **Reqnroll:** https://reqnroll.net/
- **Assertions:** https://fluentassertions.com/
- **ParaBank API:** https://parabank.parasoft.com/

---

That's it! You now have a **fully automated test suite** for the ParaBank API. 🚀
