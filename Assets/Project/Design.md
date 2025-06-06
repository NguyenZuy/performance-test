# Zombie War Game Design Document

## Player Stats

| Stat         | Base Value | Notes |
|--------------|------------|-------|
| Health       | 100 HP     | Visual health bar with damage flash effect |
| Move Speed   | 5 m/s      | Adjustable with power-ups |
| Damage       | Weapon-dependent | See weapon table below |
| Attack Speed | Weapon-dependent | See weapon table below |

## Zombie Types by Level

### Level 1 Zombies
| Type    | Health | Damage | Speed | Attack Speed | Special Traits |
|---------|--------|--------|-------|--------------|----------------|
| Normal  | 50 HP  | 10     | 2 m/s | 1 attack/sec | Basic zombie |
| Fast    | 30 HP  | 5      | 4 m/s | 2 attacks/sec | Moves quickly |
| Tank    | 100 HP | 15     | 1 m/s | 0.5 attacks/sec | High health |

### Level 2 Zombies
| Type    | Health | Damage | Speed | Attack Speed | Special Traits |
|---------|--------|--------|-------|--------------|----------------|
| Normal  | 70 HP  | 15     | 2.5 m/s | 1 attack/sec | Stronger than L1 |
| Fast    | 40 HP  | 8      | 5 m/s | 2 attacks/sec | Very fast |
| Tank    | 150 HP | 20     | 1.2 m/s | 0.5 attacks/sec | Armored (takes 50% bullet damage) |

### Level 3 Zombies
| Type    | Health | Damage | Speed | Attack Speed | Special Traits |
|---------|--------|--------|-------|--------------|----------------|
| Normal  | 100 HP | 20     | 3 m/s | 1 attack/sec | Toxic aura (damage over time) |
| Fast    | 50 HP  | 10     | 6 m/s | 2 attacks/sec | Leaves trail of acid |
| Tank    | 200 HP | 25     | 1.5 m/s | 0.5 attacks/sec | Explodes on death |

## Weapons

| Weapon       | Damage | Fire Rate | Ammo | Reload Time | Special Traits |
|--------------|--------|-----------|------|-------------|----------------|
| Pistol       | 15     | 2 shots/sec | 12  | 1.5 sec     | Default weapon |
| Shotgun      | 30 (x5 pellets) | 1 shot/sec | 6   | 2.5 sec     | Spread damage |
| Assault Rifle| 20     | 6 shots/sec | 30  | 2 sec       | Medium range |
| Sniper       | 60     | 0.75 shots/sec | 5  | 3 sec       | Pierces through zombies |