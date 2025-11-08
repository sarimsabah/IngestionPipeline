# UI Metrics
<img width="1454" height="780" alt="image" src="https://github.com/user-attachments/assets/bf72cb62-1a1e-46b6-874b-7d476e371f07" />
<img width="1408" height="810" alt="image" src="https://github.com/user-attachments/assets/c9688aac-6f45-4785-942f-ddb85efd0b23" />


# 📚 Complete API Implementation Guide

## Table of Contents
1. [System Overview](#system-overview)
2. [Architecture](#architecture)
3. [Database Schema](#database-schema)
4. [Data Flow](#data-flow)
5. [API Endpoints](#api-endpoints)
6. [Validation Rules](#validation-rules)
7. [Background Processing](#background-processing)
8. [Monitoring Dashboard](#monitoring-dashboard)
9. [Setup & Installation](#setup--installation)
10. [Testing Guide](#testing-guide)
11. [Troubleshooting](#troubleshooting)

---

## System Overview

### What This System Does

This is a **Master Data Ingestion and Processing System** for Customer and Item data with the following capabilities:

1. **Accepts API requests** for customer and item data
2. **Validates data** immediately with comprehensive business rules
3. **Logs all requests** to staging tables for audit and tracking
4. **Processes data asynchronously** using background jobs
5. **Normalizes master data** into separate reference tables
6. **Creates transaction records** with foreign key relationships
7. **Provides monitoring dashboard** for real-time visibility

### Key Features

✅ **Immediate Validation** - Returns 202 (success) or 400 (validation failed) within milliseconds
✅ **Audit Trail** - Every request logged with full payload and status
✅ **Asynchronous Processing** - Background jobs process data without blocking API
✅ **Master Data Management** - Automatic upsert of reference data
✅ **Normalized Schema** - Proper relational database design
✅ **Real-time Monitoring** - Dashboard shows live metrics and logs
✅ **Error Handling** - Comprehensive error capture and reporting

---

## Architecture

### High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                         CLIENT LAYER                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │   Postman    │  │  Python      │  │  Dashboard   │          │
│  │   API Client │  │  Test Script │  │  (Browser)   │          │
│  └──────┬───────┘  └──────┬───────┘  └──────┬───────┘          │
└─────────┼──────────────────┼──────────────────┼─────────────────┘
          │                  │                  │
          ▼                  ▼                  ▼
┌─────────────────────────────────────────────────────────────────┐
│                      API LAYER (ASP.NET Core)                   │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Endpoints:                                               │  │
│  │  • POST /api/v1/customers/ingest                         │  │
│  │  • POST /api/v1/items/ingest                             │  │
│  │  • GET  /api/v1/dashboard/metrics                        │  │
│  │  • GET  /api/v1/dashboard/logs                           │  │
│  └───────────────────────────────────────────────────────────┘  │
│                                                                 │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Services:                                                │  │
│  │  • ValidationService - Validates requests                │  │
│  │  • DashboardService  - Aggregates metrics                │  │
│  │  • MappingService    - Maps DTOs to entities             │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
          │                                          ▲
          │ Writes                                   │ Reads
          ▼                                          │
┌─────────────────────────────────────────────────────────────────┐
│                   STAGING LAYER (PostgreSQL)                    │
│  ┌────────────────────────────┬────────────────────────────┐    │
│  │ log_customer_ingestion     │ log_item_ingestion         │    │
│  │ • log_id (PK)              │ • log_id (PK)              │    │
│  │ • raw_payload (JSON)       │ • raw_payload (JSON)       │    │
│  │ • status                   │ • status                   │    │
│  │ • process_status           │ • process_status           │    │
│  │ • validation_details       │ • validation_details       │    │
│  └────────────────────────────┴────────────────────────────┘    │
└─────────────────────────────────────────────────────────────────┘
          │                                          ▲
          │ Processed by                             │ Updates
          ▼                                          │
┌─────────────────────────────────────────────────────────────────┐
│              BACKGROUND PROCESSING LAYER                        │
│  ┌───────────────────────────────────────────────────────────┐  │
│  │  Background Jobs (Runs every 10 seconds):                 │  │
│  │                                                            │  │
│  │  CustomerMasterProcessor                                  │  │
│  │  ├─ Reads: log_customer_ingestion (PENDING)              │  │
│  │  ├─ Upserts: m_region, m_city, m_payment_term,          │  │
│  │  │           m_channel                                    │  │
│  │  ├─ Upserts: t_customer (with FK references)            │  │
│  │  └─ Updates: process_status → PROCESSED                  │  │
│  │                                                            │  │
│  │  ItemMasterProcessor                                      │  │
│  │  ├─ Reads: log_item_ingestion (PENDING)                 │  │
│  │  ├─ Upserts: m_brand, m_category, m_uom                 │  │
│  │  ├─ Upserts: t_item (with FK references)                │  │
│  │  ├─ Creates: t_item_uom_conversion                       │  │
│  │  └─ Updates: process_status → PROCESSED                  │  │
│  └───────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
          │
          │ Writes
          ▼
┌─────────────────────────────────────────────────────────────────┐
│                  MASTER DATA LAYER (PostgreSQL)                 │
│  ┌──────────┬──────────┬────────────────┬──────────┐            │
│  │ m_region │ m_city   │ m_payment_term │m_channel │            │
│  │ m_brand  │m_category│ m_uom          │          │            │
│  └──────────┴──────────┴────────────────┴──────────┘            │
└─────────────────────────────────────────────────────────────────┘
          │
          │ Referenced by
          ▼
┌─────────────────────────────────────────────────────────────────┐
│               TRANSACTION DATA LAYER (PostgreSQL)               │
│  ┌─────────────────────────┬──────────────────────────────┐     │
│  │ t_customer              │ t_item                       │     │
│  │ • customer_code (UQ)    │ • item_code (UQ)             │     │
│  │ • region_id (FK)        │ • brand_id (FK)              │     │
│  │ • city_id (FK)          │ • category_id (FK)           │     │
│  │ • payment_term_id (FK)  │                              │     │
│  │ • channel_id (FK)       │ t_item_uom_conversion        │     │
│  │                         │ • item_id (FK)               │     │
│  │                         │ • uom_id (FK)                │     │
│  └─────────────────────────┴──────────────────────────────┘     │
└─────────────────────────────────────────────────────────────────┘
```

---

## Database Schema

### Complete Table Structure

#### **1. STAGING TABLES** (Log Layer)

##### `log_customer_ingestion`
**Purpose:** Logs all customer ingestion requests immediately upon receipt

| Column              | Type           | Description                          |
|---------------------|----------------|--------------------------------------|
| `log_id`            | UUID (PK)      | Unique identifier, used as referenceId |
| `request_time`      | TIMESTAMP      | When request was received            |
| `raw_payload`       | TEXT           | Complete JSON request body           |
| `http_status`       | INTEGER        | 202 (success) or 400 (validation fail) |
| `status`            | VARCHAR(50)    | SUCCESS or VALIDATION_FAILED         |
| `validation_details`| TEXT           | JSON array of validation errors      |
| `process_status`    | VARCHAR(50)    | PENDING, PROCESSED, or ERROR         |
| `processed_at`      | TIMESTAMP      | When background job processed it     |
| `error_message`     | TEXT           | Error details if processing failed   |
| `reference_id`      | VARCHAR(100)   | Copy of log_id for easy reference    |

**Indexes:**
- `log_id` (PRIMARY KEY)
- `request_time`
- `status`
- `process_status`
- `reference_id`

**Lifecycle:**
1. Created immediately when API receives request
2. `status` set based on validation (SUCCESS/VALIDATION_FAILED)
3. `process_status` = PENDING for successful validations
4. Background job updates `process_status` to PROCESSED or ERROR
5. Never deleted (permanent audit trail)

---

##### `log_item_ingestion`
**Purpose:** Logs all item ingestion requests immediately upon receipt

Same structure as `log_customer_ingestion` but for item data.

---

#### **2. MASTER DATA TABLES** (Reference Data)

##### `m_region`
**Purpose:** Region master data (normalized)

| Column        | Type           | Description                |
|---------------|----------------|----------------------------|
| `id`          | SERIAL (PK)    | Auto-increment ID          |
| `region_code` | VARCHAR(50) UQ | Unique region code (UAE)   |
| `region_name` | VARCHAR(100)   | Region name                |
| `created_at`  | TIMESTAMP      | First created              |
| `updated_at`  | TIMESTAMP      | Last updated               |

**Business Rules:**
- `region_code` is UNIQUE
- Upserted by background job (INSERT if new, UPDATE if exists)

---

##### `m_city`
**Purpose:** City master data (normalized)

| Column            | Type           | Description              |
|-------------------|----------------|--------------------------|
| `id`              | SERIAL (PK)    | Auto-increment ID        |
| `city_code`       | VARCHAR(50) UQ | Unique city code (DXB)   |
| `city_name`       | VARCHAR(100)   | City name (Dubai)        |
| `city_name_arabic`| VARCHAR(100)   | Arabic name (دبي)        |
| `created_at`      | TIMESTAMP      | First created            |
| `updated_at`      | TIMESTAMP      | Last updated             |

---

##### `m_payment_term`
**Purpose:** Payment term master data

| Column             | Type           | Description                |
|--------------------|----------------|----------------------------|
| `id`               | SERIAL (PK)    | Auto-increment ID          |
| `payment_term_code`| VARCHAR(50) UQ | Unique code (N60, N90EOM)  |
| `payment_term_name`| VARCHAR(100)   | Name (Net 60 Days)         |
| `credit_days`      | INTEGER        | Number of credit days      |
| `created_at`       | TIMESTAMP      | First created              |
| `updated_at`       | TIMESTAMP      | Last updated               |

---

##### `m_channel`
**Purpose:** Sales channel master data

| Column            | Type           | Description                    |
|-------------------|----------------|--------------------------------|
| `id`              | SERIAL (PK)    | Auto-increment ID              |
| `channel_code`    | VARCHAR(50) UQ | Unique code (B2B, RETAIL)      |
| `channel_name`    | VARCHAR(255)   | Channel name                   |
| `sub_channel_code`| VARCHAR(50)    | Sub-channel code (WHOLESALE)   |
| `sub_channel_name`| VARCHAR(255)   | Sub-channel name               |
| `created_at`      | TIMESTAMP      | First created                  |
| `updated_at`      | TIMESTAMP      | Last updated                   |

---

##### `m_brand`
**Purpose:** Product brand master data

| Column        | Type           | Description               |
|---------------|----------------|---------------------------|
| `id`          | SERIAL (PK)    | Auto-increment ID         |
| `brand_code`  | VARCHAR(50) UQ | Unique code (FRESH)       |
| `brand_name`  | VARCHAR(255)   | Brand name (Fresh & Pure) |
| `created_at`  | TIMESTAMP      | First created             |
| `updated_at`  | TIMESTAMP      | Last updated              |

---

##### `m_category`
**Purpose:** Product category master data

| Column          | Type           | Description                  |
|-----------------|----------------|------------------------------|
| `id`            | SERIAL (PK)    | Auto-increment ID            |
| `category_code` | VARCHAR(50) UQ | Unique code (JUICE)          |
| `category_name` | VARCHAR(255)   | Category name (Fruit Juices) |
| `created_at`    | TIMESTAMP      | First created                |
| `updated_at`    | TIMESTAMP      | Last updated                 |

---

##### `m_uom`
**Purpose:** Unit of Measure master data

| Column           | Type           | Description              |
|------------------|----------------|--------------------------|
| `id`             | SERIAL (PK)    | Auto-increment ID        |
| `uom_code`       | VARCHAR(50) UQ | Unique code (BTL, CASE)  |
| `uom_description`| VARCHAR(255)   | Description              |
| `created_at`     | TIMESTAMP      | First created            |
| `updated_at`     | TIMESTAMP      | Last updated             |

---

#### **3. TRANSACTION DATA TABLES** (Operational Data)

##### `t_customer`
**Purpose:** Customer transaction/operational data with FK references to master data

| Column                | Type           | Description                      |
|-----------------------|----------------|----------------------------------|
| `id`                  | SERIAL (PK)    | Auto-increment ID                |
| `customer_code`       | VARCHAR(50) UQ | Unique customer code             |
| `customer_name`       | VARCHAR(255)   | Customer name                    |
| `arabic_description`  | VARCHAR(500)   | Arabic description               |
| `parent_customer_code`| VARCHAR(50)    | Parent customer code             |
| `parent_customer_name`| VARCHAR(255)   | Parent customer name             |
| `contact_no`          | VARCHAR(50)    | Contact number                   |
| `fax`                 | VARCHAR(50)    | Fax number                       |
| `email`               | VARCHAR(255)   | Email address                    |
| `address1`            | VARCHAR(255)   | Address line 1                   |
| `address2`            | VARCHAR(255)   | Address line 2                   |
| `address3`            | VARCHAR(255)   | Address line 3                   |
| `address4`            | VARCHAR(255)   | Address line 4                   |
| `is_active`           | BOOLEAN        | Active status                    |
| `longitude`           | DOUBLE         | GPS longitude                    |
| `latitude`            | DOUBLE         | GPS latitude                     |
| `contact_person_name` | VARCHAR(255)   | Contact person                   |
| **`region_id`**       | **INTEGER FK** | **→ m_region.id**                |
| **`city_id`**         | **INTEGER FK** | **→ m_city.id**                  |
| **`payment_term_id`** | **INTEGER FK** | **→ m_payment_term.id**          |
| **`channel_id`**      | **INTEGER FK** | **→ m_channel.id**               |
| `price_list_code`     | VARCHAR(50)    | Price list code                  |
| `customer_group_code` | VARCHAR(50)    | Customer group code              |
| `customer_group_name` | VARCHAR(255)   | Customer group name              |
| `credit_limit`        | DECIMAL(18,2)  | Credit limit amount              |
| `credit_days`         | INTEGER        | Credit days                      |
| `customer_type`       | VARCHAR(50)    | CASH or CREDIT                   |
| `is_blocked`          | BOOLEAN        | Blocked status                   |
| `created_at`          | TIMESTAMP      | Record created                   |
| `updated_at`          | TIMESTAMP      | Record updated                   |
| `reference_id`        | VARCHAR(100)   | Link to log_id                   |

**Indexes:**
- `customer_code` (UNIQUE)
- `region_id`
- `city_id`
- `payment_term_id`
- `channel_id`

**Foreign Keys:**
- `region_id` → `m_region(id)`
- `city_id` → `m_city(id)`
- `payment_term_id` → `m_payment_term(id)`
- `channel_id` → `m_channel(id)`

---

##### `t_item`
**Purpose:** Item transaction/operational data with FK references to master data

| Column                     | Type           | Description                    |
|----------------------------|----------------|--------------------------------|
| `id`                       | SERIAL (PK)    | Auto-increment ID              |
| `item_code`                | VARCHAR(50) UQ | Unique item code               |
| `item_name`                | VARCHAR(255)   | Item name                      |
| `arabic_description`       | VARCHAR(500)   | Arabic description             |
| `sales_org_code`           | VARCHAR(50)    | Sales organization code        |
| `base_uom`                 | VARCHAR(50)    | Base unit of measure           |
| **`brand_id`**             | **INTEGER FK** | **→ m_brand.id**               |
| **`category_id`**          | **INTEGER FK** | **→ m_category.id**            |
| `is_active`                | VARCHAR(10)    | "0" or "1"                     |
| `is_batch_enabled`         | BOOLEAN        | Batch tracking enabled         |
| `business_type`            | VARCHAR(50)    | Business type (F&B)            |
| `business_type_description`| VARCHAR(255)   | Description                    |
| `created_at`               | TIMESTAMP      | Record created                 |
| `updated_at`               | TIMESTAMP      | Record updated                 |
| `reference_id`             | VARCHAR(100)   | Link to log_id                 |

**Indexes:**
- `item_code` (UNIQUE)
- `brand_id`
- `category_id`

**Foreign Keys:**
- `brand_id` → `m_brand(id)`
- `category_id` → `m_category(id)`

---

##### `t_item_uom_conversion`
**Purpose:** Item-specific UOM conversion factors

| Column             | Type           | Description                        |
|--------------------|----------------|------------------------------------|
| `id`               | SERIAL (PK)    | Auto-increment ID                  |
| **`item_id`**      | **INTEGER FK** | **→ t_item.id**                    |
| **`uom_id`**       | **INTEGER FK** | **→ m_uom.id**                     |
| `conversion_factor`| DECIMAL        | Conversion factor (e.g., 24, 1)    |
| `created_at`       | TIMESTAMP      | Record created                     |
| `updated_at`       | TIMESTAMP      | Record updated                     |

**Indexes:**
- `(item_id, uom_id)` (UNIQUE composite)
- `uom_id`

**Foreign Keys:**
- `item_id` → `t_item(id)` ON DELETE CASCADE
- `uom_id` → `m_uom(id)` ON DELETE CASCADE

**Example Data:**
```
item_id | uom_id | conversion_factor
--------|--------|------------------
   1    |   1    |       1          (BTL → BTL = 1)
   1    |   2    |      12          (CASE = 12 BTL)
   1    |   3    |     480          (PALLET = 480 BTL)
```

---

#### **4. LEGACY TABLES** (For backward compatibility)

##### `customers`
Original customer table (still maintained for backward compatibility)

##### `items`
Original item table (still maintained for backward compatibility)

##### `item_uoms`
Original UOM table (still maintained for backward compatibility)

---

#### **5. TRANSACTION LOGGING TABLES**

##### `customer_transactions`
**Purpose:** Logs each transaction attempt during background processing

| Column              | Type           | Description                    |
|---------------------|----------------|--------------------------------|
| `transaction_id`    | UUID (PK)      | Unique transaction ID          |
| `log_id`            | UUID FK        | → log_customer_ingestion.log_id|
| `customer_code`     | VARCHAR(50)    | Customer code                  |
| `customer_name`     | VARCHAR(255)   | Customer name                  |
| `transaction_type`  | VARCHAR(20)    | INSERT or UPDATE               |
| `transaction_status`| VARCHAR(50)    | PENDING, SUCCESS, FAILED       |
| `transaction_time`  | TIMESTAMP      | When transaction started       |
| `completed_at`      | TIMESTAMP      | When completed                 |
| `error_message`     | TEXT           | Error if failed                |
| `reference_id`      | VARCHAR(100)   | Reference ID                   |

**Indexes:**
- `log_id`
- `customer_code`
- `transaction_status`

---

##### `item_transactions`
Same structure as `customer_transactions` but for items.

---

## Data Flow

### Customer Ingestion Flow (Step-by-Step)

```
┌─────────────────────────────────────────────────────────────────┐
│ STEP 1: API Request Received                                   │
└─────────────────────────────────────────────────────────────────┘
POST /api/v1/customers/ingest
{
  "customerCode": "CUST001",
  "customerName": "Test Customer",
  "email": "test@example.com",
  ...
}
                    │
                    ▼
┌─────────────────────────────────────────────────────────────────┐
│ STEP 2: Immediate Validation (< 100ms)                         │
└─────────────────────────────────────────────────────────────────┘
ValidationService.ValidateCustomer()
• Check required fields
• Validate email format
• Check credit logic
• Validate data types
                    │
        ┌───────────┴───────────┐
        │                       │
        ▼                       ▼
   ✓ VALID                 ✗ INVALID
        │                       │
        ▼                       ▼
┌─────────────────┐    ┌─────────────────┐
│ log_id created  │    │ log_id created  │
│ status=SUCCESS  │    │ status=         │
│ http_status=202 │    │ VALIDATION_     │
│ process_status= │    │ FAILED          │
│ PENDING         │    │ http_status=400 │
│                 │    │ validation_     │
│ INSERT INTO     │    │ details=[...]   │
│ log_customer_   │    │                 │
│ ingestion       │    │ INSERT INTO     │
│                 │    │ log_customer_   │
│ RETURN 202      │    │ ingestion       │
└────────┬────────┘    │                 │
         │             │ RETURN 400      │
         │             └─────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│ STEP 3: Background Job (every 10 seconds)                      │
│ CustomerMasterProcessor.ExecuteAsync()                          │
└─────────────────────────────────────────────────────────────────┘
Query:
  SELECT * FROM log_customer_ingestion
  WHERE status = 'SUCCESS'
    AND process_status = 'PENDING'
  ORDER BY request_time
  LIMIT 10
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│ STEP 4: Process Master Data (Upsert)                           │
└─────────────────────────────────────────────────────────────────┘
For each pending log:
  1. Deserialize raw_payload
  2. UpsertRegion(regionCode, regionName)
     └─ INSERT INTO m_region ... ON CONFLICT UPDATE
     └─ RETURN region_id
  3. UpsertCity(cityCode, cityName)
     └─ RETURN city_id
  4. UpsertPaymentTerm(paymentTermCode, ...)
     └─ RETURN payment_term_id
  5. UpsertChannel(channelCode, ...)
     └─ RETURN channel_id
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│ STEP 5: Process Transaction Data (Upsert)                      │
└─────────────────────────────────────────────────────────────────┘
  Check if customer exists:
    SELECT * FROM t_customer
    WHERE customer_code = 'CUST001'

  IF EXISTS:
    UPDATE t_customer
    SET customer_name = ...,
        region_id = ...,
        city_id = ...,
        updated_at = NOW()
    WHERE customer_code = 'CUST001'
  ELSE:
    INSERT INTO t_customer
    (customer_code, customer_name, region_id, ...)
    VALUES (...)
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│ STEP 6: Update Processing Status                               │
└─────────────────────────────────────────────────────────────────┘
  UPDATE log_customer_ingestion
  SET process_status = 'PROCESSED',
      processed_at = NOW()
  WHERE log_id = ...

  INSERT INTO customer_transactions
  (log_id, customer_code, transaction_type,
   transaction_status, ...)
  VALUES (...)
         │
         ▼
┌─────────────────────────────────────────────────────────────────┐
│ COMPLETE: Data Available in Transaction Table                  │
└─────────────────────────────────────────────────────────────────┘
```

### Item Ingestion Flow (Step-by-Step)

Similar to customer flow with these additional steps:

```
┌─────────────────────────────────────────────────────────────────┐
│ ADDITIONAL STEP: UOM Processing                                │
└─────────────────────────────────────────────────────────────────┘
For each UOM in uomList:
  1. UpsertUom(uomCode)
     └─ INSERT INTO m_uom ... ON CONFLICT UPDATE
     └─ RETURN uom_id
     └─ Store in dictionary[uomCode] = uom_id

  2. Delete existing conversions:
     DELETE FROM t_item_uom_conversion
     WHERE item_id = ...

  3. Insert new conversions:
     FOR EACH uom IN uomList:
       INSERT INTO t_item_uom_conversion
       (item_id, uom_id, conversion_factor)
       VALUES (item_id, uom_ids[uom], factor)
```

---

## API Endpoints

### 1. Customer Ingestion

**Endpoint:** `POST /api/v1/customers/ingest`
**Authentication:** Required (Bearer token)
**Content-Type:** application/json

**Request Body:**
```json
{
  "customerCode": "CUST001",           // Required
  "customerName": "Test Customer",     // Required
  "contactNo": "+971501234567",        // Required
  "email": "test@example.com",         // Required, valid format
  "cityCode": "DXB",                   // Required
  "cityName": "Dubai",                 // Required
  "regionCode": "UAE",                 // Required
  "regionName": "United Arab Emirates",// Required
  "customerType": "CREDIT",            // Required
  "isActive": true,                    // Required
  "isBlocked": false,                  // Required
  "creditLimit": 100000.00,            // Required, numeric
  "creditDays": 30,                    // Required, numeric
  "arabicDescription": "...",          // Optional
  "address1": "...",                   // Optional
  "paymentTermCode": "N30",            // Optional
  "channelCode": "B2B"                 // Optional
}
```

**Success Response (202 Accepted):**
```json
{
  "status": "Success",
  "message": "Customer data received and queued for processing.",
  "referenceId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Failure Response (400 Bad Request):**
```json
{
  "status": "Failure",
  "message": "Validation failed for one or more fields.",
  "validationErrors": [
    {
      "field": "email",
      "reason": "Email format is invalid."
    },
    {
      "field": "creditLimit",
      "reason": "For non-CASH customer types, credit limit must be greater than 0."
    }
  ]
}
```

---

### 2. Item Ingestion

**Endpoint:** `POST /api/v1/items/ingest`
**Authentication:** Required (Bearer token)
**Content-Type:** application/json

**Request Body:**
```json
{
  "material": {
    "itemCode": "ITEM001",             // Required
    "itemName": "Test Item",           // Required
    "salesOrgCode": "AE_DUBAI",        // Required
    "baseUOM": "PCS",                  // Required
    "brandCode": "BRAND01",            // Required
    "brandName": "Test Brand",         // Required
    "categoryCode": "CAT01",           // Required
    "categoryName": "Test Category",   // Required
    "isActive": "1",                   // Required ("0" or "1")
    "isBatchEnabled": true,            // Required
    "uomList": [                       // Required, non-empty
      {
        "uom": "PCS",                  // Required
        "conversionFactor": 1          // Required, > 0
      },
      {
        "uom": "CASE",
        "conversionFactor": 24
      }
    ]
  }
}
```

**Responses:** Same as customer endpoint

---

### 3. Dashboard Metrics

**Endpoint:** `GET /api/v1/dashboard/metrics`
**Authentication:** None (public endpoint)

**Response:**
```json
{
  "totalRequests": 150,
  "customerMetrics": {
    "totalRequests": 75,
    "apiSuccessRate": 90.67,
    "processingSuccessRate": 100.00,
    "successCount": 68,
    "failureCount": 7,
    "processedCount": 68,
    "pendingCount": 0,
    "errorCount": 0
  },
  "itemMetrics": {
    "totalRequests": 75,
    "apiSuccessRate": 88.00,
    "processingSuccessRate": 100.00,
    "successCount": 66,
    "failureCount": 9,
    "processedCount": 66,
    "pendingCount": 0,
    "errorCount": 0
  },
  "ingestionStatus": {
    "apiSuccess": 134,
    "apiFailure": 16
  },
  "processingStatus": {
    "pending": 0,
    "processed": 134,
    "error": 0
  }
}
```

---

### 4. Dashboard Logs

**Endpoint:** `GET /api/v1/dashboard/logs`
**Authentication:** None (public endpoint)

**Query Parameters:**
- `domain` (optional): "customer" or "item" (null = all)
- `pageNumber` (optional): default 1
- `pageSize` (optional): default 20

**Example:**
```
GET /api/v1/dashboard/logs?domain=customer&pageNumber=1&pageSize=10
```

**Response:**
```json
{
  "logs": [
    {
      "referenceId": "550e8400-e29b-41d4-a716-446655440000",
      "domain": "Customer",
      "receivedTime": "2025-11-08T16:30:00Z",
      "apiStatus": 202,
      "validationFailures": null,
      "processingStatus": "PROCESSED",
      "rawRequest": "{...json...}",
      "errorMessage": null
    }
  ],
  "totalCount": 75,
  "pageNumber": 1,
  "pageSize": 10
}
```

---

## Validation Rules

### Customer Validation Rules

| Field          | Rule                                          | Error Message                                           |
|----------------|-----------------------------------------------|---------------------------------------------------------|
| customerCode   | Mandatory, non-empty string                   | "Customer code is mandatory and cannot be empty."       |
| customerName   | Mandatory, non-empty string                   | "Customer name is mandatory and cannot be empty."       |
| contactNo      | Mandatory, non-empty string                   | "Contact number is mandatory and cannot be empty."      |
| email          | Mandatory, valid email format                 | "Email is mandatory and cannot be empty." / "Email format is invalid." |
| cityCode       | Mandatory, non-empty string                   | "City code is mandatory and cannot be empty."           |
| cityName       | Mandatory, non-empty string                   | "City name is mandatory and cannot be empty."           |
| regionCode     | Mandatory, non-empty string                   | "Region code is mandatory and cannot be empty."         |
| regionName     | Mandatory, non-empty string                   | "Region name is mandatory and cannot be empty."         |
| customerType   | Mandatory, non-empty string                   | "Customer type is mandatory and cannot be empty."       |
| isActive       | Mandatory, boolean                            | "isActive is mandatory and must be a boolean."          |
| isBlocked      | Mandatory, boolean                            | "isBlocked is mandatory and must be a boolean."         |
| creditLimit    | Mandatory, numeric                            | "Credit limit is mandatory and must be numeric."        |
| creditDays     | Mandatory, numeric                            | "Credit days is mandatory and must be numeric."         |
| **Credit Logic** | If customerType != "CASH", creditLimit > 0  | "For non-CASH customer types, credit limit must be greater than 0." |
| **Credit Logic** | If customerType != "CASH", creditDays > 0   | "For non-CASH customer types, credit days must be greater than 0." |

### Item Validation Rules

| Field              | Rule                                      | Error Message                                                |
|--------------------|-------------------------------------------|--------------------------------------------------------------|
| material           | Mandatory, must be object                 | "Material data is required."                                 |
| itemCode           | Mandatory, non-empty string               | "Item code is mandatory and cannot be empty."                |
| itemName           | Mandatory, non-empty string               | "Item name is mandatory and cannot be empty."                |
| salesOrgCode       | Mandatory, non-empty string               | "Sales org code is mandatory and cannot be empty."           |
| baseUOM            | Mandatory, non-empty string               | "Base UOM is mandatory and cannot be empty."                 |
| brandCode          | Mandatory, non-empty string               | "Brand code is mandatory and cannot be empty."               |
| brandName          | Mandatory, non-empty string               | "Brand name is mandatory and cannot be empty."               |
| categoryCode       | Mandatory, non-empty string               | "Category code is mandatory and cannot be empty."            |
| categoryName       | Mandatory, non-empty string               | "Category name is mandatory and cannot be empty."            |
| isBatchEnabled     | Mandatory, boolean                        | "isBatchEnabled is mandatory and must be a boolean."         |
| isActive           | Mandatory, "0" or "1"                     | "isActive is mandatory and cannot be empty." / "isActive must be either '0' or '1'." |
| uomList            | Mandatory, non-empty array                | "uomList is mandatory and must be a non-empty array."        |
| uomList[].uom      | Mandatory, non-empty string               | "UOM is mandatory and cannot be empty."                      |
| uomList[].conversionFactor | Mandatory, numeric > 0          | "Conversion factor is mandatory and must be numeric." / "Conversion factor must be greater than 0." |

---

## Background Processing

### CustomerMasterProcessor

**Runs:** Every 10 seconds
**Processes:** Up to 10 pending customer logs per cycle

**Algorithm:**
```csharp
while (not stopped)
{
    // 1. Get pending logs
    var logs = db.CustomerIngestionLogs
        .Where(l => l.Status == "SUCCESS" && l.ProcessStatus == "PENDING")
        .OrderBy(l => l.RequestTime)
        .Take(10)
        .ToList();

    foreach (var log in logs)
    {
        try
        {
            // 2. Deserialize payload
            var request = JsonDeserialize<CustomerRequest>(log.RawPayload);

            // 3. Upsert master data
            regionId = UpsertRegion(request);
            cityId = UpsertCity(request);
            paymentTermId = UpsertPaymentTerm(request);
            channelId = UpsertChannel(request);

            // 4. Upsert transaction data
            UpsertCustomer(request, regionId, cityId, paymentTermId, channelId);

            // 5. Mark as processed
            log.ProcessStatus = "PROCESSED";
            log.ProcessedAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            log.ProcessStatus = "ERROR";
            log.ErrorMessage = ex.Message;
            log.ProcessedAt = DateTime.UtcNow;
        }
    }

    // Wait 10 seconds
    await Task.Delay(10000);
}
```

**Upsert Logic (Example for Region):**
```csharp
async Task<int?> UpsertRegion(CustomerRequest request)
{
    if (string.IsNullOrWhiteSpace(request.RegionCode))
        return null;

    // Try to find existing
    var existing = await db.MasterRegions
        .FirstOrDefaultAsync(r => r.RegionCode == request.RegionCode);

    if (existing != null)
    {
        // UPDATE
        existing.RegionName = request.RegionName ?? existing.RegionName;
        existing.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return existing.Id;
    }

    // INSERT
    var newRegion = new MRegionEntity
    {
        RegionCode = request.RegionCode,
        RegionName = request.RegionName ?? string.Empty
    };
    db.MasterRegions.Add(newRegion);
    await db.SaveChangesAsync();
    return newRegion.Id;
}
```

---

### ItemMasterProcessor

**Runs:** Every 10 seconds
**Processes:** Up to 10 pending item logs per cycle

**Special Handling for UOMs:**
1. Extract all unique UOM codes from uomList
2. Upsert each UOM to m_uom table
3. Store UOM ID mappings: `Dictionary<uomCode, uomId>`
4. Delete existing conversions for this item
5. Insert new conversions using the ID mappings

**UOM Conversion Processing:**
```csharp
// Delete old conversions
db.TransactionItemUomConversions
    .Where(c => c.ItemId == itemId)
    .ToList()
    .ForEach(c => db.Remove(c));

// Insert new conversions
foreach (var uomData in material.UomList)
{
    var conversion = new TItemUomConversionEntity
    {
        ItemId = itemId,
        UomId = uomIds[uomData.Uom],  // From dictionary
        ConversionFactor = uomData.ConversionFactor ?? 1
    };
    db.TransactionItemUomConversions.Add(conversion);
}
await db.SaveChangesAsync();
```

---

## Monitoring Dashboard

### Dashboard Features

**1. Real-time Metrics**
- Auto-refreshes every 10 seconds
- Shows combined and per-domain statistics
- Color-coded status indicators

**2. Filtering**
- View all logs
- Filter by customer domain
- Filter by item domain

**3. Pagination**
- 20 records per page
- Easy navigation

**4. JSON Viewer**
- Click "View JSON" on any log
- See complete request payload
- Formatted display

### Dashboard Calculations

**API Success Rate:**
```
API Success Rate = (Success Count / Total Requests) × 100
Success Count = logs where http_status = 202
Total Requests = all logs
```

**Processing Success Rate:**
```
Processing Rate = (Processed Count / Successful Logs) × 100
Processed Count = logs where process_status = 'PROCESSED'
Successful Logs = logs where status = 'SUCCESS'
```

---

## Setup & Installation

### Prerequisites

- .NET 9 SDK
- PostgreSQL 13+
- Python 3.8+ (for test script)
- Modern web browser

### Step 1: Clone/Setup Project

```bash
cd /Users/turbostart-blr-lap0061/Desktop/MyAPI/MyApiApp
```

### Step 2: Configure Database

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=myapidb;Username=postgres;Password=yourpassword"
  }
}
```

### Step 3: Run Migrations

```bash
dotnet ef database update
```

This creates all tables:
- Staging: log_customer_ingestion, log_item_ingestion
- Master: m_region, m_city, m_payment_term, m_channel, m_brand, m_category, m_uom
- Transaction: t_customer, t_item, t_item_uom_conversion
- Logging: customer_transactions, item_transactions

### Step 4: Start API

```bash
dotnet run
```

### Step 5: Populate Test Data

```bash
cd ..
python3 test_data_generator.py
```

### Step 6: Open Dashboard

```bash
open dashboard.html
```

---

## Testing Guide

### Manual Testing with Postman

See `QUICK_START.md` for Postman examples.

### Automated Testing

Run Python test script:
```bash
python3 test_data_generator.py
```

This sends:
- 10 valid customer requests
- 5 invalid customer requests
- 10 valid item requests
- 5 invalid item requests

### Database Verification

```sql
-- Check staging
SELECT log_id, status, process_status
FROM log_customer_ingestion;

-- Check master data
SELECT * FROM m_region;
SELECT * FROM m_city;
SELECT * FROM m_brand;

-- Check transaction data with joins
SELECT
    c.customer_code,
    c.customer_name,
    r.region_name,
    ct.city_name,
    pt.payment_term_name,
    ch.channel_name
FROM t_customer c
LEFT JOIN m_region r ON c.region_id = r.id
LEFT JOIN m_city ct ON c.city_id = ct.id
LEFT JOIN m_payment_term pt ON c.payment_term_id = pt.id
LEFT JOIN m_channel ch ON c.channel_id = ch.id;

-- Check item UOM conversions
SELECT
    i.item_code,
    i.item_name,
    u.uom_code,
    c.conversion_factor
FROM t_item_uom_conversion c
JOIN t_item i ON c.item_id = i.id
JOIN m_uom u ON c.uom_id = u.id
ORDER BY i.item_code, c.conversion_factor;
```

---

## Troubleshooting

### API Not Starting

**Issue:** Port already in use

**Solution:**
```bash
lsof -i :5067
kill -9 <PID>
dotnet run
```

---

### Background Jobs Not Processing

**Issue:** Logs stuck in PENDING status

**Check:**
```bash
# Look for these messages in API console
CustomerMasterProcessor background job started.
ItemMasterProcessor background job started.
Processing X pending customer records.
```

**Solution:** Restart API

---

### Dashboard Not Loading Data

**Issue:** CORS or connection errors

**Check:** Browser console (F12)

**Solution:**
1. Verify API running on port 5067
2. Check CORS enabled in Program.cs
3. Refresh page

---

### Database Migration Errors

**Issue:** Migration fails

**Solution:**
```bash
# Remove last migration
dotnet ef migrations remove

# Recreate
dotnet ef migrations add MigrationName

# Apply
dotnet ef database update
```

---

## Summary

This system provides:

✅ **Complete audit trail** - Every request logged with full payload
✅ **Immediate feedback** - Validation happens synchronously
✅ **Asynchronous processing** - Background jobs don't block API
✅ **Normalized data** - Master data separated for reusability
✅ **Referential integrity** - Foreign keys maintain relationships
✅ **Real-time monitoring** - Dashboard shows live status
✅ **Error handling** - Comprehensive error capture and reporting
✅ **Scalability** - Background jobs process in batches

**Total Tables:** 17
- Staging: 2
- Master: 7
- Transaction: 3
- Logging: 2
- Legacy: 3

**Background Jobs:** 2 (run every 10 seconds)

**API Endpoints:** 4

**Validation Rules:** 30+

---

For quick start instructions, see `QUICK_START.md`
For testing details, see `README_TESTING.md`

