# Project Intelligence

This project stores all agent instructions in `.agents/`. Read these files on session start.

## Required (read first, in parallel)

- `.agents/config.yaml` — project name, description, stack
- `.agents/rules/*.md` — all rule files are mandatory, apply them throughout the session

## Recommended

- `.agents/knowledge/_index.md` — team knowledge index; read on start, drill into linked files when relevant
- `.agents/skills/_index.md` — skill index (if exists); read full SKILL.md only when a matching task arises

## Loading policy

- For debugging, incidents, deployment, CI, data flow, and hidden-contract tasks: read relevant knowledge early.
- For local implementation tasks with clear code patterns: start from code, then read only the knowledge that reduces risk or resolves hidden context.
- If saved knowledge conflicts with the current task framing, surface the conflict explicitly instead of silently choosing one.
