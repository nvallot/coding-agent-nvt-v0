# Layout Algorithm for Draw.io Generation

## Overview

This document describes the algorithm for positioning components in Draw.io diagrams to avoid overlapping.

## Grid System

```
Grid Size: 20px
Snap: Always enabled
```

## Component Dimensions

| Component Type | Width | Height | Label Height |
|----------------|-------|--------|--------------|
| Azure Icon | 60px | 60px | 30px (2 lines) |
| Azure Icon (large) | 80px | 80px | 40px (3 lines) |
| Zone Rectangle | Variable | Variable | 40px header |
| Number Circle | 24px | 24px | N/A |

## Spacing Constants

```python
HORIZONTAL_GAP = 40  # Minimum horizontal space between components
VERTICAL_GAP = 30    # Minimum vertical space between components
ZONE_MARGIN = 20     # Internal padding of zones
ZONE_HEADER = 40     # Height reserved for zone title
INTER_ZONE_GAP = 80  # Space between zones
LABEL_SPACING = 10   # Space between icon and label
```

## Position Calculation

### For components within a zone (Grid Layout)

```python
def calculate_position(zone, component_index, total_components):
    """
    Calculate X,Y position for a component within a zone.
    Uses a 2-column layout for better visual balance.
    """
    columns = 2 if total_components <= 6 else 3
    
    col = component_index % columns
    row = component_index // columns
    
    cell_width = ICON_WIDTH + HORIZONTAL_GAP
    cell_height = ICON_HEIGHT + LABEL_HEIGHT + VERTICAL_GAP
    
    x = zone.x + ZONE_MARGIN + (col * cell_width)
    y = zone.y + ZONE_HEADER + ZONE_MARGIN + (row * cell_height)
    
    return (x, y)
```

### For zones (Left to Right Flow)

```python
def calculate_zone_positions(zones):
    """
    Position zones from left to right based on flow direction.
    """
    positions = {}
    current_x = 0
    
    # Order: On-Premise -> Azure -> External -> Monitoring
    zone_order = ['on-premise', 'azure', 'external', 'monitoring']
    
    for zone_type in zone_order:
        if zone_type in zones:
            zone = zones[zone_type]
            positions[zone_type] = {
                'x': current_x,
                'y': 0,
                'width': calculate_zone_width(zone),
                'height': calculate_zone_height(zone)
            }
            current_x += positions[zone_type]['width'] + INTER_ZONE_GAP
    
    return positions
```

## Anti-Overlap Validation

```python
def validate_no_overlap(components):
    """
    Check that no two components overlap.
    Returns list of conflicts if any.
    """
    conflicts = []
    
    for i, comp1 in enumerate(components):
        for comp2 in components[i+1:]:
            if boxes_overlap(comp1.bounds, comp2.bounds):
                conflicts.append((comp1, comp2))
    
    return conflicts

def boxes_overlap(box1, box2):
    """
    Check if two bounding boxes overlap.
    Box format: (x, y, width, height)
    """
    x1, y1, w1, h1 = box1
    x2, y2, w2, h2 = box2
    
    # Add padding for labels
    h1 += LABEL_HEIGHT
    h2 += LABEL_HEIGHT
    
    return not (
        x1 + w1 + HORIZONTAL_GAP <= x2 or  # box1 is left of box2
        x2 + w2 + HORIZONTAL_GAP <= x1 or  # box2 is left of box1
        y1 + h1 + VERTICAL_GAP <= y2 or    # box1 is above box2
        y2 + h2 + VERTICAL_GAP <= y1       # box2 is above box1
    )
```

## Auto-Fix Overlaps

```python
def fix_overlaps(components):
    """
    Automatically adjust positions to fix overlaps.
    Uses horizontal displacement first, then vertical.
    """
    conflicts = validate_no_overlap(components)
    
    while conflicts:
        comp1, comp2 = conflicts[0]
        
        # Try horizontal displacement first
        if can_move_right(comp2):
            comp2.x += ICON_WIDTH + HORIZONTAL_GAP
        elif can_move_right(comp1):
            comp1.x += ICON_WIDTH + HORIZONTAL_GAP
        else:
            # Fall back to vertical displacement
            comp2.y += ICON_HEIGHT + LABEL_HEIGHT + VERTICAL_GAP
        
        conflicts = validate_no_overlap(components)
    
    return components
```

## Example Layout

For 6 components in a 2-column grid:

```
Zone (width=300, height=350)
├── Header: "Azure Cloud - ISP" (y=0, height=40)
├── Row 0:
│   ├── Col 0: Component 1 @ (20, 60)
│   └── Col 1: Component 2 @ (140, 60)
├── Row 1:
│   ├── Col 0: Component 3 @ (20, 180)
│   └── Col 1: Component 4 @ (140, 180)
└── Row 2:
    ├── Col 0: Component 5 @ (20, 300)
    └── Col 1: Component 6 @ (140, 300)
```

## Flow Numbers Positioning

```python
def position_flow_number(source, target, number):
    """
    Position a flow number (❶❷❸) along the arrow between two components.
    """
    # Calculate midpoint of arrow
    mid_x = (source.x + source.width/2 + target.x + target.width/2) / 2
    mid_y = (source.y + source.height/2 + target.y + target.height/2) / 2
    
    # Offset slightly above the line
    return (mid_x - 12, mid_y - 24)  # 12 = half circle width, 24 = offset above
```
