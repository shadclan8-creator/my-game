# Physics System

## Status
In Design

## Author
Claude (AI Agents: systems-designer + performance-analyst)

## Last Updated
2026-04-09

## Overview

Handles all physics interactions including movement collision detection, projectile physics, environmental collision, and cover/cover interactions. Foundation system that enables all gameplay physics.

## Player Fantasy

Authentic physical feedback. Every surface interaction feels grounded and responsive. Jumping, dashing, wall-running, and environmental contact all convey appropriate weight and impact.

## Detailed Rules

### Surface Detection

- **Wall-Running Detection**: Raycast in movement direction when player initiates wall-run. Must detect surfaces with LayerMask `Traversable`.
- **Climbing Detection**: Raycast downward when player approaches vertical surface. Surface must have `Climbable` component or LayerMask.
- **Ground Detection**: Cast sphere/box downwards when player is near ground to detect landing. Use minimum ground distance threshold.

### Projectile Physics

- **Instantiation**: Use object pooling for projectiles. Never instantiate during gameplay.
- **Movement**: Projectiles use physics-based movement (Velocity x deltaTime). No per-frame pathfinding.
- **Collision**: SphereCollider or BoxCollider with `Projectile` tag. Only collide with `Enemy`, `Environment`, and `Target` layers.

### Environmental Collision

- **Dynamic Cover**: Some surfaces provide cover. Physics queries use `Physics.OverlapSphere` to detect when player is in cover state.
- **Destructible Cover**: Explosions or specific weapons can destroy cover objects.

### Parkour-Specific Physics

- **Surface Attachment**: When wall-running, player attaches to surface via fixed joint or configurable spring. Attachment persists until player jumps or reaches edge.
- **Dynamic Friction**: Reduce friction on traversable surfaces when player is in movement state to enable sliding and wall-running control.
- **Gravity Modification**: Apply reduced gravity when sliding or climbing to improve control feel.

### Physics Events

- **Collision Events**: `OnCollisionEnter`, `OnCollisionStay`, `OnCollisionExit` fired with collision data.
- **Trigger Events**: `OnTriggerEnter` for zone boundaries and special interactions.

## Formulas

```
WallRunDistance = RaycastDirection * RayDistance
GroundCheckDistance = SphereCast(GroundLayer, downward) * GroundDetectionRadius
ProjectileVelocity = InitialVelocity * ProjectileSpeed * TimeAlive
GravityModified = BaseGravity * SlideModifier * ClimbModifier
```

**Variables**:
| Variable | Symbol | Type | Range | Description |
|----------|--------|------|------------|
| RaycastDirection | Vector3 | -1 to 1 (normalized) | Direction of wall-run raycast |
| WallRunDistance | float | 0.1 to 5.0 meters | Maximum distance for wall-run attachment |
| GroundDetectionRadius | float | 0.5 to 1.5 meters | Sphere radius for ground detection |
| ProjectileSpeed | float | 500 to 2000 | Units/second for projectile speed |
| SlideModifier | float | 0.6 to 0.8 | Friction multiplier during slides |
| ClimbModifier | float | 0.7 to 0.9 | Gravity multiplier during climbs |
| BaseGravity | float | -30.0 | m/s² | Standard Unity gravity |
| TimeAlive | float | 0 to 10.0 seconds | How long projectile exists before despawn |

**Output Ranges**:
| Formula | Output | Range | Notes |
|----------|-------|------|--------|
| WallRunDistance | float | 0.1 to 5.0m | Detects surface at 5m max |
| ProjectileVelocity | float | -1000 to -2000 | Speed vectors for projectiles |
| GravityModified | float | -27.0 to -21.0 | Modified gravity during slides/climbs |

## Edge Cases

- **Edge Surfaces**: Player at world edge. Wall-running raycast may go out of bounds. Clamp raycast to maximum safe distance (20m). Log warning.
- **Zero Wall-Run Vector**: Player jumping directly upward. Forward vector is (0,1,0). Ignore wall-run input to prevent incorrect attachment.
- **Climbing on Moving Objects**: Player climbing on elevator or platform that is moving. Disallow climbing. Provide error message via UI.
- **Projectile Pool Exhaustion**: All pooled projectiles in use and new instantiation requested. Log warning. Log error message or reuse failed projectile (visual indicator).
- **Multiple Surface Contact**: Player touching both wall and ground simultaneously. Prioritize ground contact (landing) for movement state transitions.
- **Cover During Dash**: Player dashing into cover. Dash ignores cover collision (treats dash as high-velocity movement through cover).
- **Projectiles Through Thin Walls**: Raycast may miss thin surfaces. Use continuous collision detection (SphereCollider with continuous checks) rather than single frame raycast.

## Dependencies

- **Input System** (ADR-0001): Provides player input for parkour actions
- **Movement System** (ADR-0003): Consumes collision detection results
- **Combat System** (ADR-0004): Consumes projectile hit detection
- **Environment Art Direction**: Provides traversable surface materials (LayerMasks)

## Tuning Knobs

| Tuning Knob | Range | Effect | Notes |
|-------------|-------|--------|
| WallRunAttachSpeed | 1.0 to 3.0 | How fast attachment locks to surface |
| WallRunDetachSpeed | 0.5 to 2.0 | How quickly player detaches from wall |
| GroundDetectionThreshold | 0.5 to 1.5m | Minimum distance to consider ground |
| SlideFrictionReduction | 0.4 to 0.8 | How much friction is reduced during slides |
| ClimbGravityReduction | 0.7 to 0.9 | How much gravity is reduced during climbs |
| ProjectilePoolSize | 10 to 50 | Initial pool size for projectiles |

## Acceptance Criteria

1. **Wall-Running Detection**: Player can wall-run on valid surfaces detected within 0.5m at raycast start distance.
2. **Climbing Detection**: Player can climb vertical surfaces within 1.0m downward raycast.
3. **Ground Detection**: Player lands correctly on surfaces with `Ground` layer within 0.15m of surface point.
4. **Projectile Pooling**: No projectiles are instantiated during gameplay. Pool is reused correctly.
5. **Cover Interactions**: Cover physics queries work correctly. Dynamic cover provides expected protection.
6. **Slide/Climb Modifiers**: Friction and gravity modifications feel responsive during parkour actions.
7. **Edge Surface Handling**: Edge of world surfaces handled safely. Maximum raycast distance enforced.
8. **Performance**: Physics calculations stay under 0.5ms frame budget during parkour with 20+ active projectiles.

## Open Questions

- Should climbing require player to face surface (for better UX) or is vertical approach sufficient?
- Should wall-running have maximum distance limit (for world edge safety) or rely on raycast out-of-bounds detection?
- Should projectiles use continuous collision (SphereCollider with fixed timestep) or sphere casts per frame?

