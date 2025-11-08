# 🚀 QUICK START GUIDE

## Run Everything in 3 Commands

### 1️⃣ Start the API (Terminal 1)
```bash
cd /Users/turbostart-blr-lap0061/Desktop/MyAPI/MyApiApp
dotnet run
```

**Wait for this message:**
```
Now listening on: http://localhost:5067
```

---

### 2️⃣ Run Test Data Generator (Terminal 2 - New Window)
```bash
cd /Users/turbostart-blr-lap0061/Desktop/MyAPI
python3 test_data_generator.py
```

**Press ENTER when prompted**

**What you'll see:**
```
================================================================================
                        CUSTOMER INGESTION TESTS
================================================================================

[TEST 1] Valid Customer - Dubai, CREDIT type
→ POST http://localhost:5067/api/v1/customers/ingest
Response [202]:
  Status: Success
  Message: Customer data received and queued for processing.
  Reference ID: xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
✓ Expected status 202 received

[TEST 2] Valid Customer - Abu Dhabi, High Credit
...
```

**At the end:**
```
================================================================================
                        TEST EXECUTION SUMMARY
================================================================================

Customer Tests:
  ✓ Success (202): 10
  ✗ Failed (400): 5

Item Tests:
  ✓ Success (202): 10
  ✗ Failed (400): 5

Total:
  Total Requests: 30
  Success Rate: 66.7%

================================================================================
NEXT STEPS:
1. Wait 10-15 seconds for background jobs to process
2. Open dashboard.html in your browser
3. Check the metrics and logs
================================================================================
```

---

### 3️⃣ Open Dashboard
```bash
open /Users/turbostart-blr-lap0061/Desktop/MyAPI/dashboard.html
```

**Or:** Double-click `dashboard.html` file

---

## What You'll See

### Dashboard Top Section
```
┌─────────────────────────────────────────────────────────────┐
│  🚀 API Monitoring Dashboard                                │
│  Real-time monitoring of Customer and Item ingestion        │
└─────────────────────────────────────────────────────────────┘

┌──────────────┬──────────────┬──────────────┬──────────────┐
│ Total        │ API Success  │ Processing   │ Pending      │
│ Requests     │ Rate         │ Rate         │ Jobs         │
│     30       │   66.7%      │   100%       │     0        │
└──────────────┴──────────────┴──────────────┴──────────────┘
```

### Domain Breakdown
```
┌────────────────────────────┐ ┌────────────────────────────┐
│ 👥 Customer Domain         │ │ 📦 Item Domain             │
├────────────────────────────┤ ├────────────────────────────┤
│ Total Requests:     15     │ │ Total Requests:     15     │
│ API Success Rate:   66.7%  │ │ API Success Rate:   66.7%  │
│ Processing Rate:    100%   │ │ Processing Rate:    100%   │
│ Pending:            0      │ │ Pending:            0      │
└────────────────────────────┘ └────────────────────────────┘
```

### Status Matrix
```
┌────────────┬────────────┬────────────┬────────────┬────────────┐
│ API        │ API        │ Pending    │ Processed  │ Processing │
│ Success    │ Failure    │ Processing │            │ Errors     │
│ (202)      │ (400)      │            │            │            │
│    20      │    10      │     0      │     20     │     0      │
└────────────┴────────────┴────────────┴────────────┴────────────┘
```

### Logs Table
```
┌──────────────┬──────────┬─────────────────┬────────┬──────────┬────────────────┐
│ Reference ID │ Domain   │ Received Time   │ Status │ Process  │ Actions        │
├──────────────┼──────────┼─────────────────┼────────┼──────────┼────────────────┤
│ abc123...    │ Customer │ 2025-11-08 4:30 │  202   │PROCESSED │ [View JSON]    │
│ def456...    │ Item     │ 2025-11-08 4:31 │  202   │PROCESSED │ [View JSON]    │
│ ghi789...    │ Customer │ 2025-11-08 4:32 │  400   │   N/A    │ [View JSON]    │
└──────────────┴──────────┴─────────────────┴────────┴──────────┴────────────────┘

[All] [Customer] [Item]  ← Filter buttons
```

---

## Verify in Database

Open another terminal and connect to PostgreSQL:

```sql
-- Quick checks
SELECT COUNT(*) FROM log_customer_ingestion;  -- Should be 15
SELECT COUNT(*) FROM log_item_ingestion;      -- Should be 15
SELECT COUNT(*) FROM t_customer;              -- Should be 5
SELECT COUNT(*) FROM t_item;                  -- Should be 6

-- See master data
SELECT * FROM m_region;        -- UAE
SELECT * FROM m_city;          -- DXB, AUH, SHJ
SELECT * FROM m_brand;         -- FRESH, CRISPY, ALMARAI, etc.
SELECT * FROM m_category;      -- JUICE, CHIPS, DAIRY, etc.
```

---

## Troubleshooting

### API won't start
```bash
# Check if port 5067 is in use
lsof -i :5067

# Kill if needed
kill -9 <PID>

# Start again
cd MyApiApp
dotnet run
```

### Dashboard shows nothing
1. Check browser console (F12)
2. Make sure API is running
3. Refresh page (Cmd+R)

### Test script fails
```bash
# Make sure you're in the right directory
cd /Users/turbostart-blr-lap0061/Desktop/MyAPI

# Run again
python3 test_data_generator.py
```

### Background jobs not processing
- Wait 10-20 seconds
- Check API console for:
  ```
  CustomerMasterProcessor background job started.
  ItemMasterProcessor background job started.
  Processing X pending customer records.
  ```

---

## Complete Test Data Summary

The script creates:

**✅ 10 Valid Customers:**
- CUST001: Al Manara Trading (Dubai, CREDIT)
- CUST002: Capital Distributors (Abu Dhabi, CREDIT)
- CUST003: Quick Mart (Dubai, CASH)
- CUST004: Sharjah Trade Center (Sharjah, CREDIT)
- CUST005: Blocked Customer (Dubai, BLOCKED)
- + 5 more variations

**❌ 5 Invalid Customers:**
- Missing required fields
- Invalid email format
- Bad credit logic
- Missing mandatory data

**✅ 10 Valid Items:**
- JUICE001: Orange Juice (3 UOMs)
- SNACK001: Potato Chips (3 UOMs)
- DAIRY001: Fresh Milk (2 UOMs)
- CLEAN001: Floor Cleaner (inactive)
- WATER001: Mineral Water
- CHOCO001: Chocolate Bar
- + 4 more variations

**❌ 5 Invalid Items:**
- Missing required fields
- Invalid isActive value
- Empty UOM list
- Bad conversion factors
- Missing brand/category

---

## Next Steps

1. ✅ Run test script
2. ✅ Open dashboard
3. ✅ Watch metrics update
4. ✅ Filter logs by domain
5. ✅ Click "View JSON" to see payloads
6. ✅ Verify data in PostgreSQL
7. ✅ Test Postman endpoints manually
8. ✅ Add your own test data

---

**You're all set! 🎉**

For detailed information, see `README_TESTING.md`
