# Talabat.Users.IntegrationTests

This project contains integration tests for the Talabat.Users module's CustomerService functionality.

## Overview

The integration tests use:
- **Testcontainers** for PostgreSQL database isolation
- **NSubstitute** for mocking external dependencies (Products, Payments modules)
- **FluentAssertions** for readable assertions
- **xUnit** as the test framework

## Test Structure

### Test Classes

Each CustomerService method has its own test class:

- **AddCartItemTests** - Tests for adding items to the customer's cart
- **RemoveCartItemTests** - Tests for removing items from the cart
- **CreateCheckoutSessionTests** - Tests for creating checkout sessions
- **CancelCheckoutSessionTests** - Tests for canceling checkout sessions
- **CheckoutTests** - Tests for the complete checkout flow

### UsersApiFactory

The `UsersApiFactory` is the core test infrastructure that:
1. Spins up a PostgreSQL container using Testcontainers
2. Creates and initializes the UsersDbContext
3. Provides mocked ISender for external module calls
4. Offers helper methods to setup mock responses

### Test Constants

Centralized constants in `TestConstants/Constants.cs` for:
- Customer IDs and emails
- Merchant IDs
- Product IDs and prices
- Addresses
- Payment information

## Running Tests

```bash
dotnet test Talabat.Users.IntegrationTests/Talabat.Users.IntegrationTests.csproj
```

## Test Patterns

### AAA Pattern (Arrange-Act-Assert)

All tests follow the AAA pattern:

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - Setup test data and mocks
    var productId = Constants.Product.Id;
    _factory.SetupProductQuery(productId, productResponse);
    
    // Act - Execute the method under test
    var result = await customerService.AddCartItemAsync(productId, quantity);
    
    // Assert - Verify the results
    result.IsError.Should().BeFalse();
    customer.Cart.Should().NotBeNull();
}
```

### External Dependencies Mocking

External calls to Products and Payments modules are mocked:

```csharp
// Mock product query
_factory.SetupProductQuery(productId, productResponse);

// Mock products query (bulk)
_factory.SetupProductsQuery(productIds, productsResponse);

// Mock payment session creation
_factory.SetupCreatePaymentSession(customerId, sessionId, amount, response);
```

## Key Test Scenarios

### AddCartItemTests
? Adding item to empty cart  
? Updating quantity of existing item  
? Handling non-existent products  
? Preventing merchant mismatch

### RemoveCartItemTests
? Removing existing items  
? Handling non-existent items  
? Removing specific items from multi-item cart

### CreateCheckoutSessionTests
? Creating session with valid cart  
? Handling empty cart  
? Preventing duplicate active sessions  
? Handling missing product data

### CancelCheckoutSessionTests
? Canceling active session  
? Handling no active session

### CheckoutTests
? Successful checkout with valid data  
? Handling missing checkout session  
? Validating customer address  
? Detecting price mismatches

## Database Isolation

Each test uses the same PostgreSQL container instance but operates on seeded customer data. Tests should be designed to:
- Use the pre-seeded customer (ID: `1fb673f4-6974-478b-b4eb-b9882dd13c5f`)
- Clean up by resetting cart and checkout sessions
- Not interfere with other tests

## Notes

- Tests use the actual database context and repository implementations
- External module calls (Products, Payments) are mocked at the ISender level
- Each test class receives a shared `UsersApiFactory` instance via `IClassFixture`
- The PostgreSQL container is started once per test class and disposed after all tests complete
