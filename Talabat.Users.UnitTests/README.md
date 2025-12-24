# Talabat.Users.UnitTests

This project contains unit tests for the Talabat.Users domain module, following Domain-Driven Design (DDD) principles.

## Project Structure

```
Talabat.Users.UnitTests/
??? CustomerAggregate/
?   ??? CustomerTests.cs          # Tests for Customer aggregate root (25 tests)
?   ??? CartTests.cs              # Tests for Cart entity (7 tests)
?   ??? CheckoutSessionTests.cs   # Tests for CheckoutSession entity (8 tests)
??? MerchantAggregate/
?   ??? MerchantTests.cs          # Tests for Merchant aggregate root (5 tests)
??? TestConstants/
?   ??? Constants.Customer.cs     # Customer-related test constants
?   ??? Constants.Merchant.cs     # Merchant-related test constants
?   ??? Constants.Cart.cs         # Cart-related test constants
?   ??? Constants.CheckoutSession.cs  # CheckoutSession-related test constants
?   ??? Constants.Product.cs      # Product-related test constants
?   ??? Constants.Address.cs      # Address-related test constants
??? TestUtils/
?   ??? Factories/
?       ??? CustomerFactory.cs    # Factory for creating Customer test instances
?       ??? MerchantFactory.cs    # Factory for creating Merchant test instances
?       ??? CheckoutItemFactory.cs # Factory for creating CheckoutItem test instances
??? Talabat.Users.UnitTests.csproj
```

## Testing Framework & Libraries

- **xUnit**: Testing framework
- **FluentAssertions**: Fluent assertion library for more readable assertions
- **NSubstitute**: Mocking library (for future service/repository tests)

## Testing Patterns

All tests follow the **AAA (Arrange-Act-Assert)** pattern:

```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange: Set up test data and preconditions
    var customer = CustomerFactory.Create();
    
    // Act: Execute the method being tested
    var result = customer.AddAddress("123 Test St");
    
    // Assert: Verify the expected outcome
    result.Should().NotBeNull();
}
```

## Test Constants

Test constants are organized in partial classes under the `Constants` namespace. This allows for:
- Consistent test data across all tests
- Easy maintenance when values need to change
- Clear separation of test data by domain concept

Example:
```csharp
public static partial class Constants
{
    public static class Customer
    {
        public static readonly Guid Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly string Email = "customer@test.com";
    }
}
```

## Test Factories

Factories use reflection to create instances of internal domain entities, providing:
- Simplified test setup
- Reusable object creation logic
- Pre-configured test scenarios (e.g., `CreateWithCart`, `CreateWithAddress`)

Example:
```csharp
var customer = CustomerFactory.Create(email: "test@test.com");
var customerWithCart = CustomerFactory.CreateWithCart();
```

## Test Coverage

### Customer Aggregate (25 tests)
- Customer creation
- Address management
- Cart operations (create, update, remove, reset)
- Checkout session management
- Error scenarios (merchant mismatch, cart not found, etc.)

### Cart Entity (7 tests)
- Cart creation
- Adding/updating items
- Removing items
- Error handling

### CheckoutSession Entity (8 tests)
- Session creation
- Address assignment
- Total price calculation
- Items management

### Merchant Aggregate (5 tests)
- Merchant creation
- Entity equality
- HashCode behavior

## Known Issues

There is a bug in the `CheckoutItem` domain model where constructor parameters are not assigned to properties. The unit tests document this behavior with comments. Once fixed, the TotalPrice tests should be updated.

## Running the Tests

```bash
# Run all tests
dotnet test Talabat.Users.UnitTests/Talabat.Users.UnitTests.csproj

# Run with detailed output
dotnet test Talabat.Users.UnitTests/Talabat.Users.UnitTests.csproj --verbosity detailed

# Run specific test class
dotnet test --filter "FullyQualifiedName~CustomerTests"
```

## Adding New Tests

1. Create test class in appropriate aggregate folder
2. Follow AAA pattern
3. Use existing factories and constants
4. Add new constants/factories as needed
5. Ensure test names follow convention: `MethodName_Scenario_ExpectedBehavior`

## Internal Visibility

The `Talabat.Users` project exposes its internal types to the test project via:
```csharp
[assembly: InternalsVisibleTo("Talabat.Users.UnitTests")]
```
This is declared in `Talabat.Users/Properties/AssemblyInfo.cs`.
