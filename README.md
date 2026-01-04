# SAHomeLoans SauceDemo Playwright Project

## Description
This project uses [Playwright](https://playwright.dev/dotnet) for end-to-end testing of web applications.

I wrote 3 UI tests for https://www.saucedemo.com. These tests cover:
1. User Login Process
2. Navigating to the Checkout Page
3. Completion of the Purchase Process

I found these 3 UI tests to be the most beneficial, since they test the entire successfuly process flow.
If i was testing the website (https://www.saucedemo.com) professionaly, i would add these tests.
- Positive tests 
	- The your Cart page has discriptions i would test to ensure they are correct.
	- On the information page i would test First Name, Last Name, Zip Code, and the Continue button
	To name a few
- Negitive tests
	- The login screen has Username and Password details that simulate diffrent failiers 
	- On the information page i would test validation for First Name, Last Name and Zip Code
	To name a few

## Installation
Follow these steps to set up the project:

```bash
# Navigate to the project directory
cd SAHomeLoans-saucedemo

# Install dependencies
dotnet restore
```

Usage
To run the tests, use the following command:

# Run all tests
dotnet test

## Contributing
Contributions are welcome! Please follow these steps to contribute:

Fork the repository.
Create a new branch (git checkout -b feature-branch).
Make your changes.
Commit your changes (git commit -m 'Add new feature').
Push to the branch (git push origin feature-branch).
Open a pull request.

Improvments:

I would add a reporting feature to the CI/CD pipeline to provide a detailed report of the test results. (https://github.com/AndrewN5150/Task2/blob/master/.github/workflows/playwrightdocker.yml)
