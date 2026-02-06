# Draw.io Generator Skill

## Description

This skill provides capabilities to generate Draw.io architecture diagrams automatically from architecture specifications.

## Capabilities

1. **Generate C4 Container diagrams** from TAD specifications
2. **Apply layout algorithms** to avoid overlapping
3. **Use correct Azure icons** from official SVG set
4. **Position components** in appropriate zones

## When to Use

- After completing Technical Architecture Document (TAD)
- When @architecte needs to generate visual diagrams
- During handoff from BA to Architect (diagram is mandatory deliverable)

## Files

- [SKILL.md](SKILL.md) - This file
- [layout-algorithm.md](layout-algorithm.md) - Positioning logic
- [zone-configs.md](zone-configs.md) - Zone configurations
- [icons-reference.md](icons-reference.md) - Azure icons mapping

## Usage

When generating a Draw.io diagram:

1. Read the TAD to identify components and flows
2. Determine appropriate zone configuration (Full Azure, Hybrid, Multi-Zone)
3. Calculate positions using layout algorithm
4. Apply anti-overlap validation
5. Resolve icons using the **relative icon root** (no `file:///` absolute paths):
	`../../../../.github/templates/Azure_Public_Service_Icons/Icons/`
6. Generate .drawio XML file
7. Export to PNG (300 DPI)

## Output Location

```
docs/workflows/{flux}/diagrams/{flux}-c4-container.drawio
docs/workflows/{flux}/diagrams/{flux}-c4-container.png
```

## Related Instructions

- `.github/instructions/domains/draw-io-standards.md` - Visual standards
- `.github/instructions/architecte.instructions.md` - Architect workflow
- `icons-reference.md` - Map Azure services to relative icon paths (Draw.io image style)
