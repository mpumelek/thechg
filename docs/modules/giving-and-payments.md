# Giving and payments module specification

Status: Draft. Owners: Finance leadership and technical lead. Country accounting, gateway and receipt rules require approval before online release. Related: [financial SOP](../operations/financial-operating-procedures.md), [payment integration](../api/integrations/payments.md).

## Boundary

Own `Fund`, `Campaign`, `ContributionBatch`, `Contribution`, `PaymentAttempt`, `PaymentProviderEvent`, `Receipt`, `Adjustment` and `ReconciliationItem`. Store and Event modules own commercial orders/tickets; the Payments adapter can serve them, but purchase revenue is not a donation. Each contribution captures branch/country/fund/currency at transaction time, unaffected by later member transfer.

## Manual flow

Scoped finance user opens a branch batch, captures amounts and payment methods, validates, then posts. No routine dual approval is required. Posting freezes contributions. Offline capture stores encrypted/minimized local operation with client UUID, pending indicator and recovery path. Server accepts each `(DeviceId, ClientOperationId)` once; rejection returns a reason and never silently drops the item.

## Online flow

Create payment attempt → provider checkout → signed webhook → persist provider event → verify and deduplicate → confirm settlement state → create/post contribution → queue receipt. Browser redirect only displays pending/confirmed state read from server. Refund creates separate provider and accounting records. Reconciliation compares provider settlements, attempts and contributions.

## Invariants and acceptance

No floating-point money, no cross-currency aggregation without explicit exchange-rate policy, no editing posted rows, no hard deletion. Period close policy TBD. Tests cover duplicate/out-of-order webhook, branch mismatch, replayed offline operation, partial payment, provider outage, correction/reversal, and member self-service limited to own giving.
