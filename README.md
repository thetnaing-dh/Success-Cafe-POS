# Cafe POS System
A comprehensive Point of Sale (POS) system for cafes and restaurants built with C#, SQLite database, and Report Viewer.

![CoverImage](https://github.com/thetnaing-dh/Success-Cafe-POS/blob/master/Main%20Menu.png?raw=true)

## Features
### Table Management
* Table Groups: Organize tables into logical groups
* Table Creation: Create and configure new tables
* Table Check-In: Manage customer seating
* Table Change: Move orders between tables
* Merge Table: Combine multiple tables into a single order

### Menu Management
* Menu Categories: Categorize menu items
* Menu Items: Full CRUD operations for menu items
* Item Images: Support for product images

### Customer Management
* Customer Database: Store customer information
* Member Cards: Implement loyalty programs
* Member Discount: Apply discounts for members

### Order Processing
* Print to Kitchen: Send orders directly to kitchen printers
* Print Bill: Generate customer bills
* Split Bill: Divide bills among multiple customers
* Item Take Away: Handle takeaway orders
* Item FOC: Mark items as complimentary
* Discounts: Apply various discount types (% or amount)
* Service Charges: Add service fees
* Government Tax: Calculate and apply taxes

### Reservation System
* Reservation Order: Pre-book orders for reservations
* Advance Paid: Handle advance payments for reservations
* Order to Delivery: Manage delivery orders

### Financial Management
* Expenses Tracking: Record and categorize business expenses
* Daily Summary: Dashboard with key performance indicators

### Reporting
* Daily Sales: Summary of daily sales performance
* Sale by Each Invoice: Detailed invoice reporting
* Sale by Table: Table sales analysis
* Top Sale Items: Item sales analysis
* Daily Incomes: Income tracking report
* Daily Expenses: Expense tracking report
* Profit and Loss: Comprehensive financial statement

### System Administration
* User Permissions: Role-based access control
* Database Backup and Restore: Data protection utilities
* System Registration: Software licensing and activation

### Technology Stack
* Programming Language: C#
* Database: SQLite 
* Reporting: Report Viewer (RDLC)
* UI Framework: Windows Forms (WinForms)

## Installation
* Download the latest Release : [SuccessCafePOS](https://github.com/thetnaing-dh/Success-Cafe-POS/releases/tag/v1.0.0)
* Install the setup file
* Enjoy the application in window app menu or on desktop.

## Database Setup
* The system uses SQLite which requires no additional setup. The database file will be created automatically in the AppData folder on first run.

## Usage
1. Login: Use the default admin credentials (admin) or create new users
2. Configure Settings: Set up system information, system password, invoice no text, tax rates, service charges, and printer settings
3. Create Menu Items: Add categories and menu items
4. Set Up Tables: Create table groups and individual tables
5. Process Orders: Start taking orders, applying discounts, and printing bills
6. Expenses : Add daily expenses 
7. Generate Reports: Access various reports from the reporting menu

## Printer Setup
The system supports both kitchen printers and bill printers. Configure printers through:
* Settings → Printer Configuration

## Default Login Credentials
* Username: admin
* Password: 
### Important: Change the default password after first login in the setting menu.

## Backup and Restore
Regular database backups are recommended. Use the built-in backup utility:
1. Go to Setting → Database Backup
2. Select backup location
3. Restore using Administration → Database Restore

## Reporting
The system includes comprehensive reporting capabilities using Report Viewer:
1. Daily sales reports
2. Invoice details
3. Item performance
4. Table performance
5. Financial statements
6. Expense reports

## License
This software requires registration. Please contact support for licensing information.

## Support
For technical support or feature requests, please contact:
Email: spisoftware.mm@gmail.com
