# API Automation Framework

[![API Automation Framework CI](https://github.com/yousufwaqar/api-automation-framework/actions/workflows/ci.yml/badge.svg)](https://github.com/yousufwaqar/api-automation-framework/actions/workflows/ci.yml)

Production-ready API automation proof of concept built using **.NET 9**, **C#**, **Reqnroll**, **xUnit**, **RestSharp**, **FluentAssertions**, and **Serilog**.

The framework follows API Client / API Object design patterns, Dependency Injection, separation of concerns, reusable DTOs, and BDD practices.

## Test Status

| Test Suite | Scenarios | Status |
|---|---:|---|
| Authentication API | 4 | Passing |
| Posts API | 8 | Passing |
| User API CRUD Operations | 10 | Passing |
| **Total** | **22** | **22 Passed** |

## Technology Stack

| Area | Technology |
|---|---|
| Language | C# |
| Runtime | .NET 9 |
| BDD Framework | Reqnroll |
| Test Runner | xUnit |
| HTTP Client | RestSharp |
| Assertions | FluentAssertions |
| JSON Serialization | Newtonsoft.Json |
| Logging | Serilog |
| Retry / Resilience | Polly |
| Test Data Generation | Bogus |
| Schema Utilities | NJsonSchema |
| CI/CD | GitHub Actions |

## Framework Capabilities

- BDD scenarios written in Gherkin
- Reusable API Client Pattern
- Request and response DTOs
- RestSharp HTTP abstraction
- Dependency Injection using Reqnroll BoDi container
- Centralized configuration through `appsettings.json`
- Environment switching through `TEST_ENVIRONMENT`
- Serilog request, response, error, and scenario logging
- Correlation IDs added to API requests
- Retry mechanism for transient API failures
- FluentAssertions for readable validations
- Positive, negative, CRUD, and authentication tests
- Dynamic/random test data generation using Bogus
- Test execution reports and logs
- GitHub Actions CI pipeline

## Project Structure

```text
ApiAutomationFramework/
│
├── .github/
│   └── workflows/
│       └── ci.yml
│
├── ApiAutomationFramework.Tests/
│   ├── APIClients/
│   │   ├── Base/
│   │   ├── Interfaces/
│   │   ├── UserApiClient.cs
│   │   └── PostApiClient.cs
│   │
│   ├── Configuration/
│   ├── Constants/
│   ├── DTOs/
│   │   ├── Request/
│   │   └── Response/
│   ├── Features/
│   ├── Helpers/
│   ├── Hooks/
│   ├── StepDefinitions/
│   ├── TestData/
│   ├── Utilities/
│   ├── appsettings.json
│   └── ApiAutomationFramework.Tests.csproj
│
├── ApiAutomationFramework.slnx
└── README.md