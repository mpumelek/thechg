# API contract conventions

Status: Draft. Owner: API lead. [OpenAPI draft](openapi.yaml) covers the first membership slice and must evolve with implementation. The current MVC application does not yet expose this contract.

## Surface

Prefix `/api/v1`. Resources use plural nouns; commands that are real domain transitions use action subresources such as `POST /members/{id}/submit` and `/approve`. JSON request/response DTOs never expose EF entities. Use ISO 8601 UTC timestamps, ISO country/currency codes, opaque IDs and decimal amounts serialized consistently. Browser UI may use cookies and antiforgery protection; future mobile clients require a reviewed token flow.

## Lists, errors and concurrency

Bound page size; use cursor pagination for large feeds and allowlisted filters/sorts. Return RFC-style `ProblemDetails` with stable `code`, `traceId`, field validation details and no sensitive internals. Use `401` unauthenticated, `403` insufficient permission, `404` where resource existence should not be disclosed, `409` invalid state/concurrency, `422` semantic validation where appropriate, `429` rate limit. Updates use ETag/row-version or explicit concurrency token.

## Authorization and replay

Each endpoint maps to a permission and resource scope; a member `/me` endpoint derives the target from the authenticated account. `Idempotency-Key` is required for payment/order writes. Offline sync has `(DeviceId, ClientOperationId)` uniqueness. Webhooks validate provider signature, timestamp/replay window and external event uniqueness. Requests and logs must not contain secrets or card data. OpenAPI is a design/verification contract, not an implementation claim; follow the [OpenAPI specification](https://spec.openapis.org/oas/).

## Change policy

Additive changes can remain in `v1`; breaking field/meaning changes require a new version or compatibility period. Every endpoint needs positive and negative contract, authorization and validation tests. Generate published OpenAPI from running code and compare it with the reviewed contract in CI.
