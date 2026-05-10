# Talabat API Endpoints Documentation

> **Base URL:** `/api`
> **Authentication:** All endpoints require authentication unless marked as **Anonymous**.
> **Authorization:** Endpoints are scoped by caller type — **Admin** (System tenant), **Shop** (Shop tenant), or **Customer** (Customer tenant).

---

## Table of Contents

- [Accounts Module](#accounts-module)
  - [Accounts — Admin](#accounts--admin)
  - [Accounts — Shop](#accounts--shop)
  - [Accounts — All Users](#accounts--all-users)
  - [Roles — Admin](#roles--admin)
  - [Roles — Shop](#roles--shop)
- [Products Module](#products-module)
  - [Products — Admin](#products--admin)
  - [Products — Shop](#products--shop)
  - [Products — Customer](#products--customer)
  - [Shops — Admin](#shops--admin)
  - [Shops — Shop Owner](#shops--shop-owner)
  - [Shops — Customer](#shops--customer)
- [Orders Module](#orders-module)
  - [Orders — Admin](#orders--admin)
  - [Orders — Shop](#orders--shop)
  - [Orders — Customer](#orders--customer)
  - [Checkout Sessions](#checkout-sessions)
- [Users Module](#users-module)
  - [Customer Details](#customer-details)
  - [Cart](#cart)
- [Payments Module](#payments-module)
  - [Webhook](#webhook)
- [Order Processing Module (Legacy)](#order-processing-module-legacy)

---

## Accounts Module

### Accounts — Admin

#### `POST /api/admin/accounts` — Create Account

Creates an account for any tenant type.

| Field | Type | Required | Validation |
|---|---|---|---|
| `UserId` | `Guid` | ✅ | Must be non-empty. |
| `Name` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `Email` | `string` | ✅ | Must be non-empty, valid email. Max 256 characters. |
| `TenantType` | `string` | ✅ | Must be `System`, `Shop`, or `Customer`. |
| `TenantId` | `Guid?` | Conditional | Required when `TenantType` is `Shop`. |
| `RoleIds` | `List<Guid>` | ✅ | Must not be null. No duplicates. Each GUID must be non-empty. |

**Auth:** TenantType `System` + permission `accounts.add`
**Response:** `ErrorOr<Guid>` — the created account ID.

---

#### `PUT /api/admin/accounts/{AccountId}` — Update Account

Updates any account regardless of tenant.

| Field | Type | Required | Validation |
|---|---|---|---|
| `AccountId` | `Guid` | ✅ (route) | Must be non-empty. |
| `Name` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `RoleIds` | `List<Guid>` | ✅ | Must not be null. No duplicates. Each GUID must be non-empty. |

**Auth:** TenantType `System` + permission `accounts.update`
**Response:** `ErrorOr<Success>`

---

#### `DELETE /api/admin/accounts/{AccountId}` — Remove Account

Removes any account regardless of tenant.

| Field | Type | Required | Validation |
|---|---|---|---|
| `AccountId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `System` + permission `accounts.remove`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/admin/accounts` — List Accounts

Lists all accounts with optional filters.

| Field | Type | Required | Validation |
|---|---|---|---|
| `TenantType` | `string?` | ❌ (query) | Optional filter. |
| `TenantId` | `Guid?` | ❌ (query) | Optional filter. |

**Auth:** TenantType `System` + permission `accounts.view`
**Response:** `ErrorOr<List<AccountDto>>`

---

### Accounts — Shop

#### `POST /api/shop/accounts` — Create Account

Creates an account scoped to the shop's tenant. TenantType and TenantId are resolved from the caller's token.

| Field | Type | Required | Validation |
|---|---|---|---|
| `UserId` | `Guid` | ✅ | Must be non-empty. |
| `Name` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `Email` | `string` | ✅ | Must be non-empty, valid email. Max 256 characters. |
| `RoleIds` | `List<Guid>` | ✅ | Must not be null. No duplicates. Each GUID must be non-empty. |

**Auth:** TenantType `Shop` + permission `accounts.add`
**Response:** `ErrorOr<Guid>`

---

#### `PUT /api/shop/accounts/{AccountId}` — Update Account

Updates an account within the shop's tenant (ownership enforced via TenantId from token).

| Field | Type | Required | Validation |
|---|---|---|---|
| `AccountId` | `Guid` | ✅ (route) | Must be non-empty. |
| `Name` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `RoleIds` | `List<Guid>` | ✅ | Must not be null. No duplicates. Each GUID must be non-empty. |

**Auth:** TenantType `Shop` + permission `accounts.update`
**Response:** `ErrorOr<Success>`

---

#### `DELETE /api/shop/accounts/{AccountId}` — Remove Account

Removes an account within the shop's tenant (ownership enforced).

| Field | Type | Required | Validation |
|---|---|---|---|
| `AccountId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Shop` + permission `accounts.remove`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/shop/accounts` — List Accounts

Lists accounts belonging to the shop's tenant.

**Auth:** TenantType `Shop` + permission `accounts.view`
**Response:** `ErrorOr<List<AccountDto>>`

---

### Accounts — All Users

#### `GET /api/accounts/me` — Get My Accounts

Returns all accounts belonging to the authenticated user (resolved from JWT `NameIdentifier` claim).

**Auth:** Any authenticated user (`[Authorize]`)
**Response:** `ErrorOr<List<MyAccountDto>>`

---

### Roles — Admin

#### `POST /api/admin/roles` — Create Role

Creates a role for any tenant type.

| Field | Type | Required | Validation |
|---|---|---|---|
| `Name` | `string` | ✅ | Must be non-empty. Max 100 characters. |
| `Permissions` | `List<string>` | ✅ | Must not be null or empty. No duplicates. Each must be non-empty. |
| `TenantType` | `string` | ✅ | Must be `System`, `Shop`, or `Customer`. |
| `TenantId` | `Guid?` | Conditional | Required when `TenantType` is `Shop`. |

**Auth:** TenantType `System` + permission `accounts.roles.add`
**Response:** `ErrorOr<Guid>`

---

#### `PUT /api/admin/roles/{RoleId}` — Edit Role

Edits any role regardless of tenant.

| Field | Type | Required | Validation |
|---|---|---|---|
| `RoleId` | `Guid` | ✅ (route) | Must be non-empty. |
| `Name` | `string` | ✅ | Must be non-empty. Max 100 characters. |
| `Permissions` | `List<string>` | ✅ | Must not be null or empty. No duplicates. Each must be non-empty. |

**Auth:** TenantType `System` + permission `accounts.roles.edit`
**Response:** `ErrorOr<Success>`

---

#### `DELETE /api/admin/roles/{RoleId}` — Remove Role

Removes any role regardless of tenant.

| Field | Type | Required | Validation |
|---|---|---|---|
| `RoleId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `System` + permission `accounts.roles.remove`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/admin/roles` — List Roles

Lists all roles with optional filters.

| Field | Type | Required | Validation |
|---|---|---|---|
| `TenantType` | `string?` | ❌ (query) | Optional filter. |
| `TenantId` | `Guid?` | ❌ (query) | Optional filter. |

**Auth:** TenantType `System` + permission `accounts.roles.view`
**Response:** `ErrorOr<List<RoleDto>>`

---

### Roles — Shop

#### `POST /api/shop/roles` — Create Role

Creates a role scoped to the shop's tenant.

| Field | Type | Required | Validation |
|---|---|---|---|
| `Name` | `string` | ✅ | Must be non-empty. Max 100 characters. |
| `Permissions` | `List<string>` | ✅ | Must not be null or empty. No duplicates. Each must be non-empty. |

**Auth:** TenantType `Shop` + permission `accounts.roles.add`
**Response:** `ErrorOr<Guid>`

---

#### `PUT /api/shop/roles/{RoleId}` — Edit Role

Edits a role within the shop's tenant (ownership enforced).

| Field | Type | Required | Validation |
|---|---|---|---|
| `RoleId` | `Guid` | ✅ (route) | Must be non-empty. |
| `Name` | `string` | ✅ | Must be non-empty. Max 100 characters. |
| `Permissions` | `List<string>` | ✅ | Must not be null or empty. No duplicates. Each must be non-empty. |

**Auth:** TenantType `Shop` + permission `accounts.roles.edit`
**Response:** `ErrorOr<Success>`

---

#### `DELETE /api/shop/roles/{RoleId}` — Remove Role

Removes a role within the shop's tenant (ownership enforced).

| Field | Type | Required | Validation |
|---|---|---|---|
| `RoleId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Shop` + permission `accounts.roles.remove`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/shop/roles` — List Roles

Lists roles belonging to the shop's tenant.

**Auth:** TenantType `Shop` + permission `accounts.roles.view`
**Response:** `ErrorOr<List<RoleDto>>`

---

## Products Module

### Products — Admin

#### `POST /api/admin/products` — Create Product

Creates a product for any shop.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ShopId` | `Guid` | ✅ | Must be non-empty. |
| `Title` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `BasePrice` | `decimal` | ✅ | Must be greater than zero. |
| `Quantity` | `int` | ✅ | Must not be negative. |

**Auth:** TenantType `System` + permission `products.create`
**Response:** `ErrorOr<Guid>` — wrapped in `AdminCreateProductResponse { ProductId }`.

---

#### `PUT /api/admin/products/{ProductId}` — Update Product

Updates any product regardless of shop.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ (route) | Must be non-empty. |
| `BasePrice` | `decimal` | ✅ | Must be greater than zero. |
| `Quantity` | `int` | ✅ | Must not be negative. |

**Auth:** TenantType `System` + permission `products.update`
**Response:** `ErrorOr<Success>`

---

#### `DELETE /api/admin/products/{ProductId}` — Delete Product

Deletes any product regardless of shop.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `System` + permission `products.delete`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/admin/products` — List Products

Lists all products with optional shop filter.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ShopId` | `Guid?` | ❌ (query) | Optional filter. |

**Auth:** TenantType `System` + permission `products.read`
**Response:** `List<ProductResponse>`

---

#### `GET /api/admin/products/{ProductId}` — Get Product

Gets any product detail.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `System` + permission `products.read`
**Response:** `ErrorOr<ProductResponse>`

---

### Products — Shop

#### `POST /api/shop/products` — Create Product

Creates a product for the shop's own tenant. ShopId resolved from token.

| Field | Type | Required | Validation |
|---|---|---|---|
| `Title` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `BasePrice` | `decimal` | ✅ | Must be greater than zero. |
| `Quantity` | `int` | ✅ | Must not be negative. |

**Auth:** TenantType `Shop` + permission `products.create`
**Response:** `ErrorOr<Guid>` — wrapped in `CreateProductResponse { ProductId }`.

---

#### `PUT /api/shop/products/{ProductId}` — Update Product

Updates a product owned by the shop (ownership enforced via TenantId).

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ (route) | Must be non-empty. |
| `BasePrice` | `decimal` | ✅ | Must be greater than zero. |
| `Quantity` | `int` | ✅ | Must not be negative. |

**Auth:** TenantType `Shop` + permission `products.update`
**Response:** `ErrorOr<Success>`

---

#### `DELETE /api/shop/products/{ProductId}` — Delete Product

Deletes a product owned by the shop (ownership enforced).

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Shop` + permission `products.delete`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/shop/products` — List Products

Lists products belonging to the shop's tenant.

**Auth:** TenantType `Shop` + permission `products.read`
**Response:** `List<ProductResponse>`

---

#### `GET /api/shop/products/{ProductId}` — Get Product

Gets a product owned by the shop (ownership enforced).

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Shop` + permission `products.read`
**Response:** `ErrorOr<ProductResponse>`

---

### Products — Customer

#### `GET /api/products` — List Products

Lists products for browsing. Optional shop filter.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ShopId` | `Guid?` | ❌ (query) | Optional filter. |

**Auth:** TenantType `Customer` + permission `products.read`
**Response:** `List<ProductResponse>`

---

#### `GET /api/products/{ProductId}` — Get Product

Gets a product detail for viewing.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Customer` + permission `products.read`
**Response:** `ErrorOr<ProductResponse>`

---

### Shops — Admin

#### `POST /api/admin/shops` — Create Shop

| Field | Type | Required | Validation |
|---|---|---|---|
| `Name` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `Description` | `string` | ❌ | Max 500 characters. |

**Auth:** TenantType `System` + permission `shops.create`
**Response:** `ErrorOr<Guid>` — wrapped in `AdminCreateShopResponse { ShopId }`.

---

#### `PUT /api/admin/shops/{ShopId}` — Update Shop

Updates any shop.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ShopId` | `Guid` | ✅ (route) | Must be non-empty. |
| `Name` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `Description` | `string` | ❌ | Max 500 characters. |

**Auth:** TenantType `System` + permission `shops.update`
**Response:** `ErrorOr<Success>`

---

#### `DELETE /api/admin/shops/{ShopId}` — Delete Shop

Deletes any shop.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ShopId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `System` + permission `shops.delete`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/admin/shops` — List Shops

Lists all shops.

**Auth:** TenantType `System` + permission `shops.list`
**Response:** `IReadOnlyList<ShopResponse>`

---

#### `GET /api/admin/shops/{ShopId}` — Get Shop

Gets any shop detail.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ShopId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `System` + permission `shops.read`
**Response:** `ErrorOr<ShopResponse>`

---

### Shops — Shop Owner

#### `PUT /api/shop/shops` — Update Own Shop

Updates the shop owned by the caller. ShopId resolved from token.

| Field | Type | Required | Validation |
|---|---|---|---|
| `Name` | `string` | ✅ | Must be non-empty. Max 200 characters. |
| `Description` | `string` | ❌ | Max 500 characters. |

**Auth:** TenantType `Shop` + permission `shops.update`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/shop/shops/me` — Get Own Shop

Gets the shop details for the caller's tenant.

**Auth:** TenantType `Shop` + permission `shops.read`
**Response:** `ErrorOr<ShopResponse>`

---

### Shops — Customer

#### `GET /api/shops` — List Shops

Lists all shops for browsing.

**Auth:** TenantType `Customer` + permission `shops.list`
**Response:** `IReadOnlyList<ShopResponse>`

---

#### `GET /api/shops/{ShopId}` — Get Shop

Gets a shop detail for viewing.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ShopId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Customer` + permission `shops.read`
**Response:** `ErrorOr<ShopResponse>`

---

## Orders Module

### Orders — Admin

#### `POST /api/admin/orders/{OrderId}/cancel` — Cancel Order

Cancels any order regardless of tenant.

| Field | Type | Required | Validation |
|---|---|---|---|
| `OrderId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `System` + permission `orders.cancel`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/admin/orders/{OrderId}` — Get Order

Gets any order detail.

| Field | Type | Required | Validation |
|---|---|---|---|
| `OrderId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `System` + permission `orders.read`
**Response:** `ErrorOr<OrderResponse>`

---

#### `GET /api/admin/orders` — List Orders

Lists all orders with optional filters.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ShopId` | `Guid?` | ❌ (query) | Optional filter. |
| `CustomerId` | `Guid?` | ❌ (query) | Optional filter. |

**Auth:** TenantType `System` + permission `orders.read`
**Response:** `List<OrderResponse>`

---

### Orders — Shop

#### `POST /api/shop/orders/{OrderId}/cancel` — Cancel Order

Cancels an order belonging to the shop (ownership enforced via ShopId from token).

| Field | Type | Required | Validation |
|---|---|---|---|
| `OrderId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Shop` + permission `orders.cancel`
**Response:** `ErrorOr<Success>`

---

#### `POST /api/shop/orders/{OrderId}/ship` — Ship Order

Marks an order as shipped (ownership enforced).

| Field | Type | Required | Validation |
|---|---|---|---|
| `OrderId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Shop` + permission `orders.ship`
**Response:** `ErrorOr<Success>`

---

#### `POST /api/shop/orders/{OrderId}/deliver` — Deliver Order

Marks an order as delivered (ownership enforced).

| Field | Type | Required | Validation |
|---|---|---|---|
| `OrderId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Shop` + permission `orders.deliver`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/shop/orders/{OrderId}` — Get Order

Gets an order belonging to the shop.

| Field | Type | Required | Validation |
|---|---|---|---|
| `OrderId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Shop` + permission `orders.read`
**Response:** `ErrorOr<OrderResponse>`

---

#### `GET /api/shop/orders` — List Orders

Lists orders belonging to the shop's tenant.

**Auth:** TenantType `Shop` + permission `orders.read`
**Response:** `List<OrderResponse>`

---

### Orders — Customer

#### `POST /api/orders/{OrderId}/cancel` — Cancel Order

Cancels an order belonging to the customer (ownership enforced via CustomerId from token).

| Field | Type | Required | Validation |
|---|---|---|---|
| `OrderId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Customer` + permission `orders.cancel`
**Response:** `ErrorOr<Success>`

---

#### `GET /api/orders/{OrderId}` — Get Order

Gets an order belonging to the customer.

| Field | Type | Required | Validation |
|---|---|---|---|
| `OrderId` | `Guid` | ✅ (route) | Must be non-empty. |

**Auth:** TenantType `Customer` + permission `orders.read`
**Response:** `ErrorOr<OrderResponse>`

---

#### `GET /api/orders` — List Orders

Lists orders belonging to the customer.

**Auth:** TenantType `Customer` + permission `orders.read`
**Response:** `List<OrderResponse>`

---

### Checkout Sessions

#### `POST /api/checkout-sessions` — Create Checkout Session

Creates a checkout session from the customer's current cart.

**Auth:** Role `Customer`
**Response:** `ErrorOr<CheckoutSessionDto>`

---

#### `POST /api/checkout-sessions/checkout` — Checkout

Completes checkout by selecting a delivery address and receiving a payment URL.

| Field | Type | Required | Validation |
|---|---|---|---|
| `CheckoutSessionId` | `Guid` | ✅ | Must be non-empty. |
| `AddressId` | `Guid` | ✅ | Must be non-empty. |

**Auth:** Role `Customer`
**Response:** `ErrorOr<CheckoutResponse>` — contains `PaymentId` and `PaymentUrl`.

---

#### `POST /api/checkout-sessions/cancel` — Cancel Checkout Session

Cancels an active checkout session.

| Field | Type | Required | Validation |
|---|---|---|---|
| `CheckoutSessionId` | `Guid` | ✅ | Must be non-empty. |

**Auth:** Role `Customer`
**Response:** `ErrorOr<Success>`

---

## Users Module

### Customer Details

#### `GET /api/customer/details` — Get Customer Details

Returns the authenticated customer's profile including cart and addresses.

**Auth:** TenantType `Customer` + permission `users.read`
**Response:** `ErrorOr<CustomerDetailsResponse>`

```json
{
  "cart": {
    "shopId": "guid",
    "items": [
      { "productId": "guid", "quantity": 1 }
    ]
  },
  "addresses": [
    { "id": "guid", "address": "string" }
  ]
}
```

---

### Cart

#### `POST /api/cart/items` — Add Cart Item

Adds or updates a product in the customer's cart.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ | Must be non-empty. |
| `Quantity` | `int` | ✅ | Must be greater than zero. |

**Auth:** Role `Customer`
**Response:** `ErrorOr<Success>`

---

#### `DELETE /api/cart/items` — Remove Cart Item

Removes a product from the customer's cart.

| Field | Type | Required | Validation |
|---|---|---|---|
| `ProductId` | `Guid` | ✅ | Must be non-empty. |

**Auth:** Role `Customer`
**Response:** `ErrorOr<Success>`

---

## Payments Module

### Webhook

#### `POST /api/payments/webhook` — Payment Webhook

Receives payment provider callbacks. **Anonymous** — no authentication required.

| Field | Type | Required | Description |
|---|---|---|---|
| `EventType` | `enum` | ✅ | One of: `PaymentSucceeded`, `PaymentFailed`, `PaymentRefunded`, `PaymentRefundFailed`. |
| `PaymentId` | `Guid` | ✅ | The payment identifier. |
| `Amount` | `decimal` | ✅ | The payment amount. |

**Auth:** Anonymous
**Response:** `ErrorOr<Success>`

---

## Order Processing Module (Legacy)

> ⚠️ These endpoints appear to be from an older iteration. They use `AllowAnonymous()` and a different service pattern (`IOrderService`). They may be deprecated in favor of the Orders module above.

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/start-checkout-session` | Start a checkout session (legacy). |
| `POST` | `/api/orders/cancel` | Cancel an order (legacy). |
| `POST` | `/api/orders/ship` | Ship an order (legacy). |
| `POST` | `/api/orders/deliver` | Deliver an order (legacy). |

---

## Route Pattern Summary

| Prefix | Caller | Auth Policy |
|---|---|---|
| `/api/admin/...` | System admin | TenantType `System` |
| `/api/shop/...` | Shop owner/staff | TenantType `Shop` |
| `/api/...` (no prefix) | Customer | TenantType `Customer` or Role `Customer` |

## Permissions Reference

### Accounts (`accounts.*`)
| Permission | Admin | Shop | Customer |
|---|---|---|---|
| `accounts.add` | ✅ | ✅ | ❌ |
| `accounts.update` | ✅ | ✅ | ❌ |
| `accounts.remove` | ✅ | ✅ | ❌ |
| `accounts.view` | ✅ | ✅ | ❌ |
| `accounts.roles.add` | ✅ | ✅ | ❌ |
| `accounts.roles.edit` | ✅ | ✅ | ❌ |
| `accounts.roles.remove` | ✅ | ✅ | ❌ |
| `accounts.roles.view` | ✅ | ✅ | ❌ |

### Products (`products.*`)
| Permission | Admin | Shop | Customer |
|---|---|---|---|
| `products.create` | ✅ | ✅ | ❌ |
| `products.read` | ✅ | ✅ | ✅ |
| `products.update` | ✅ | ✅ | ❌ |
| `products.delete` | ✅ | ✅ | ❌ |

### Shops (`shops.*`)
| Permission | Admin | Shop | Customer |
|---|---|---|---|
| `shops.create` | ✅ | ❌ | ❌ |
| `shops.read` | ✅ | ✅ | ✅ |
| `shops.update` | ✅ | ✅ | ❌ |
| `shops.delete` | ✅ | ❌ | ❌ |
| `shops.list` | ✅ | ✅ | ✅ |

### Orders (`orders.*`)
| Permission | Admin | Shop | Customer |
|---|---|---|---|
| `orders.read` | ✅ | ✅ | ✅ |
| `orders.cancel` | ✅ | ✅ | ✅ |
| `orders.ship` | ❌ | ✅ | ❌ |
| `orders.deliver` | ❌ | ✅ | ❌ |

### Users (`users.*`)
| Permission | Admin | Shop | Customer |
|---|---|---|---|
| `users.read` | ✅ | ❌ | ✅ |
| `users.update` | ✅ | ❌ | ✅ |
