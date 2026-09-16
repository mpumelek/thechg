# Security and service incident response

Status: Draft. Owners: Security lead and operations lead; information officer owns privacy escalation. Country-specific legal notifications require counsel review. Record actual contacts and response times before launch.

## Trigger and classification

Triggers: suspected unauthorized member/pastoral/child access, leaked credential, payment tampering, malicious upload, ransomware, lost offline device, provider compromise or widespread outage. Preserve correlation IDs, access logs, affected records and timeline. Do not include confidential content in broad chat/tickets. P1 invokes incident commander and information officer immediately.

## Procedure

1. Detect and open controlled incident record with time, reporter, environment and impact.
2. Contain: revoke account/device/token, rotate secret, disable affected integration or route, isolate service as needed.
3. Preserve evidence before destructive changes; record chain of custody and access.
4. Assess affected people, countries, data types, payments, duration and ongoing risk.
5. Eradicate and recover using approved changes and verified backups; validate authorization and reconciliation.
6. Communications lead and legal/information officer decide notices to members, providers, regulators and leadership according to applicable law.
7. Monitor recurrence; hold post-incident review; update controls, tests, documentation and training.

Never promise a legal notification deadline until country counsel confirms the applicable rule. Do not hide a security event as a routine support ticket. Test tabletop scenarios: leaked SQL credential, wrong-branch export, pastoral record access, and stolen offline device.
