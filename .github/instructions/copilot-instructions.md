# copilot-instructions.md

This project contains RESTful APIs built with **.NET (C#)** using **ASP.NET Core**. The following guidelines and best practices should be followed when generating or modifying code.
Organization name SolidifyDemo and the repository MythAPI

---

## 🧪 Test-Driven Development (TDD)
Always follow the TDD workflow:
Write a failing test for the new feature or bug fix before writing any implementation code.
Run the test to confirm it fails (red).
Implement the minimum code required to make the test pass.
Run the test again to confirm it passes (green).
Refactor the code as needed, ensuring all tests remain green.
Repeat this cycle for each new feature or change.
All new code must be covered by tests written first.
Use NUnit and NSubstitute for all unit tests and mocking.
Use the arrange-act-assert pattern and only one assertion per test.
Ensure tests are isolated and do not depend on external state.
Copilot must always use this TDD process: tests first, then implementation, then refactor.

## Technologies used
- .NET 8.0
- Always use built-in ASP.NET Core features for .NET 8.0
- Serilog for logging

--- 

## 📝 Branching
- Prefix all branches with the issue number.
- After issue number, branch names should be prefixed with feature or bug separated with a dash. 

--- 

## 🔐 Security
- Always make sure that rate limiting is in place for all API endpoints

## 🧪 Testing

- Use only NSubstitute for mocking dependencies.
- Use **NUnit** for unit tests.
- Write tests for:
  - Controllers
  - Services
  - Critical validation and logic
- Favor **integration tests** for full API pipelines (e.g., using `WebApplicationFactory`).
- Use the arrange act assert pattern
- Only do one assertion per test
- Use the constraint model for assertions
- Ensure tests are isolated and do not depend on external state

---
