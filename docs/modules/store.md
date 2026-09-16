# Church store module specification

Status: Draft. Owners: Store administration and finance. Scheduled for a later phase; this defines boundaries, not permission to build all commerce features immediately.

## Model

`ProductCategory`, `Product`, `ProductVariant` (SKU/size/colour), `PriceListEntry` (currency and effective dates), `ProductImage`, `StockLocation`, immutable `StockMovement`, `Cart`, `Order`, `OrderLine`, `PaymentAttempt`, `Fulfilment`, `ReturnRequest` and `Refund`. Begin with branch stock; add warehouses only when there is a real distribution operation.

## Lifecycle

Catalogue publication is separate from draft editing. Checkout snapshots SKU, description, price, tax treatment and currency onto order lines. Reserve stock with expiry; confirmed payment converts reservation to sale movement, failed/expired payment releases it. Fulfilment records collection or delivery. Cancellation/return/refund are transitions, not deletion. Inventory adjustments require reason and scoped `Store.ManageInventory`; larger adjustments may use the shared approval service if policy requires.

## Constraints and acceptance

No negative stock unless a documented back-order policy is enabled. Stock balance derives from movements or a verified projection. Orders and receipts remain readable after a product rename or price change. Church-store proceeds are not displayed as giving. Test simultaneous last-item checkout, duplicate payment webhook, reservation expiry, branch stock scope, return and refund audit.
