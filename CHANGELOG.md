v0.0.1-init =======================================================================================
- Created project.
- Added black skybox for 2D game.
- Removed light source from environmental lighting.

v0.1.0-map-base ===================================================================================
- Added MapManager, GameManager, and PrefabManager classes and bound them to the GameManager object.
- Added logic for parsing a .txt map file and placing floors and walls.
- Added logic for reading in and cataloguing prefabs.

v0.2.0-base-player ================================================================================
- Added player object.
- Added to player:
    - Movement
    - Animation
    - Attack
- Create player 3D model.
- Added pillars.
- Added sprite sort order logic.

v0.3.0-ranged-enemy ===============================================================================
- Added enemy prefab.
- Added enemy 3D model and renders for movement, fire, and death.
- Added scripts for the following logic:
    - Animation
    - Attack
    - Attribute
    - Behavior
    - Health
    - Movement
- Added plasma projectile and ricochet.
- Added pathfinding logic.

v0.4.0-round-logic ================================================================================
- Added console manager.
- Added round phase logic.
- Added logic for each round phase.
- Added enemy population escalation logic.
- Added enemy spawn logic and animation.

v0.5.0-map-progression ============================================================================