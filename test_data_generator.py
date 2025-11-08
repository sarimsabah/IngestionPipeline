#!/usr/bin/env python3
"""
API Test Data Generator
Runs comprehensive test cases to populate the database and validate all functionalities
"""

import requests
import json
import time
from datetime import datetime
from typing import Dict, List, Tuple

# Configuration
API_BASE_URL = "http://localhost:5067/api/v1"
AUTH_TOKEN = "test-token-12345"
HEADERS = {
    "Authorization": f"Bearer {AUTH_TOKEN}",
    "Content-Type": "application/json"
}

# Colors for terminal output
class Colors:
    GREEN = '\033[92m'
    RED = '\033[91m'
    YELLOW = '\033[93m'
    BLUE = '\033[94m'
    MAGENTA = '\033[95m'
    CYAN = '\033[96m'
    WHITE = '\033[97m'
    RESET = '\033[0m'
    BOLD = '\033[1m'

# Statistics tracking
stats = {
    "customer_success": 0,
    "customer_failed": 0,
    "item_success": 0,
    "item_failed": 0,
    "total_requests": 0
}

def print_header(text: str):
    """Print a formatted header"""
    print(f"\n{Colors.BOLD}{Colors.CYAN}{'='*80}{Colors.RESET}")
    print(f"{Colors.BOLD}{Colors.CYAN}{text.center(80)}{Colors.RESET}")
    print(f"{Colors.BOLD}{Colors.CYAN}{'='*80}{Colors.RESET}\n")

def print_test(test_num: int, test_name: str):
    """Print test case header"""
    print(f"{Colors.BOLD}{Colors.BLUE}[TEST {test_num}] {test_name}{Colors.RESET}")

def print_success(message: str):
    """Print success message"""
    print(f"{Colors.GREEN}✓ {message}{Colors.RESET}")

def print_error(message: str):
    """Print error message"""
    print(f"{Colors.RED}✗ {message}{Colors.RESET}")

def print_info(message: str):
    """Print info message"""
    print(f"{Colors.YELLOW}ℹ {message}{Colors.RESET}")

def print_response(status_code: int, response_data: dict):
    """Print formatted response"""
    if status_code == 202:
        print(f"{Colors.GREEN}Response [{status_code}]:{Colors.RESET}")
        print(f"  Status: {response_data.get('status')}")
        print(f"  Message: {response_data.get('message')}")
        print(f"  Reference ID: {Colors.BOLD}{response_data.get('referenceId')}{Colors.RESET}")
    elif status_code == 400:
        print(f"{Colors.RED}Response [{status_code}]:{Colors.RESET}")
        print(f"  Status: {response_data.get('status')}")
        print(f"  Message: {response_data.get('message')}")
        if 'validationErrors' in response_data:
            print(f"  Validation Errors ({len(response_data['validationErrors'])}):")
            for error in response_data['validationErrors'][:3]:  # Show first 3
                print(f"    - {error['field']}: {error['reason']}")
            if len(response_data['validationErrors']) > 3:
                print(f"    ... and {len(response_data['validationErrors']) - 3} more")
    else:
        print(f"{Colors.YELLOW}Response [{status_code}]:{Colors.RESET}")
        print(f"  {json.dumps(response_data, indent=2)}")

def make_request(endpoint: str, data: dict, expected_status: int) -> Tuple[bool, dict]:
    """Make API request and validate response"""
    global stats
    stats["total_requests"] += 1

    try:
        url = f"{API_BASE_URL}{endpoint}"
        print(f"{Colors.WHITE}→ POST {url}{Colors.RESET}")

        response = requests.post(url, headers=HEADERS, json=data, timeout=10)
        response_data = response.json() if response.text else {}

        print_response(response.status_code, response_data)

        # Validate status code
        if response.status_code == expected_status:
            print_success(f"Expected status {expected_status} received")
            return True, response_data
        else:
            print_error(f"Expected {expected_status}, got {response.status_code}")
            return False, response_data

    except requests.exceptions.ConnectionError:
        print_error("Connection failed! Is the API running on http://localhost:5067?")
        return False, {}
    except requests.exceptions.Timeout:
        print_error("Request timed out!")
        return False, {}
    except Exception as e:
        print_error(f"Error: {str(e)}")
        return False, {}

# ============================================================================
# CUSTOMER TEST CASES
# ============================================================================

CUSTOMER_TESTS = [
    # Valid customer tests
    {
        "name": "Valid Customer - Dubai, CREDIT type",
        "expected": 202,
        "data": {
            "customerCode": "CUST001",
            "customerName": "Al Manara Trading LLC",
            "arabicDescription": "شركة المنارة للتجارة",
            "parentCustomerCode": "PARENT01",
            "parentCustomerName": "Al Manara Group",
            "contactNo": "+971501234567",
            "fax": "+97143334444",
            "email": "info@almanara.ae",
            "address1": "Building 123, Sheikh Zayed Road",
            "address2": "Near Metro Station",
            "address3": "Trade Centre Area",
            "address4": "Dubai, UAE",
            "isActive": True,
            "longitude": 55.2708,
            "latitude": 25.2048,
            "contactPersonName": "Ahmed Al Mazrouei",
            "cityCode": "DXB",
            "cityName": "Dubai",
            "cityNameArabic": "دبي",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "priceListCode": "STD_UAE",
            "customerGroupCode": "RETAIL",
            "customerGroupName": "Retail Customers",
            "creditLimit": 500000.00,
            "creditDays": 60,
            "paymentTermName": "Net 60 Days",
            "paymentTermCode": "N60",
            "customerType": "CREDIT",
            "channelCode": "B2B",
            "channelName": "Business to Business",
            "subChannelCode": "WHOLESALE",
            "subChannelName": "Wholesale Distribution",
            "isBlocked": False
        }
    },
    {
        "name": "Valid Customer - Abu Dhabi, High Credit",
        "expected": 202,
        "data": {
            "customerCode": "CUST002",
            "customerName": "Capital Distributors FZE",
            "arabicDescription": "موزعو العاصمة",
            "contactNo": "+971506667777",
            "email": "sales@capitaldist.ae",
            "address1": "Industrial Area 15",
            "address2": "Mussafah",
            "address3": "Abu Dhabi",
            "address4": "UAE",
            "isActive": True,
            "contactPersonName": "Fatima Al Dhaheri",
            "cityCode": "AUH",
            "cityName": "Abu Dhabi",
            "cityNameArabic": "أبو ظبي",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "creditLimit": 750000.00,
            "creditDays": 90,
            "paymentTermName": "Net 90 Days EOM",
            "paymentTermCode": "N90EOM",
            "customerType": "CREDIT",
            "channelCode": "B2B",
            "channelName": "Business to Business",
            "subChannelCode": "DISTRIBUTOR",
            "subChannelName": "Authorized Distributors",
            "isBlocked": False
        }
    },
    {
        "name": "Valid Customer - CASH type (no credit)",
        "expected": 202,
        "data": {
            "customerCode": "CUST003",
            "customerName": "Quick Mart Supermarket",
            "arabicDescription": "سوبر ماركت كويك مارت",
            "contactNo": "+971505556666",
            "email": "manager@quickmart.ae",
            "address1": "Al Barsha Mall, Shop 45",
            "address2": "Al Barsha 1",
            "address3": "Dubai",
            "address4": "UAE",
            "isActive": True,
            "contactPersonName": "Mohamed Ali",
            "cityCode": "DXB",
            "cityName": "Dubai",
            "cityNameArabic": "دبي",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "creditLimit": 0,
            "creditDays": 0,
            "paymentTermName": "Cash on Delivery",
            "paymentTermCode": "COD",
            "customerType": "CASH",
            "channelCode": "RETAIL",
            "channelName": "Retail Channel",
            "subChannelCode": "SUPERMARKET",
            "subChannelName": "Supermarkets",
            "isBlocked": False
        }
    },
    {
        "name": "Valid Customer - Sharjah Region",
        "expected": 202,
        "data": {
            "customerCode": "CUST004",
            "customerName": "Sharjah Trade Center LLC",
            "contactNo": "+971509998888",
            "email": "info@sharjahtrade.ae",
            "address1": "Industrial Area 6",
            "isActive": True,
            "cityCode": "SHJ",
            "cityName": "Sharjah",
            "cityNameArabic": "الشارقة",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "creditLimit": 300000.00,
            "creditDays": 45,
            "paymentTermName": "Net 45 Days",
            "paymentTermCode": "N45",
            "customerType": "CREDIT",
            "channelCode": "B2B",
            "channelName": "Business to Business",
            "isBlocked": False
        }
    },
    {
        "name": "Valid Customer - Blocked Customer",
        "expected": 202,
        "data": {
            "customerCode": "CUST005",
            "customerName": "Blocked Customer LLC",
            "contactNo": "+971501112222",
            "email": "blocked@example.ae",
            "address1": "Some Address",
            "isActive": False,
            "cityCode": "DXB",
            "cityName": "Dubai",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "creditLimit": 50000.00,
            "creditDays": 30,
            "paymentTermName": "Net 30 Days",
            "paymentTermCode": "N30",
            "customerType": "CREDIT",
            "channelCode": "RETAIL",
            "channelName": "Retail Channel",
            "isBlocked": True
        }
    },

    # Invalid customer tests
    {
        "name": "Invalid - Missing Required Fields",
        "expected": 400,
        "data": {
            "customerCode": "INVALID001",
            "customerName": "Incomplete Customer"
        }
    },
    {
        "name": "Invalid - Bad Email Format",
        "expected": 400,
        "data": {
            "customerCode": "INVALID002",
            "customerName": "Bad Email Customer",
            "contactNo": "+971501234567",
            "email": "not-an-email",
            "cityCode": "DXB",
            "cityName": "Dubai",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "customerType": "CREDIT",
            "isActive": True,
            "isBlocked": False,
            "creditLimit": 100000,
            "creditDays": 30
        }
    },
    {
        "name": "Invalid - CREDIT type with zero credit",
        "expected": 400,
        "data": {
            "customerCode": "INVALID003",
            "customerName": "Invalid Credit Customer",
            "contactNo": "+971501234567",
            "email": "test@example.ae",
            "cityCode": "DXB",
            "cityName": "Dubai",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "customerType": "CREDIT",
            "isActive": True,
            "isBlocked": False,
            "creditLimit": 0,
            "creditDays": 0
        }
    },
    {
        "name": "Invalid - Missing Email",
        "expected": 400,
        "data": {
            "customerCode": "INVALID004",
            "customerName": "No Email Customer",
            "contactNo": "+971501234567",
            "cityCode": "DXB",
            "cityName": "Dubai",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "customerType": "CASH",
            "isActive": True,
            "isBlocked": False,
            "creditLimit": 0,
            "creditDays": 0
        }
    },
    {
        "name": "Update Existing Customer - CUST001",
        "expected": 202,
        "data": {
            "customerCode": "CUST001",
            "customerName": "Al Manara Trading LLC - UPDATED",
            "contactNo": "+971509999999",
            "email": "updated@almanara.ae",
            "address1": "NEW ADDRESS - Building 456",
            "isActive": False,
            "cityCode": "DXB",
            "cityName": "Dubai",
            "cityNameArabic": "دبي",
            "regionCode": "UAE",
            "regionName": "United Arab Emirates",
            "creditLimit": 1000000.00,
            "creditDays": 90,
            "paymentTermName": "Net 90 Days",
            "paymentTermCode": "N90",
            "customerType": "CREDIT",
            "channelCode": "B2B",
            "channelName": "Business to Business",
            "isBlocked": True
        }
    }
]

# ============================================================================
# ITEM TEST CASES
# ============================================================================

ITEM_TESTS = [
    # Valid item tests
    {
        "name": "Valid Item - Juice Product",
        "expected": 202,
        "data": {
            "material": {
                "itemCode": "JUICE001",
                "itemName": "Orange Fresh Juice 1L",
                "arabicDescription": "عصير برتقال طازج ١ لتر",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "BTL",
                "brandCode": "FRESH",
                "brandName": "Fresh & Pure",
                "categoryCode": "JUICE",
                "categoryName": "Fruit Juices",
                "isActive": "1",
                "isBatchEnabled": True,
                "businessType": "F&B",
                "businessTypeDescription": "Food & Beverage - Consumable",
                "uomList": [
                    {"uom": "BTL", "conversionFactor": 1},
                    {"uom": "CASE", "conversionFactor": 12},
                    {"uom": "PALLET", "conversionFactor": 480}
                ]
            }
        }
    },
    {
        "name": "Valid Item - Snack Product",
        "expected": 202,
        "data": {
            "material": {
                "itemCode": "SNACK001",
                "itemName": "Potato Chips BBQ 150g",
                "arabicDescription": "رقائق البطاطس بنكهة الشواء ١٥٠ جرام",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "PKT",
                "brandCode": "CRISPY",
                "brandName": "Crispy Snacks Co.",
                "categoryCode": "CHIPS",
                "categoryName": "Potato Chips & Snacks",
                "isActive": "1",
                "isBatchEnabled": True,
                "businessType": "F&B",
                "businessTypeDescription": "Food & Beverage - Packaged Snacks",
                "uomList": [
                    {"uom": "PKT", "conversionFactor": 1},
                    {"uom": "BOX", "conversionFactor": 24},
                    {"uom": "CARTON", "conversionFactor": 96}
                ]
            }
        }
    },
    {
        "name": "Valid Item - Dairy Product",
        "expected": 202,
        "data": {
            "material": {
                "itemCode": "DAIRY001",
                "itemName": "Fresh Milk Full Cream 2L",
                "arabicDescription": "حليب طازج كامل الدسم ٢ لتر",
                "salesOrgCode": "AE_ABU_DHABI",
                "baseUOM": "BTL",
                "brandCode": "ALMARAI",
                "brandName": "Almarai",
                "categoryCode": "DAIRY",
                "categoryName": "Dairy Products",
                "isActive": "1",
                "isBatchEnabled": True,
                "businessType": "F&B",
                "businessTypeDescription": "Food & Beverage - Refrigerated",
                "uomList": [
                    {"uom": "BTL", "conversionFactor": 1},
                    {"uom": "CRATE", "conversionFactor": 6}
                ]
            }
        }
    },
    {
        "name": "Valid Item - Cleaning Product (isActive=0)",
        "expected": 202,
        "data": {
            "material": {
                "itemCode": "CLEAN001",
                "itemName": "Floor Cleaner Lemon 5L",
                "arabicDescription": "منظف أرضيات برائحة الليمون ٥ لتر",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "BTL",
                "brandCode": "SPARKLE",
                "brandName": "Sparkle Clean",
                "categoryCode": "CLEANING",
                "categoryName": "Cleaning Products",
                "isActive": "0",
                "isBatchEnabled": False,
                "businessType": "HOME",
                "businessTypeDescription": "Home Care Products",
                "uomList": [
                    {"uom": "BTL", "conversionFactor": 1},
                    {"uom": "CASE", "conversionFactor": 4}
                ]
            }
        }
    },
    {
        "name": "Valid Item - Water Bottle",
        "expected": 202,
        "data": {
            "material": {
                "itemCode": "WATER001",
                "itemName": "Mineral Water 500ml",
                "arabicDescription": "مياه معدنية ٥٠٠ مل",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "BTL",
                "brandCode": "AQUA",
                "brandName": "Aqua Pure",
                "categoryCode": "BEVERAGE",
                "categoryName": "Beverages",
                "isActive": "1",
                "isBatchEnabled": True,
                "businessType": "F&B",
                "businessTypeDescription": "Food & Beverage",
                "uomList": [
                    {"uom": "BTL", "conversionFactor": 1},
                    {"uom": "PACK", "conversionFactor": 6},
                    {"uom": "CARTON", "conversionFactor": 24}
                ]
            }
        }
    },
    {
        "name": "Valid Item - Chocolate Bar",
        "expected": 202,
        "data": {
            "material": {
                "itemCode": "CHOCO001",
                "itemName": "Milk Chocolate Bar 50g",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "PCS",
                "brandCode": "GALAXY",
                "brandName": "Galaxy",
                "categoryCode": "CONFECTION",
                "categoryName": "Confectionery",
                "isActive": "1",
                "isBatchEnabled": True,
                "businessType": "F&B",
                "businessTypeDescription": "Food & Beverage - Confectionery",
                "uomList": [
                    {"uom": "PCS", "conversionFactor": 1},
                    {"uom": "BOX", "conversionFactor": 24}
                ]
            }
        }
    },

    # Invalid item tests
    {
        "name": "Invalid - Missing Required Fields",
        "expected": 400,
        "data": {
            "material": {
                "itemCode": "INVALID_ITEM001",
                "itemName": "Incomplete Item"
            }
        }
    },
    {
        "name": "Invalid - Bad isActive Value",
        "expected": 400,
        "data": {
            "material": {
                "itemCode": "INVALID_ITEM002",
                "itemName": "Bad Active Status",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "PCS",
                "brandCode": "TEST",
                "brandName": "Test Brand",
                "categoryCode": "TEST",
                "categoryName": "Test Category",
                "isActive": "YES",
                "isBatchEnabled": True,
                "uomList": [
                    {"uom": "PCS", "conversionFactor": 1}
                ]
            }
        }
    },
    {
        "name": "Invalid - Empty UOM List",
        "expected": 400,
        "data": {
            "material": {
                "itemCode": "INVALID_ITEM003",
                "itemName": "No UOM Item",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "PCS",
                "brandCode": "TEST",
                "brandName": "Test Brand",
                "categoryCode": "TEST",
                "categoryName": "Test Category",
                "isActive": "1",
                "isBatchEnabled": True,
                "uomList": []
            }
        }
    },
    {
        "name": "Invalid - Zero/Negative Conversion Factor",
        "expected": 400,
        "data": {
            "material": {
                "itemCode": "INVALID_ITEM004",
                "itemName": "Bad Conversion Factor",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "PCS",
                "brandCode": "TEST",
                "brandName": "Test Brand",
                "categoryCode": "TEST",
                "categoryName": "Test Category",
                "isActive": "1",
                "isBatchEnabled": True,
                "uomList": [
                    {"uom": "PCS", "conversionFactor": 0},
                    {"uom": "BOX", "conversionFactor": -5}
                ]
            }
        }
    },
    {
        "name": "Update Existing Item - JUICE001",
        "expected": 202,
        "data": {
            "material": {
                "itemCode": "JUICE001",
                "itemName": "Orange Fresh Juice 1L - UPDATED FORMULA",
                "arabicDescription": "عصير برتقال طازج - تركيبة محسنة",
                "salesOrgCode": "AE_DUBAI",
                "baseUOM": "BTL",
                "brandCode": "FRESH",
                "brandName": "Fresh & Pure Premium",
                "categoryCode": "JUICE",
                "categoryName": "Premium Fruit Juices",
                "isActive": "1",
                "isBatchEnabled": True,
                "businessType": "F&B",
                "businessTypeDescription": "Food & Beverage - Premium Line",
                "uomList": [
                    {"uom": "BTL", "conversionFactor": 1},
                    {"uom": "CASE", "conversionFactor": 12},
                    {"uom": "PALLET", "conversionFactor": 600}
                ]
            }
        }
    }
]

# ============================================================================
# MAIN EXECUTION
# ============================================================================

def run_customer_tests():
    """Run all customer test cases"""
    print_header("CUSTOMER INGESTION TESTS")

    for idx, test in enumerate(CUSTOMER_TESTS, 1):
        print_test(idx, test["name"])
        success, response = make_request(
            "/customers/ingest",
            test["data"],
            test["expected"]
        )

        if success:
            if test["expected"] == 202:
                stats["customer_success"] += 1
            else:
                stats["customer_failed"] += 1

        print()  # Blank line
        time.sleep(0.5)  # Small delay between requests

def run_item_tests():
    """Run all item test cases"""
    print_header("ITEM INGESTION TESTS")

    for idx, test in enumerate(ITEM_TESTS, 1):
        print_test(idx, test["name"])
        success, response = make_request(
            "/items/ingest",
            test["data"],
            test["expected"]
        )

        if success:
            if test["expected"] == 202:
                stats["item_success"] += 1
            else:
                stats["item_failed"] += 1

        print()  # Blank line
        time.sleep(0.5)  # Small delay between requests

def print_summary():
    """Print test execution summary"""
    print_header("TEST EXECUTION SUMMARY")

    print(f"{Colors.BOLD}Customer Tests:{Colors.RESET}")
    print(f"  {Colors.GREEN}✓ Success (202):{Colors.RESET} {stats['customer_success']}")
    print(f"  {Colors.RED}✗ Failed (400):{Colors.RESET} {stats['customer_failed']}")

    print(f"\n{Colors.BOLD}Item Tests:{Colors.RESET}")
    print(f"  {Colors.GREEN}✓ Success (202):{Colors.RESET} {stats['item_success']}")
    print(f"  {Colors.RED}✗ Failed (400):{Colors.RESET} {stats['item_failed']}")

    print(f"\n{Colors.BOLD}Total:{Colors.RESET}")
    print(f"  Total Requests: {stats['total_requests']}")
    print(f"  Success Rate: {((stats['customer_success'] + stats['item_success']) / stats['total_requests'] * 100):.1f}%")

    print(f"\n{Colors.MAGENTA}{Colors.BOLD}{'='*80}{Colors.RESET}")
    print(f"{Colors.MAGENTA}{Colors.BOLD}NEXT STEPS:{Colors.RESET}")
    print(f"{Colors.YELLOW}1. Wait 10-15 seconds for background jobs to process{Colors.RESET}")
    print(f"{Colors.YELLOW}2. Open dashboard.html in your browser{Colors.RESET}")
    print(f"{Colors.YELLOW}3. Check the metrics and logs{Colors.RESET}")
    print(f"{Colors.YELLOW}4. Verify data in database:{Colors.RESET}")
    print(f"   {Colors.WHITE}SELECT * FROM log_customer_ingestion;{Colors.RESET}")
    print(f"   {Colors.WHITE}SELECT * FROM log_item_ingestion;{Colors.RESET}")
    print(f"   {Colors.WHITE}SELECT * FROM t_customer;{Colors.RESET}")
    print(f"   {Colors.WHITE}SELECT * FROM t_item;{Colors.RESET}")
    print(f"{Colors.MAGENTA}{Colors.BOLD}{'='*80}{Colors.RESET}\n")

def main():
    """Main execution function"""
    print(f"\n{Colors.BOLD}{Colors.MAGENTA}")
    print("╔════════════════════════════════════════════════════════════════════════════╗")
    print("║                    API TEST DATA GENERATOR                                 ║")
    print("║                                                                            ║")
    print("║  This script will populate your database with comprehensive test data     ║")
    print("║  including valid and invalid scenarios to showcase all functionalities    ║")
    print("╚════════════════════════════════════════════════════════════════════════════╝")
    print(f"{Colors.RESET}\n")

    print_info(f"API Base URL: {API_BASE_URL}")
    print_info(f"Auth Token: {AUTH_TOKEN}")
    print_info(f"Total Tests: {len(CUSTOMER_TESTS)} Customer + {len(ITEM_TESTS)} Item = {len(CUSTOMER_TESTS) + len(ITEM_TESTS)} Total")

    input(f"\n{Colors.CYAN}Press ENTER to start testing...{Colors.RESET}")

    # Run tests
    run_customer_tests()
    run_item_tests()

    # Print summary
    print_summary()

if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print(f"\n\n{Colors.YELLOW}Test execution interrupted by user{Colors.RESET}")
        print_summary()
    except Exception as e:
        print(f"\n{Colors.RED}Fatal error: {str(e)}{Colors.RESET}")

