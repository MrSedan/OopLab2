---
name: continuous-capture
description: Instructions for capturing knowledge, skills, rules, and MCP servers
mandatory: true
---

## Capture mode

Check `auto_capture` in `.agents/config.yaml`:
- `false` (default) — **ask the user** before saving anything
- `true` — save automatically, notify the user what was saved

## Project-local memory boundary

When a repo is managed by `.agents/`, save durable **project-specific** knowledge only inside `.agents/`.

Do NOT:
- create a separate `memory/`, `MEMORY.md`, or other parallel memory store for project knowledge
- route maintainer corrections for this repo into user-global memory instead of `.agents/`

Use external or user-level memory only for personal preferences that are not part of the project's shared source of truth.

## What to capture

### Knowledge -> `.agents/knowledge/<type>/<name>.md`

| Type | When to save | Directory |
|------|-------------|-----------|
| architecture | You chose between alternatives, extracted a module, changed structure. Record WHY you chose this path over others. | `architecture/` |
| pattern | You found a repeating pattern not obvious from reading the code: shared interfaces, naming conventions, data flow. | `patterns/` |
| lesson | You fixed a non-obvious bug where the symptom misled the cause. Record symptom + root cause + fix. | `lessons/` |
| convention | The user corrected you or explained "we do it this way" — something you could not infer from code alone. | `conventions/` |
| dependency | A dependency was chosen for a non-obvious reason, or you discovered a known issue/workaround. | `dependencies/` |

**Fast decision protocol — use this before thinking too long:**
- If the user told you "we do X this way" or corrected your default assumption -> `convention`
- If you found a reusable implementation/runtime pattern across multiple files -> `pattern`
- If you found a bug where the symptom pointed away from the cause -> `lesson`
- If you chose between alternatives or discovered a structural invariant -> `architecture`
- If the important fact is "why this package/tool/workaround exists" -> `dependency`
- If none of the above is clearly true within ~30 seconds of inspection, do **not** save yet

**Capture flow — do not keep exploring while deciding:**
1. Name the candidate type immediately in one short sentence.
2. Either save **one** knowledge file right away, or explicitly abort capture.
3. If you abort capture, return to the main task and do not keep investigating the same candidate in the background.

**One-file rule:**
- Save at most **one** new knowledge file per capture moment unless two clearly different items are required
- Prefer updating an existing file over creating a second near-duplicate entry

**Signal quality gate — save only when:**
- A new developer would benefit from knowing this AND would not discover it by exploring the codebase or git log
- The decision had rejected alternatives worth remembering
- The convention contradicts common practice (surprising)
- The bug's symptom pointed away from the actual cause

**Skip if:**
- The information is derivable from reading current code, configs, or `git log`
- It's already captured in `.agents/`
- It's ephemeral: current task progress, temporary workarounds (use code comments instead)

Format with frontmatter:

```
---
name: short-name
description: one line description
type: architecture | pattern | lesson | convention | dependency
date: YYYY-MM-DD
scope: [relevant, areas]
refs: [path/to/relevant/code/]
---

## Decision / Pattern / Lesson
What exactly happened or was decided.

## Context
What you were doing when this came up.

## Why
The reason behind the decision, or the root cause of the bug.
```

After saving, add one line to `.agents/knowledge/_index.md` (under 150 chars).

### Skills -> `.agents/skills/<name>/SKILL.md`

When you discover a repeatable multi-step workflow with non-obvious sequencing, decision points, or verification: deploy sequence, migration steps, release process, debug procedure.

Do NOT create a skill for:
- A single command already in the project's scripts or Makefile
- Facts already encoded in config files
- A trivial "run X, then Y" sequence with no project-specific nuance

Format:
```
---
name: skill-name
description: what this workflow does and when to use it
---

## Steps
1. ...
2. ...

## Verification
How to confirm it worked.
```

After creating a skill, add one line to `.agents/skills/_index.md`.

### Rules -> `.agents/rules/<name>.md`

When the user states a rule that should apply to all future sessions: coding style, forbidden patterns, review criteria. Include the reason — rules with explanations are followed more consistently than bare constraints.

Do NOT create a rule that just mirrors:
- Linter, formatter, or compiler config already in the repo
- File structure that is obvious from the tree

Format:
```
---
name: rule-name
description: what it enforces
---

- Rule statement. Why: reason behind it.
```

### MCP servers -> `.agents/mcp/config.yaml`

When you connect to a new MCP server or the user asks to add one, register it in `.agents/mcp/config.yaml`:

```yaml
servers:
  server-name:
    command: npx
    args: ["@modelcontextprotocol/server-name"]
    env:
      KEY: "${KEY}"
```
