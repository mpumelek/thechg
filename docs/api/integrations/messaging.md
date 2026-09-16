# Messaging integration contract

Status: Draft. Owners: Communications lead and integration lead. Email, SMS and WhatsApp providers and country sender requirements remain unselected.

## Ports and data

Adapters implement send, check status where available, parse delivery webhook, and normalize errors. `OutboundMessage` records purpose, channel, template/version, recipient reference, country, scheduled time, consent/policy decision and idempotency key. `DeliveryAttempt` records provider ID, status, retry and error category. Avoid placing raw sensitive payloads in logs or dead-letter messages.

## Dispatch and consent

The business transaction writes an outbox intent. Worker resolves current contact method, suppression and permitted purpose, renders an approved localized template, sends, and records outcome. Distinguish essential service communication from optional broadcast under legal review. WhatsApp templates/sender approval may differ by country. Retries use exponential backoff for transient errors; invalid address/opt-out is terminal until corrected.

## Acceptance

Provider timeout cannot duplicate a message when provider idempotency is available; failed attempts are visible; revoked consent stops optional sends; a branch broadcaster cannot target another branch; delivery webhook cannot modify unrelated recipient state. See [communications module](../../modules/communications.md).
