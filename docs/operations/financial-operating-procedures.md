# Financial operating procedures

Status: Draft. Owner: Finance leadership. Country receipt, tax, settlement, refund and retention rules require local finance/legal approval. This is an operating design, not approved accounting policy.

## Manual giving

Assigned finance user selects own branch, date/period, fund, currency and payment method; captures entries into a batch. Check duplicate source references, member identity where known, totals and source evidence. For offline capture, pending entries are not considered server-posted. On sync, resolve rejects before posting. Posting fixes the record; a receipt is generated according to approved country policy. Separate cash-handling and deposit practices remain under Church finance control.

## Online giving

Member initiates gateway checkout. Finance sees `Pending`, `Confirmed`, `Failed`, `Refunded`, `Disputed` and `Unmatched` states. Only verified provider event/settlement information supports posting. Reconcile gateway settlement to bank/payment report, attempt and contribution; record fees separately if needed. Anonymous giving is allowed only under approved rules.

## Corrections and close

Never edit/delete a posted contribution. Create a linked reversal or adjustment with reason, actor and effective date. Refund requires defined authority and provider confirmation. Close periods after reconciliation; reopening requires controlled permission and audit. Multi-currency totals must show original currencies or an explicitly sourced exchange rate and date.

## Exceptions and evidence

Daily/weekly cadence TBD. Investigate duplicate webhook, amount mismatch, wrong branch/fund, failed receipt, chargeback, cash discrepancy and out-of-period entry through an exception queue. Keep supporting documents private; restrict exports. Review financial access and audit logs periodically. See [giving module](../modules/giving-and-payments.md).
