# Rewind Acceptance Criteria

## Feature Overview
The rewind mechanic allows the player to reverse time for a limited duration (5 seconds) to undo mistakes or solve puzzles.

## Criteria
1. **Duration**: Exactly 5 seconds of gameplay state must be stored.
2. **Buffer**: Circular buffer implementation to minimize memory overhead.
3. **States tracked**:
    - Player position and rotation.
    - Player velocity.
    - Animation state.
    - Health and resource values.
    - Active projectile positions.
4. **Visuals**: Desaturation filter applied while rewinding.
5. **Input**: Rewind triggered by holding 'R' key.

## Performance
- No frame drops during buffer recording.
- Memory usage < 5MB for the buffer.
