# Payment integration contract

Status: Draft. Owners: Finance lead and integration lead. Provider selection is country-specific and pending.

## Adapter contract

`IPaymentGateway`: create checkout, verify webhook, query transaction, initiate refund (if supported), map provider status. Store provider credentials in a secret manager, never source control. Persist `PaymentAttempt` with church, country, branch, purpose (`Giving`, `Event`, `Store`), amount, currency, return reference and status before redirecting.

## Webhook processing

Validate provider signature and timestamp/replay window; persist a uniquely keyed provider event; acknowledge promptly; process asynchronously and idempotently. Confirm event belongs to expected attempt, amount, currency and purpose. Never trust a browser return URL. Duplicate/out-of-order callbacks must not create duplicate contributions/orders. Reconcile settled provider transactions against attempts and business records; surface unmatched items to finance.

## Failure scenarios

Timeout after checkout creation, user abandoning checkout, delayed webhook, provider retry, refund rejected, chargeback, amount mismatch, branch reassignment and provider downtime each need an explicit state and operational queue. Do not log full payment tokens or card data. Refund authority is set by country finance policy, not hard-coded globally. See [giving module](../../modules/giving-and-payments.md).
