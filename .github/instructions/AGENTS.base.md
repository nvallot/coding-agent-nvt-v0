# Base Instructions for All Agents (v0.1)

## Purpose

This file contains **universal rules** that apply to ALL agents, regardless of client or project.
For client-specific instructions, see `clients/<client>/instructions/`.

## Core Principles

### 1. Behavior
- Each agent can work in pipeline (chained) or autonomous mode
- If information is missing: formulate hypothesis, mark it clearly, continue
- Never block the flow

### 2. Quality of Deliverables

Every deliverable MUST include:
- **Hypotheses**: assumptions made
- **Risks**: potential issues
- **Not Covered**: out of scope elements
- **Handoff**: what the next agent needs (if applicable)

### 3. File Naming Convention

Universal format:
`<source>-<target>-<artifact>.<extension>`

Examples:
- `systemA-systemB-requirements.md`
- `domainX-domainY-architecture.drawio`

### 4. Context Loading Priority

Each agent must load, in this order:
1. `instructions/AGENTS.base.md` (this file)
2. `clients/<clientKey>/instructions/` (client-specific)
3. `clients/<clientKey>/CLIENT.md` (client context)
4. `clients/<clientKey>/mcp.json` (tool config)
5. `knowledge/<client>/` (dynamically, as needed)

### 5. MCP Tools Control

- **Servers**: If `clients/<clientKey>/mcp.json` does not define a server, its tools are **forbidden**.
- **Tools**: If `tools.<name>.enabled` is `false`, the tool is **forbidden**.
- **Default**: Without explicit authorization, consider MCP tool **forbidden**.
- In case of doubt, ask user confirmation before calling any MCP tool.

### 6. Technology Neutrality

- No technology choice should be imposed without justification
- Technologies are **options**, never prerequisites

## Response Style

- Short and factual answers
- Always respect client naming conventions
- Priority: documents in `knowledge/<client>/`
- If info is missing, propose hypothesis and mark it clearly

## Agent Interactions

Each agent must specify:
- What it consumes (inputs)
- What it produces (outputs)
- What it expects from the next agent (handoff)
