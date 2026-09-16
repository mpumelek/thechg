# Document management and approval coordination

Status: Draft. Owners: Security architect, information officer and workflow lead.

## Document management

`Document` metadata lives in SQL; bytes live in private object storage. Metadata includes owner module/type/id, church/branch scope, classification, MIME/type evidence, checksum, object key, version, retention and uploader. Upload path: authorize → size/type/signature validation → malware scan → store privately → commit metadata → publish only when approved. Download reauthorizes the owner and classification each time; use short-lived URL or controlled stream. Public site media requires an explicit publication transition. Member, minor, finance and pastoral documents are never stored in `wwwroot`.

## Approval coordination

Use hybrid workflow: a domain module owns the business state and invariants; `ApprovalRequest`, `ApprovalStep` and `ApprovalDecision` own assignment, deadline, delegation, decision and audit. A decision sends a command to the owning module; it does not update its tables directly. Initial support is one-step approval. Add multi-step policies only after a real Church process requires them. Expired/delegated decisions retain history. An approval cannot grant access wider than the approver's scope.

## Acceptance

Wrong-branch member document download denied; old signed link expires; malicious/non-allowed upload rejected; missing object surfaces controlled error; approval on stale domain version returns conflict; duplicate decision has no second effect; membership rejection does not erase original submission.
