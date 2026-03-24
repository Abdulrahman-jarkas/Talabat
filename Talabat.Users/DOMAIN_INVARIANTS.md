# Domain Invariants - Users Module

## Single Vendor Cart Restriction

**Description**: A customer's cart can only contain products from a single vendor at any given time.

**Reasoning**: Ensures order fulfillment efficiency and consistency by preventing mixed vendor orders, which simplifies logistics, payment processing, and delivery coordination.

---

## Checkout Session Price Acknowledgment

**Description**: A checkout session must be created with the exact prices that the customer has acknowledged and agreed upon.

**Reasoning**: Establishes a clear price contract between the customer and the system at the time of checkout initiation, preventing disputes and ensuring transparency in the transaction process.

---

## Cart Immutability During Active Checkout

**Description**: A customer cannot update their cart while an active checkout session exists. The checkout session must be canceled before any cart modifications can be made.

**Reasoning**: Prevents inconsistencies between the checkout session and cart state, ensuring that the prices and items being processed for payment accurately reflect what the customer intends to purchase.

---

## Checkout Session Automatic Cancellation

**Description**: A checkout session must be automatically canceled when:
- Products in the cart are updated (added, removed, or quantity changed)
- A product in the session no longer exists at the vendor

**Reasoning**: Maintains data integrity and prevents processing payments for outdated or unavailable items. Ensures the checkout session always reflects the current state of available products and their prices.

---

## Post-Payment Cart and Session Reset

**Description**: When a customer successfully completes checkout and payment, both the checkout session and cart must be reset to their initial empty states.

**Reasoning**: Prevents duplicate orders and ensures a clean state for the next shopping experience. Marks the successful completion of the transaction lifecycle.

---

## Checkout Session Price Integrity Validation

**Description**: Before processing payment, the system must verify that the current product prices match the prices stored in the checkout session. The total price must be calculated from the checkout session, not from live product prices.

**Reasoning**: Ensures customers pay the exact amount they agreed to during checkout session creation, protecting against price manipulation and race conditions where prices might change between checkout initiation and payment completion.

---

## Minimum Checkout Session Items

**Description**: A checkout session must contain at least one item to be valid and processable.

**Reasoning**: Prevents creation of empty or invalid checkout sessions that cannot result in meaningful transactions, reducing system overhead and preventing edge cases in payment processing.

---

## Active Checkout Session Uniqueness

**Description**: A customer can have at most one active checkout session at any given time.

**Reasoning**: Simplifies the checkout flow, prevents confusion about which session is being processed, and ensures a clear transaction state for both the customer and the system.

---

## Checkout Session-Cart Synchronization

**Description**: A checkout session can only be created from the current state of the customer's cart, and must accurately reflect all items and quantities present in the cart at the time of creation.

**Reasoning**: Ensures consistency between what the customer has selected (cart) and what they are attempting to purchase (checkout session), preventing discrepancies in order processing.
