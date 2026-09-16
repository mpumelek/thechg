# Communications module specification

Status: Draft. Owners: Communications lead and information officer. Provider and country rules remain open.

## Model and boundary

Own `MessageTemplate`, `CommunicationPreference`, `ConsentRecord`, `Campaign`, `AudienceDefinition`, `OutboundMessage`, `DeliveryAttempt` and suppression lists. Source modules publish intents such as `ReceiptReady` or `BuddyCheckInDue`; they do not call vendor APIs. Provider adapters implement email, SMS and WhatsApp contracts. In-app/push can be added later.

## Workflow

Authorize sender and intended scope → validate purpose/channel/consent → resolve audience at send time → snapshot recipient IDs and template version → enqueue via outbox → worker sends with idempotency key → store provider ID/status → retry transient failure → dead-letter permanent/exhausted failure. Scheduled messages recheck audience eligibility before dispatch. Bulk messages have preview, recipient count, approval where policy requires and cancellation window.

## Privacy and acceptance

Do not expose recipient lists to unauthorized staff. Do not include pastoral content, sensitive child data, full giving details or credentials in SMS/WhatsApp. Support unsubscribe/suppression by channel and purpose, while distinguishing necessary service messages from optional broadcasts under legal review. Test opt-out, out-of-scope targeting, provider downtime, duplicate job, template versioning, and delivery history.
