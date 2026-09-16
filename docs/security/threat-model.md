# Threat model and security verification plan

Status: Draft. Owner: Security architect. Review with product, finance, pastoral, operations and branch representatives before Phase 1 release. Use [OWASP ASVS](https://owasp.org/projects/asvs) as a verification baseline, not as proof of project-specific security.

## Assets, boundaries and assumptions

Assets: member/minor/pastoral records, giving/store transactions, login sessions, scoped permissions, documents, webhook secrets, offline device queues and audit evidence. Trust boundaries: internet→web/API; browser/PWA storage→server; staff user→branch/country scope; web→SQL/object store/worker; payment/messaging provider→webhook; administrator→pastoral case. No provider or client payload is inherently trusted.

| Threat | Example abuse | Required controls and tests |
|---|---|---|
| Broken object authorization | Change member ID to read sibling branch or spouse's giving | Resource+scope policy on every read/write; IDOR matrix tests |
| Privilege escalation | Staff alters role/scope or self-approves | Server-owned scope, delegated-grant ceiling, transition checks, audit |
| Credential/session attack | Reused default password, stolen cookie | Remove seeded secrets, MFA, lockout, secure cookies, revocation |
| Upload attack | Executable/polyglot file placed in `wwwroot` | Private storage, allowlist/signature checks, scan, size cap, re-encode images |
| Payment spoof/replay | Browser success or duplicate webhook posts giving | Verify signature, persist external event ID, idempotent handler, reconciliation |
| Offline replay/tamper | Reused operation or changed branch on device | Device registration, scoped token, unique client UUID, server validation |
| Data leakage | Pastoral text in logs, exports or broad reports | Classification, redaction, export policy, case grants, access log |
| CSRF/XSS/SQL injection | Browser state change or untrusted HTML | Antiforgery, encoding/CSP, parameterized queries, security tests |
| Insider misuse | Bulk export or break-glass viewing | Least privilege, reason, sensitive access log, alert and review |
| Availability abuse | Login/search/export flooding | Edge and app rate limits, query limits, async exports, monitoring |

## Verification gates

Threat model updated when data flow, provider, scope or authentication changes. CI runs secret/dependency/static scans; tests prove denial paths. Prior to production, conduct independent security review and remediate critical/high findings. Record accepted residual risks with owner and expiry. Log an incident without copying sensitive evidence into ordinary issue trackers.
