# Combat

Health, stamina, and hits, shared by the player and the boss. Pure rules: no animation, no input.
See `Scenes/Combat_Sandbox.unity` for two capsules using everything below.

## Putting it on a body

On the body root:
- `Health` — hit points. Raises `FightEvents.OnHit` per landed hit and `FightEvents.OnDeath` once.
- `Stamina` — spent by attacks (and rolls, if the body wants), regenerates after a short delay.
- `Hurtbox` — on the body's physical collider (or any child collider). Routes hits to `Health`.
  The collider must be on the body's layer (`Player` or `Boss`).
- `AttackRunner` — plays an `AttackData` timeline. Point it at the hitbox below and the `Stamina`.

On a child object:
- `Hitbox` — a sphere of damage (offset + radius, no collider needed) on the `PlayerHitbox` or `BossHitbox` layer.
  One per weapon or fist; add more children if a move needs coverage. It runs its own overlap query each physics step while armed.

## Using it

```csharp
runner.TryStart(lightAttack);        // false if busy or out of stamina
runner.CanStart(heavyAttack);        // ask first if you need to
runner.Phase;                        // Idle, Windup, Active, Recovery
runner.Interrupt();                  // stagger, death, episode reset

health.GrantInvulnerability(0.4f);   // call at the start of a roll
health.IsDead; health.Normalized;    // for HUD and observations
stamina.TrySpend(cost);              // for rolls or anything else the body charges for
```

Attack timing and numbers live in `AttackData` assets (Create → BossFight → Attack Data). `Data/` has a Light and a Heavy to start from.

Events: `FightEvents.OnHit(attacker, victim, info)` and `FightEvents.OnDeath(victim)` in Core. Subscribe from anywhere; unsubscribe in `OnDisable`.

Not in v1: knockback, poise/stagger, blocking. `AttackRunner.Interrupt()` is the hook for stagger when someone needs it.
