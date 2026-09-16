# New Member Journey module specification

Status: Draft. Owners: New Member Coordinator and Church leadership. Engineering owns the configurable engine; Church leadership owns curriculum content and graduation policy.

## Boundary and model

`Curriculum` has immutable published `CurriculumVersion`s. A version contains modules, lessons, requirements and milestones. `MemberJourney` enrols an approved member against one version. `JourneyProgress` records evidence, completion actor/time and exceptions. Attendance is captured by the Attendance module and referenced as evidence; documents remain in Document Management.

## Workflow

`NotStarted → Enrolled → InProgress → ReadyForReview → Completed`; `Paused` and `Withdrawn` are explicit alternatives. Enrolment follows membership approval. Required steps may include orientation, classes, assessments, baptism, confirmation, ministry introduction, iMihlangano allocation and buddy check-ins, all configured by version. A completion evaluator checks required rules and produces an explainable list of unmet requirements. Authorized override records reason and approving actor. Completion emits `NewMemberJourneyCompleted` and requests Membership to activate only if its remaining invariants pass.

## Constraints

Published versions are not edited in place. New content creates a new version. Existing enrolments stay on their version unless an approved migration maps old/new steps and evidence. Different countries/branches may select permitted versions without embedding theology in code. One active journey of a type per member unless an explicit restart is recorded.

## Acceptance

Leadership can configure and publish a curriculum without a code release; version edits do not change existing member requirements; progress shows evidence and missing steps; authorized exception is audited; journey completion cannot bypass membership approval. See [buddy/group specification](buddy-and-cell-groups.md).
