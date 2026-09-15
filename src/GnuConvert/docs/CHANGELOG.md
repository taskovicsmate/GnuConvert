# Changelog

All notable changes to SmartAccountingConverter are documented in this file.

This project follows a structured development process with milestone-based versioning to ensure maintainability, traceability, and production readiness.

---

## [v0.1.0] — Project Foundation

### Added

* Initial WPF project setup using MVVM architecture
* separation of Views, ViewModels, Models, and Services
* base project folder structure for scalable development
* Git repository initialization
* public GitHub repository preparation
* professional project documentation structure
* README.md with full project overview
* ROADMAP.md and development documentation
* .gitignore configuration for C# WPF + ASP.NET Core environment
* LICENSE file for project ownership protection

### Implemented

* first LoginView UI mockup
* initial XAML structure for authentication screen
* LoginViewModel foundation
* navigation flow from LoginView to MainView
* temporary JSON-based user data loading

### Purpose

This version establishes the architectural and documentation foundation required for stable long-term development.

---

## [v0.2.0] — Main Interface and Navigation

### Added

* MainView layout planning
* side navigation menu structure
* application header design
* main application pages:

  * Conversion
  * Settings
  * Help

### Implemented

* MVVM-based navigation system
* content switching between views
* initial dashboard workflow structure

### Improved

* overall application usability planning
* interface consistency strategy

### Purpose

This version creates the operational shell of the desktop application and defines the main workflow structure.

---

## [v0.3.0] — File Import and Upload System

### Added

* drag-and-drop file upload interface
* manual file selection support
* CSV import processing
* DataGrid preview for uploaded files

### Implemented

* initial CSV validation rules
* missing file detection
* invalid structure detection
* early-stage error handling for import failures

### Purpose

This version introduces the first real accounting workflow by enabling file input and validation.

---

## [v0.4.0] — Single File Conversion Engine

### Added

* ledger account reference file support
* accounting structure definition loading
* output CSV generation

### Implemented

* first conversion logic for accounting mapping
* accounting data transformation rules
* RLB-compatible export preparation

### Purpose

This version delivers the first business-critical milestone: successful conversion from one source file into usable accounting output.

---

## [v0.5.0] — Dual File Conversion System

### Added

* simultaneous handling of two input files
* comparison engine between accounting datasets
* third output file generation

### Implemented

* matching logic between accounting records
* duplicate detection
* validation of cross-file consistency

### Purpose

This version supports more realistic bookkeeping workflows requiring reconciliation between multiple financial sources.

---

## [v0.6.0] — Settings and Configuration

### Added

* SettingsView interface
* default working directory selection
* output structure configuration

### Implemented

* configurable business workflow preferences
* preparation for persistent user settings

### Purpose

This version improves operational flexibility and reduces manual repetitive setup for accounting offices.

---

## [v0.7.0] — UI Refinement and Stability

### Added

* icon system integration
* improved visual hierarchy
* unified design structure

### Improved

* layout responsiveness
* color consistency
* business-focused usability
* preparation for light/dark mode support

### Implemented

* standardized user-facing error messages
* file-based logging system (log.txt)
* improved exception handling
* prevention of silent failures

### Purpose

This version focuses on production quality, professional appearance, and reliability.

---

## [v0.8.0] — User Management System (Planned)

### Planned

* SQLite database integration
* UserModel implementation
* registration workflow
* secure password hashing
* password reset functionality
* role-based permission system
* authentication unit testing

### Purpose

This version transitions the system from prototype authentication to production-ready user management.

---

## [v0.9.0] — Subscription and License Management (Planned)

### Planned

* Stripe sandbox integration
* ASP.NET Core backend API
* subscription validation endpoint
* WPF payment integration
* startup license verification
* expiration notifications
* privacy policy integration
* API unit testing

### Purpose

This version enables the SaaS business model and subscription-based product delivery.

---

## [v1.0.0] — Production Release (Target)

### Planned

* full functional testing
* integration testing
* pilot release validation
* installer package creation (ClickOnce / MSIX)
* automatic updates
* user manual creation
* Help window implementation
* release tagging and versioning
* first stable production deployment

### Purpose

This version represents the first full production-ready release for accounting firms.

---

# Versioning Strategy

Versioning follows milestone-based progression:

* v0.x → active development phase
* v1.0.0 → first stable production release
* v1.x → business expansion and optimization
* v2.x → enterprise scaling and advanced automation

The goal is controlled growth, not chaotic feature accumulation.
