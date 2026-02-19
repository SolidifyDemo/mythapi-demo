---
name: My Implementation Prompt
description: This prompt is used to implement the planned steps
agent: agent
model: Claude Sonnet 4.5 (copilot)
tools: [execute, read, agent, edit, search, todo, 'ms-docs/*']
---

Implement the planned steps and following:
- Add the changes in smalle steps as atomic commits
- Provide a git commit message to be copied MANUALLY for each commit, following the conventional commit format
- Add reasonable commit messages that explain the "why" behind the changes, not just the "what"
- Provide the actual commit command
- DO NOT COMMIT CHANGES AUTOMATICALLY, I WILL REVIEW THEM FIRST AND COMMIT MANUALLY