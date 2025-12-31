using FluentAssertions;
using Microsoft.Playwright.NUnit;

namespace SAHomeLoansSauceDemo.Tests;

[Parallelizable(ParallelScope.All)]
[TestFixture]
public class ApiTests : PageTest
{
  [Test]
  public async Task GivenValidDetails_WhenUserRegisters_ThenRegistrationIsSuccessful()
  {
    // Arrange

    // All requests sent start with this API endpoint.
    var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
    var baseURL = await playwright.APIRequest.NewContextAsync(new() { BaseURL = "https://reqres.in/api/" });

    // Act

    // This is the POST request to register a user
    var request = await baseURL.PostAsync("register", new()
    {
      DataObject = new
      {
        email = "eve.holt@reqres.in",
        password = "pistol"
      }
    });

    var response = await request.JsonAsync();

    // Assert
    request.Status.Should().Be(200);

    response.Value.GetProperty("id").ToString().Should().NotBeNullOrEmpty();
    response.Value.GetProperty("token").GetString().Should().Be("QpwL5tke4Pnpja7X4");
  }

  [Test]
  public async Task GivenUserId_WhenFetchingUser_ThenUserDetailsAreReturned()
  {
    // Arrange

    // All requests sent start with this API endpoint.
    var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
    var baseURL = await playwright.APIRequest.NewContextAsync(new() { BaseURL = "https://reqres.in/api/" });

    // Act

    // This is the request to GET user number 1
    var request = await baseURL.GetAsync("users/1");

    var response = await request.JsonAsync();

    //Assert
    response.Value.GetProperty("data").GetProperty("id").ToString().Should().NotBeNullOrEmpty();
    response.Value.GetProperty("data").GetProperty("email").GetString().Should().Be("george.bluth@reqres.in");
    response.Value.GetProperty("data").GetProperty("first_name").GetString().Should().Be("George");
    response.Value.GetProperty("data").GetProperty("last_name").GetString().Should().Be("Bluth");
  }

  [Test]
  public async Task GivenUserId_WhenUserIsDeleted_ThenUserIsRemoved()
  {
    //Arrange

    // All requests sent start with this API endpoint.
    var baseURL = await Playwright.APIRequest.NewContextAsync(new() { BaseURL = "https://reqres.in/api/" });

    // This is the request to DELETE user number 2
    var request = await baseURL.DeleteAsync("users/2");

    //Assert
    request.Status.Should().Be(204);
  }
}
