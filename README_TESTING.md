# 🚀 Complete Testing & Dashboard Guide

## Quick Start (3 Steps)

### Step 1: Start the API
```bash
cd /Users/turbostart-blr-lap0061/Desktop/MyAPI/MyApiApp
dotnet run
```

**Wait for:** `Now listening on: http://localhost:5067`

---

### Step 2: Run Test Data Generator
Open a **new terminal** and run:

```bash
cd /Users/turbostart-blr-lap0061/Desktop/MyAPI
python3 test_data_generator.py
```

**What it does:**
- Sends 10 valid customer requests (will be accepted with 202)
- Sends 5 invalid customer requests (will fail with 400)
- Sends 10 valid item requests (will be accepted with 202)
- Sends 5 invalid item requests (will fail with 400)
- **Total: 30 requests** to populate your database

**You'll see:**
- ✓ Green for successful requests (202)
- ✗ Red for validation failures (400)
- Detailed request/response info
- Summary statistics at the end

---

### Step 3: Open Dashboard
```bash
open /Users/turbostart-blr-lap0061/Desktop/MyAPI/dashboard.html
```

**Or manually:** Double-click `dashboard.html`

---

## What You'll See in the Dashboard

### 📊 Metrics Section (Top)
- **Total Requests**: ~30 (from test script)
- **API Success Rate**: ~66% (20 success / 30 total)
- **Processing Rate**: Will increase from 0% → 100% as background jobs process
- **Pending Jobs**: Will decrease from 20 → 0 as jobs complete

### 👥📦 Domain Breakdown
**Customer Domain:**
- Total: 10 requests
- API Success: 66.7% (10 valid / 15 total)
- Processing: Watch it go PENDING → PROCESSED

**Item Domain:**
- Total: 10 requests
- API Success: 66.7% (10 valid / 15 total)
- Processing: Watch it go PENDING → PROCESSED

### 📈 Status Matrix
- **API Success (202)**: 20 (green)
- **API Failure (400)**: 10 (red)
- **Pending**: 20 → 0 (blue, decreasing)
- **Processed**: 0 → 20 (green, increasing)
- **Errors**: 0 (hopefully!)

### 📋 Logs Table
Click filter buttons to view:
- **All**: Shows all 30 requests
- **Customer**: Shows 15 customer requests
- **Item**: Shows 15 item requests

Each log shows:
- Reference ID (UUID)
- Domain (Customer/Item)
- Received Time
- API Status (202 badge or 400 badge)
- Processing Status (PENDING → PROCESSED)
- Validation Errors (for failed requests)
- **View JSON** button (click to see full request payload)

---

## Test Cases Included

### ✅ Valid Customer Tests (10)
1. **Dubai CREDIT** - Full details, high credit
2. **Abu Dhabi CREDIT** - Different region, 90-day terms
3. **CASH type** - Zero credit (valid for CASH)
4. **Sharjah** - Another region
5. **Blocked Customer** - isBlocked=true
6-9. Additional variations
10. **Update Existing** - Updates CUST001

### ❌ Invalid Customer Tests (5)
1. **Missing fields** - Only code & name
2. **Bad email** - Invalid format
3. **Bad credit logic** - CREDIT type with 0 credit
4. **Missing email** - Required field
5. **Missing contact** - Required field

### ✅ Valid Item Tests (10)
1. **Juice** - 3 UOMs (BTL, CASE, PALLET)
2. **Snacks** - 3 UOMs (PKT, BOX, CARTON)
3. **Dairy** - 2 UOMs
4. **Cleaning** - isActive="0"
5. **Water** - Multiple UOMs
6. **Chocolate** - Simple product
7-9. Additional variations
10. **Update Existing** - Updates JUICE001

### ❌ Invalid Item Tests (5)
1. **Missing fields** - Only code & name
2. **Bad isActive** - Value "YES" instead of "0"/"1"
3. **Empty UOM list** - Required array
4. **Bad conversion** - Zero/negative factors
5. **Missing brand** - Required field

---

## Verify Data in Database

After running the test script, check PostgreSQL:

```sql
-- Check staging logs
SELECT log_id, status, process_status, http_status
FROM log_customer_ingestion
ORDER BY request_time DESC;

SELECT log_id, status, process_status, http_status
FROM log_item_ingestion
ORDER BY request_time DESC;

-- Check master data created
SELECT * FROM m_region;           -- Should have UAE
SELECT * FROM m_city;             -- Should have DXB, AUH, SHJ
SELECT * FROM m_payment_term;     -- Should have N60, N90EOM, COD, etc.
SELECT * FROM m_channel;          -- Should have B2B, RETAIL
SELECT * FROM m_brand;            -- Should have FRESH, CRISPY, ALMARAI, etc.
SELECT * FROM m_category;         -- Should have JUICE, CHIPS, DAIRY, etc.
SELECT * FROM m_uom;              -- Should have BTL, CASE, PKT, etc.

-- Check transaction data
SELECT customer_code, customer_name, region_id, city_id
FROM t_customer;

SELECT item_code, item_name, brand_id, category_id
FROM t_item;

-- Check UOM conversions
SELECT i.item_code, u.uom_code, c.conversion_factor
FROM t_item_uom_conversion c
JOIN t_item i ON c.item_id = i.id
JOIN m_uom u ON c.uom_id = u.id;

-- Full customer view with joins
SELECT
    tc.customer_code,
    tc.customer_name,
    mr.region_name,
    mc.city_name,
    mpt.payment_term_name,
    mch.channel_name
FROM t_customer tc
LEFT JOIN m_region mr ON tc.region_id = mr.id
LEFT JOIN m_city mc ON tc.city_id = mc.id
LEFT JOIN m_payment_term mpt ON tc.payment_term_id = mpt.id
LEFT JOIN m_channel mch ON tc.channel_id = mch.id;
```

---

## Background Job Processing

**What happens:**
1. Test script sends 20 valid requests → `process_status = PENDING`
2. Background jobs run every 10 seconds
3. **CustomerMasterProcessor**:
   - Processes up to 10 customer logs per cycle
   - Creates master data (Region, City, PaymentTerm, Channel)
   - Inserts/updates `t_customer`
   - Updates `process_status = PROCESSED`

4. **ItemMasterProcessor**:
   - Processes up to 10 item logs per cycle
   - Creates master data (Brand, Category, UOM)
   - Inserts/updates `t_item`
   - Creates UOM conversions
   - Updates `process_status = PROCESSED`

**Timeline:**
- 0s: Test script completes (all in staging)
- 10s: First batch processed (up to 20 records)
- 20s: All records processed

---

## Dashboard Features

### Auto-Refresh
- **Enabled by default** (checkbox at top-right)
- Refreshes every 10 seconds
- Watch metrics update in real-time

### Manual Refresh
- Click **🔄 Refresh Now** button
- Updates immediately

### Filtering
- **All**: All requests combined
- **Customer**: Only customer requests
- **Item**: Only item requests

### Pagination
- 20 logs per page
- Click page numbers to navigate
- Previous/Next buttons

### JSON Viewer
- Click **View JSON** on any log
- See complete request payload
- Formatted & syntax highlighted

---

## Troubleshooting

### Dashboard shows "Loading logs..."
**Problem:** API not running or wrong URL

**Solution:**
```bash
# Check if API is running
ps aux | grep dotnet

# If not running, start it
cd MyApiApp
dotnet run
```

### Dashboard shows "Error loading logs"
**Problem:** CORS or connection issue

**Solution:**
- Check console (F12 in browser)
- Verify API URL: `http://localhost:5067`
- Check CORS is enabled in Program.cs

### No data in dashboard
**Problem:** Test script not run yet

**Solution:**
```bash
python3 test_data_generator.py
```

### Processing status stuck on PENDING
**Problem:** Background jobs not running

**Solution:**
- Check API logs for job startup
- Should see:
  ```
  CustomerMasterProcessor background job started.
  ItemMasterProcessor background job started.
  ```

### Python script fails
**Problem:** Requests library not installed

**Solution:**
```bash
pip3 install requests
```

---

## Color Coding Guide

### Dashboard
- 🟢 **Green**: Success (202, Processed)
- 🔴 **Red**: Errors (400, Failed)
- 🔵 **Blue**: Pending/In Progress
- 🟡 **Yellow**: Warnings

### Test Script
- ✓ Green: Success
- ✗ Red: Failure
- ℹ Yellow: Info
- → White: Request

---

## Complete Workflow Example

```bash
# Terminal 1: Start API
cd MyApiApp
dotnet run
# Wait for "Now listening on..."

# Terminal 2: Run tests
cd ..
python3 test_data_generator.py
# Press ENTER when prompted
# Watch colored output...
# See summary

# Browser: Open dashboard
# File → Open → dashboard.html
# Watch metrics update
# Filter by domain
# Click "View JSON" on logs
# Wait ~10-20 seconds
# See PENDING → PROCESSED
# See metrics update
```

---

## Expected Results

After running everything:

**Database:**
- `log_customer_ingestion`: 15 rows (10 SUCCESS, 5 VALIDATION_FAILED)
- `log_item_ingestion`: 15 rows (10 SUCCESS, 5 VALIDATION_FAILED)
- `m_region`: 1 row (UAE)
- `m_city`: 3 rows (DXB, AUH, SHJ)
- `m_payment_term`: ~4 rows
- `m_channel`: ~2 rows
- `m_brand`: ~6 rows
- `m_category`: ~6 rows
- `m_uom`: ~8 rows
- `t_customer`: 5 rows (CUST001-CUST005, with updates)
- `t_item`: 6 rows (JUICE001, SNACK001, etc.)
- `t_item_uom_conversion`: ~18 rows

**Dashboard Metrics:**
- Total Requests: 30
- API Success Rate: 66.7%
- Processing Rate: 100%
- Pending: 0
- Processed: 20
- Errors: 0

---

## Support

If something doesn't work:
1. Check API is running on port 5067
2. Check browser console (F12) for errors
3. Verify database connection
4. Check API logs for background job errors
5. Re-run test script to add more data

---

**Happy Testing! 🎉**
