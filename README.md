# Enterprise API Automation Framework

[![CI](https://github.com/yousufwaqar/api-automation-framework/actions/workflows/ci.yml/badge.svg)](https://github.com/yousufwaqar/api-automation-framework/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![Tests](https://img.shields.io/badge/Tests-30%20Passed-success)](https://github.com/yousufwaqar/api-automation-framework/actions)
[![Patterns](https://img.shields.io/badge/Design%20Patterns-10%2B-blueviolet)](https://github.com/yousufwaqar/api-automation-framework)

Production-ready API test automation framework built with .NET 9, Reqnroll, RestSharp, xUnit, FluentAssertions, Serilog, and Polly. Follows Clean Architecture, SOLID principles, and multiple design patterns.

## Test Coverage

| Feature | Scenarios | Status |
|---------|-----------|--------|
| User API CRUD | 11 | Passing |
| Posts API | 7 | Passing |
| Authentication | 4 | Passing |
| Advanced Patterns | 8 | Passing |
| **Total** | **30** | **All Passing** |

## Design Patterns

### Structural

| Pattern | Class |
|---------|-------|
| API Client Pattern | UserApiClient, PostApiClient |
| Facade Pattern | ApiTestFacade |
| Adapter Pattern | DTOs with JsonProperty |

### Creational

| Pattern | Class |
|---------|-------|
| Factory Pattern | TestDataFactory |
| Builder Pattern | UserRequestBuilder, BulkOperationBuilder |
| Singleton Pattern | FrameworkConfigurationManager |

### Behavioral

| Pattern | Class |
|---------|-------|
| Template Method | BaseApiClient |
| Strategy Pattern | RetryHelper with Polly |
| Specification Pattern | ResponseSelector |
| Dependency Injection | DependencyInjectionHooks |

## Technology Stack

| Category | Technology |
|----------|------------|
| Language | C# 13 |
| Runtime | .NET 9 |
| BDD | Reqnroll 3.3.4 |
| Test Runner | xUnit 2.9.3 |
| HTTP Client | RestSharp 114.0.0 |
| Assertions | FluentAssertions 8.10.0 |
| JSON | Newtonsoft.Json 13.0.4 |
| Logging | Serilog 4.4.0 |
| Resilience | Polly 8.7.0 |
| Data Generation | Bogus 35.6.5 |
| Schema Validation | NJsonSchema 11.6.1 |
| CI/CD | GitHub Actions |

## Project Structure

The framework follows Clean Architecture with clear separation of concerns.

- APIClients folder contains all HTTP interactions
- DTOs folder contains Request and Response data models
- Features folder contains Gherkin feature files
- StepDefinitions folder contains BDD step implementations
- Helpers folder contains Factory, Facade, Builder, and Selector implementations
- Hooks folder contains DI configuration and test lifecycle hooks
- Configuration folder handles multi-environment settings
- Constants folder centralizes string constants

## Architecture Layers

1. Specification Layer - Gherkin Feature Files
2. Orchestration Layer - Step Definitions
3. Pattern Layer - Facades, Factories, Builders, Selectors
4. Abstraction Layer - API Client Interfaces
5. Implementation Layer - Concrete API Clients
6. Infrastructure Layer - BaseApiClient with logging and retry
7. HTTP Layer - RestSharp

Dependencies flow downward only. Each layer has a single responsibility.

## APIs Under Test

The framework tests two different public APIs.

### ReqRes (reqres.in)

Used for User CRUD and authentication scenarios. Supports full CRUD, pagination, authentication with API keys.

### JSONPlaceholder (jsonplaceholder.typicode.com)

Used for Post CRUD scenarios. Different response structure than ReqRes, returns arrays, no authentication required.

Two different APIs prove the framework is service-agnostic and easily extensible.

## Prerequisites

- .NET 9 SDK
- Visual Studio 2022 or JetBrains Rider or VS Code
- Git

## Setup

Clone the repository, restore packages, and build.

    git clone https://github.com/yousufwaqar/api-automation-framework.git
    cd api-automation-framework/ApiAutomationFramework.Tests
    dotnet restore
    dotnet build --configuration Release

## Running Tests

### All Tests

    dotnet test --configuration Release

### By Category

    dotnet test --filter "Category=smoke"
    dotnet test --filter "Category=regression"
    dotnet test --filter "Category=patterns"
    dotnet test --filter "Category=factory"
    dotnet test --filter "Category=facade"
    dotnet test --filter "Category=builder"
    dotnet test --filter "Category=selector"
    dotnet test --filter "Category=negative"

### By Feature

    dotnet test --filter "FullyQualifiedName~UserAPI"
    dotnet test --filter "FullyQualifiedName~PostsAPI"
    dotnet test --filter "FullyQualifiedName~AdvancedPatterns"

Expected output: 30 tests passed, 0 failed.

## Configuration

Configuration is in ApiAutomationFramework.Tests/appsettings.json. It contains base URLs, timeouts, retry counts, and API keys for each API.

Configuration loads in this order:
1. appsettings.json (base)
2. appsettings.Environment.json (overrides)
3. Environment variables (CI secrets)

## ReqRes API Key

ReqRes requires an API key. Get one free at https://app.reqres.in/api-keys.

Do not commit real API keys to public repositories. Use GitHub Secrets for CI/CD.

## Environment Switching

Set the TEST_ENVIRONMENT variable.

Windows:

    set TEST_ENVIRONMENT=Development
    dotnet test

Linux or macOS:

    TEST_ENVIRONMENT=Staging dotnet test

## Test Categories

Tests are tagged for flexible execution.

| Tag | Purpose |
|-----|---------|
| smoke | Critical path tests |
| regression | Full regression suite |
| negative | Error and edge cases |
| crud | CRUD operations |
| auth | Authentication |
| users | User API tests |
| posts | Post API tests |
| patterns | Design pattern demonstrations |
| factory | Factory Pattern scenarios |
| facade | Facade Pattern scenarios |
| builder | Builder Pattern scenarios |
| selector | Selector Pattern scenarios |

## Framework Capabilities

- BDD scenarios in plain English Gherkin
- Reusable API Client Pattern
- Strongly-typed Request and Response DTOs
- Complex nested payload support
- Multiple design patterns implemented
- Dependency Injection through Reqnroll BoDi
- Multi-environment configuration
- Structured logging with Serilog
- Correlation IDs for request tracing
- Automatic retry with Polly
- FluentAssertions for readable validations
- Dynamic test data generation with Bogus
- JSON schema validation
- Test execution reports and logs
- GitHub Actions CI/CD pipeline
- Multi-API testing capability
- Custom exception hierarchy

## CI/CD Pipeline

Automated pipeline runs on every push to main and develop branches.

Pipeline stages:
1. Checkout repository
2. Setup .NET 9 SDK
3. Restore NuGet packages
4. Build in Release configuration
5. Run all 30 tests
6. Upload test results as artifacts
7. Publish test summary

View pipeline runs at the Actions tab on GitHub.

## Adding a New API

The framework supports adding new APIs without modifying existing code.

Steps:
1. Add configuration in appsettings.json
2. Add endpoints in Constants/ApiEndpoints.cs
3. Create Request and Response DTOs
4. Create Interface in APIClients/Interfaces
5. Create Implementation extending BaseApiClient
6. Register in DependencyInjectionHooks.cs
7. Create Feature file
8. Create Step Definitions

This is the Open-Closed Principle in practice.

## Reports and Logs

Structured logs are generated at Reports/Logs/test-execution-date.log during execution.

Test results are saved at Reports/Run_timestamp/test-results.json.

Logs include timestamp, log level, correlation ID, scenario title, request and response details, and timing information.

## Author

Yousuf Waqar

GitHub: @yousufwaqar

## Acknowledgments

Built as a proof of concept demonstrating Clean Architecture, SOLID principles, multiple design patterns, modern .NET 9 features, and enterprise CI/CD practices.

## License

Available for educational and evaluation purposes.
