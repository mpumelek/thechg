# Privacy assessment and data inventory

Status: Draft, **not legal advice or a completed impact assessment**. Owners: Information officer and qualified country counsel. No legal basis or retention period below is approved. Review the [official POPIA text](https://www.justice.gov.za/legislation/acts/2013-004.pdf) and each other country's applicable law before production.

| Category | Examples | Purpose / access | Proposed handling; decision needed |
|---|---|---|---|
| Identity/contact | Name, phone, email, address | Membership operations; scoped staff and self | Verify accuracy; retention/legal basis TBD |
| Religious membership | Branch, status, ministry, attendance | Church administration and pastoral operation | Special sensitivity; purpose and access review |
| Children/dependants | Guardian, birth date, Sunday School, emergency details | Safeguarding and participation | Competent-person/guardian basis; minimum fields; photo consent separate |
| Pastoral/health | Cases, counselling, medical/emergency notes | Restricted care | Case grants, separate retention and encryption/key policy |
| Giving/payment | Contribution history, provider refs | Receipts and reconciliation | Finance/self-only; no card details stored; retention/accounting review |
| Store/event | Orders, delivery, registrations | Fulfilment and attendance | Separate commercial purposes/retention |
| Communications | Preferences, consents, delivery logs | Transactional/optional messages | Purpose/channel-specific basis and opt-out |
| Device/security | IP, session, audit, offline device ID | Security/fraud/support | Limited retention, strict access |
| Documents/images | Member forms, certificates, photos | Evidence and publication where approved | Private by default, checksum, scan, retention |

## Required assessment work

Document responsible party/operator roles; purpose and lawful basis per category/country; collection notice; recipients and processors; hosting/backup location; cross-border transfer assessment; retention/deletion schedule; rights request process; breach procedure; guardian/child safeguards; provider contracts; and any required prior authorization or registration. Complete a data-flow map for each payment, messaging and storage provider. Treat religious affiliation, child and pastoral information as high-risk. Do not use engagement scores for automated adverse membership decisions.

## Design rules

Private by default; collect only required data; purpose-limit secondary use; do not infer household data access; keep sensitive values out of logs; encrypt transport and storage; audit sensitive reads/exports; retain evidence of consent/policy changes. A request to erase data may conflict with legal/accounting retention, so handle through a reviewed case process rather than unconditional deletion.
