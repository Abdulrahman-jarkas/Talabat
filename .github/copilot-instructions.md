# Copilot Instructions

## Project Guidelines
- User plans to implement Option A for payment success flow: split OnPaymentSuccessEventHandler into independent handlers (Create Order, Complete Session, Deduct Stock each react to PaymentSuccessedEvent independently). Complete Session won't need OrderId — just PaymentId. Will implement later, not now.
- User chose modular monolith architecture to balance simplicity and DDD/distribution benefits. Concerned about rising complexity — wants to find the right balance point where they get the benefits without over-engineering.