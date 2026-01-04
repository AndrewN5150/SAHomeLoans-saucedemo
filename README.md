# 🛒 SauceDemo E-Commerce Automation Framework
This repository contains a professional test automation suite for the SauceDemo web application. It is built using C#, .NET 8.0, and Playwright, following the Page Object Model (POM) design pattern to ensure the tests are easy to read and maintain.

## 📋 Prerequisites

Before attempting to run the tests, ensure your environment is configured with the following:

.NET 8.0 SDK: The core runtime for executing C# code. Download here.

PowerShell: Used for running installation scripts.

IDE: Visual Studio 2022 or VS Code (with C# Dev Kit extension).

## 🚀 Getting Started

Follow these steps to initialize the project on your local machine.

1. Initialize the Project
Open your terminal in the project root folder and run the following command to download all necessary libraries:

PowerShell

dotnet restore
2. Build the Solution
Compile the code to ensure the environment is ready:

PowerShell

dotnet build
3. Install Playwright Browsers
Playwright requires specific browser binaries to control Chrome, Firefox, and WebKit. Run this command to install them:

PowerShell

pwsh bin/Debug/net8.0/playwright.ps1 install

## 🧪 Test Execution

You can run the automation suite directly from the command line or via the Test Explorer in Visual Studio.

Run All Tests
To execute the entire regression suite:

PowerShell

dotnet test
Run Specific Features
To run tests related to a specific area (like Authentication or Cart):

PowerShell

dotnet test --filter "Name~AUTH"
dotnet test --filter "Name~CART"

## 📊 Results and Reporting

This framework is designed to provide visual evidence for every test run, which is critical for debugging and auditing.

Test Evidence
After a test run completes, check the TestEvidence folder in the project directory:

TestEvidence/Pass: Contains full-page screenshots of successful test flows.

TestEvidence/Fail: Contains screenshots taken at the exact moment a test encountered an error.

## 📂 Project Structure

tests/: Contains the test logic and assertions (The "What" we are testing).

pages/: Contains the Page Objects and element selectors (The "How" we interact with the UI).

config/: Holds the appsettings.json file for managing environment variables like URLs and credentials.

utils/: Shared helper methods and setup/teardown logic.

## 💡 Code Standards

The tests are written to be "Self-Documenting." Each test follows a clear Setup → Action → Assert flow:

// 1. Action: Add a specific product to the cart
await _inventoryPage.AddProductToCartAsync("sauce-labs-backpack");

// 2. Action: Navigate to the cart view
await _inventoryPage.GoToCartAsync();

// 3. Assert: Verify the item name in the cart matches our selection
await Expect(Page.Locator(".inventory_item_name")).ToHaveTextAsync("Sauce Labs Backpack");
