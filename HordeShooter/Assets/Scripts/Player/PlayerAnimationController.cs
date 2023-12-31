using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerAttackController playerAttackController;
    private PlayerMovementController playerMovementController;
    
    private enum PlayerAnimation {
        Idle,
        Roll,
        Run
    }
    private PlayerAnimation animation = PlayerAnimation.Idle;
    
    private enum Direction {
        Back,
        Front,
        FrontLeft,
        FrontRight,
        Left,
        RearLeft,
        RearRight,
        Right
    }
    private Direction lookDirection = Direction.Front;
    private Direction moveDirection = Direction.Front;

    private enum RunDirection {
        Backwards,
        Forwards
    }
    private RunDirection runDirection = RunDirection.Forwards;

    private struct DirectionAngle {
        public DirectionAngle(Direction newDirection, float newMin, float newMax, Vector2 newInputValues, List<Direction> newBackwards) {
            direction = newDirection;
            min = newMin;
            max = newMax;
            inputValues = newInputValues;
            backwardsDirections = newBackwards;
        }

        private Direction direction;
        public Direction GetDirection() { return direction; }
        
        private float min;
        public float GetMin() { return min; }
        
        private float max;
        public float GetMax() { return max; }

        private Vector2 inputValues;
        public Vector2 GetInputValues() { return inputValues; }
        
        private List<Direction> backwardsDirections;
        public bool IsDirectionBackwards(Direction direction) {
            return backwardsDirections.Contains(direction);
        }
    }
    private List<DirectionAngle> directionAngles;

    private Animator animator;
    private string currentlyPlayingAnimation = "";
    private GameObject reloadIndicator = null;
    private float reloadIndicatorProgress = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        playerAttackController = GetComponent<PlayerAttackController>();
        playerMovementController = GetComponent<PlayerMovementController>();
        
        directionAngles = new List<DirectionAngle>();
        directionAngles.Add(new DirectionAngle(Direction.Back, 337.5f, 22.5f, new Vector2(1, 0), new List<Direction> {
            Direction.FrontLeft,
            Direction.Front,
            Direction.FrontRight
        }));
        directionAngles.Add(new DirectionAngle(Direction.RearRight, 22.5f, 67.5f, new Vector2(1, 1), new List<Direction> {
            Direction.Left,
            Direction.FrontLeft,
            Direction.Front
        }));
        directionAngles.Add(new DirectionAngle(Direction.Right, 67.5f, 112.5f, new Vector2(0, 1), new List<Direction> {
            Direction.RearLeft,
            Direction.Left,
            Direction.FrontLeft
        }));
        directionAngles.Add(new DirectionAngle(Direction.FrontRight, 112.5f, 157.5f, new Vector2(-1, 1), new List<Direction> {
            Direction.Back,
            Direction.RearLeft,
            Direction.Left
        }));
        directionAngles.Add(new DirectionAngle(Direction.Front, 157.5f, 202.5f, new Vector2(-1, 0), new List<Direction> {
            Direction.RearLeft,
            Direction.Back,
            Direction.RearRight
        }));
        directionAngles.Add(new DirectionAngle(Direction.FrontLeft, 202.5f, 247.5f, new Vector2(-1, -1), new List<Direction> {
            Direction.Back,
            Direction.RearRight,
            Direction.Right
        }));
        directionAngles.Add(new DirectionAngle(Direction.Left, 247.5f, 292.5f, new Vector2(0, -1), new List<Direction> {
            Direction.RearRight,
            Direction.Right,
            Direction.FrontRight
        }));
        directionAngles.Add(new DirectionAngle(Direction.RearLeft, 292.5f, 337.5f, new Vector2(1, -1), new List<Direction> {
            Direction.Right,
            Direction.FrontRight,
            Direction.Front
        }));
        
        animator = transform.Find(Constants.gameObjectSprite).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessAnimation();
        ProcessLookDirection();
        ProcessMoveDirection();
        ProcessMoveState();
        ProcessRunDirection();

        HandleReloadIndicator();
    }

    // Create muzzle flare.
    public void CreateMuzzleFlare() {
        // Get flare location.
        Vector3 flareLocation = transform.Find(Constants.gameObjectMuzzleFlarePoints).Find(lookDirection.ToString()).position;

        // Get flare orientation.
        int flareRotation = GetMuzzleFlareRotation();

        // Create flare.
        GameObject flareObject = Instantiate(
            PrefabManager.instance.GetPrefab(Constants.gameObjectMuzzleFlare),
            flareLocation,
            Quaternion.Euler(0, flareRotation, 0)
        );

        // Slate flare for destruction.
        Destroy(flareObject, 1);
    }

    // Create reload indicator.
    public void CreateReloadIndicator() {
        // Create reload indicator object.
        reloadIndicator = Instantiate(
            PrefabManager.instance.GetPrefab(Constants.gameObjectReloadIndicator),
            GameObject.FindGameObjectWithTag(Constants.tagPlayer).transform.position,
            Quaternion.identity
        );
        reloadIndicator.transform.Find(Constants.gameObjectSprite).GetComponent<Animator>().Play(Constants.animationReload, 0, reloadIndicatorProgress);
    }

    // Create ricochet.
    public void CreateRicochet(RaycastHit hit) {
        // Create metal ricochet.
        if (hit.collider.gameObject.CompareTag(Constants.tagEnemy)) {
            CreateRicochetMetal(hit);
            return;
        }
        
        // Create wall ricochet.
        if (hit.collider.gameObject.CompareTag(Constants.tagWall) || hit.collider.gameObject.CompareTag(Constants.tagObject)) {
            CreateRicochetWall(hit);
            return;
        }
    }

    // Create spark for bot ricochet.
    private void CreateRicochetMetal(RaycastHit hit) {
        // Create ricochet prefab at the hit point and set a random rotation.
        GameObject ricochetObject = Instantiate(
            PrefabManager.instance.GetPrefab(Constants.gameObjectMetalRicochet),
            hit.point,
            Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0))
        );

        // Destroy the ricochet.
        Destroy(ricochetObject, 1);
    }

    // Create puff of stone for wall ricochet.
    private void CreateRicochetWall(RaycastHit hit) {
        // Get hit object.
        GameObject hitObject = hit.collider.gameObject;

        // Get hit point.
        Vector3 hitPoint = new Vector3(hit.point.x, 0, hit.point.z);

        // Get difference between hit object pos and hit point (x, z).
        Vector2 difference = new Vector2(
            Mathf.Abs(hitPoint.x - hitObject.transform.position.x),
            Mathf.Abs(hitPoint.z - hitObject.transform.position.z)
        );
        
        int rotation = 0;
        Vector3 ricochetLocation = Vector3.zero;
        // Calculate direction of ricochet for a wall.
        if (hit.collider.gameObject.CompareTag(Constants.tagWall)) {
            // Up
            if (hitPoint.z < hitObject.transform.position.z && difference.y > difference.x) {
                rotation = 180;
                ricochetLocation = new Vector3(hitPoint.x, 0, hitObject.transform.position.z);
            }
            // Right
            if (hitPoint.x > hitObject.transform.position.x && difference.x > difference.y) {
                rotation = 90;
                ricochetLocation = new Vector3(hitObject.transform.position.x, 0, hitPoint.z);
            }
            // Down
            if (hitPoint.z > hitObject.transform.position.z && difference.y > difference.x) {
                rotation = 0;
                ricochetLocation = new Vector3(hitPoint.x, 0, hitObject.transform.position.z);
            }
            // Left
            if (hitPoint.x < hitObject.transform.position.x && difference.x > difference.y) {
                rotation = 270;
                ricochetLocation = new Vector3(hitObject.transform.position.x, 0, hitPoint.z);
            }
        }

        // Calculate direction of ricochet for an object.
        if (hit.collider.gameObject.CompareTag(Constants.tagObject)) {
            // Up
            if (hitPoint.z < hitObject.transform.position.z && difference.y > difference.x) {
                rotation = 180;
                ricochetLocation = new Vector3(hitPoint.x, 0, hitPoint.z);
            }
            // Right
            if (hitPoint.x > hitObject.transform.position.x && difference.x > difference.y) {
                rotation = 90;
                ricochetLocation = new Vector3(hitPoint.x, 0, hitPoint.z);
            }
            // Down
            if (hitPoint.z > hitObject.transform.position.z && difference.y > difference.x) {
                rotation = 0;
                ricochetLocation = new Vector3(hitPoint.x, 0, hitPoint.z);
            }
            // Left
            if (hitPoint.x < hitObject.transform.position.x && difference.x > difference.y) {
                rotation = 270;
                ricochetLocation = new Vector3(hitPoint.x, 0, hitPoint.z);
            }
        }

        // Create ricochet prefab.
        GameObject ricochetObject = Instantiate(
            PrefabManager.instance.GetPrefab(Constants.gameObjectRicochetObject),
            ricochetLocation,
            Quaternion.Euler(0, rotation, 0)
        );

        // Destroy ricochet prefab.
        Destroy(ricochetObject, 1);
    }

    // Get animation name.
    private string GetAnimationName() {
        string animationName = "";

        // Get move direction prefix.
        animationName += runDirection.Equals(RunDirection.Forwards) ? "" : runDirection.ToString();

        // Get animation name.
        animationName += animation.ToString();

        // Get look direction.
        Direction animationDirection = lookDirection;
        if (playerMovementController.IsRolling()) {
            animationDirection = moveDirection;
        }
        animationName += Constants.splitCharUnderscore + animationDirection.ToString();

        return animationName;
    }

    // Get direction for player input.
    private Direction GetDirectionForPlayerInput(Vector2 playerInput) {
        Direction moveDirection = lookDirection;
        foreach (DirectionAngle directionAngle in directionAngles) {
            if (directionAngle.GetInputValues().Equals(playerInput)) {
                moveDirection = directionAngle.GetDirection();
            }
        }
        return moveDirection;
    }

    // Get rotation for the muzzle flare.
    private int GetMuzzleFlareRotation() {
        int rotation = 0;
        switch (lookDirection) {
            case Direction.Back:
                rotation = -90;
                break;
            case Direction.RearRight:
                rotation = -45;
                break;
            case Direction.Right:
                rotation = 0;
                break;
            case Direction.FrontRight:
                rotation = 45;
                break;
            case Direction.Front:
                rotation = 90;
                break;
            case Direction.FrontLeft:
                rotation = 135;
                break;
            case Direction.Left:
                rotation = 180;
                break;
            case Direction.RearLeft:
                rotation = 225;
                break;
            default:
                rotation = 0;
                break;
        }
        return rotation;
    }

    // Handle the reload indicator.
    private void HandleReloadIndicator() {
        // Not needed if not reloading.
        if (!playerAttackController.IsReloading()) {
            // Destroy indicator if it still exists.
            if (reloadIndicator != null) {
                Destroy(reloadIndicator);
                reloadIndicator = null;
                reloadIndicatorProgress = 0;
            }
            
            return;
        }

        // Create reload indicator if not paused and no indicator exists.
        if (!playerAttackController.IsReloadPaused() && reloadIndicator == null) {
            CreateReloadIndicator();
        }

        // Pause reload if necessary.
        if (playerAttackController.IsReloadPaused() && reloadIndicator != null) {
            reloadIndicatorProgress = reloadIndicator.transform.Find(Constants.gameObjectSprite).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime;
            Destroy(reloadIndicator);
            reloadIndicator = null;
        }
    }

    // Check if the animation playing is equal to the current animation to be played.
    private bool IsAnimationActionEqual() {
        string animationPrefix = "";
        animationPrefix += runDirection.Equals(RunDirection.Forwards) ? "" : runDirection.ToString();
        animationPrefix += animation.ToString();
        return currentlyPlayingAnimation.Split(Constants.splitCharUnderscore)[0].Equals(animationPrefix);
    }

    // Check if animation is playing.
    private bool IsAnimationPlaying(string animationName) {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    // Check if move direction is backwards from the direction the player is facing.
    private bool IsMoveDirectionBackwards() {
        bool backwards = false;
        
        // Get player input.
        Vector2 playerInput = new Vector2(InputManager.instance.GetVerticalInput(), InputManager.instance.GetHorizontalInput());

        // Check if direction is in the backwards list for the player's current lookDirection.
        foreach (DirectionAngle directionAngle in directionAngles) {
            if (directionAngle.GetDirection().Equals(lookDirection) && directionAngle.IsDirectionBackwards(moveDirection)) {
                backwards = true;
            }
        }

        return backwards;
    }

    // Processing the animation to play.
    private void ProcessAnimation() {
        string animationName = GetAnimationName();

        if (!IsAnimationPlaying(animationName)) {
            float animationTime = IsAnimationActionEqual() ? animator.GetCurrentAnimatorStateInfo(0).normalizedTime : 0.0f;
            animator.Play(animationName, 0, animationTime);
            currentlyPlayingAnimation = animationName;
        }
    }

    // Process the direction the player is facing.
    private void ProcessLookDirection() {
        Direction newDirection = Direction.Front;
        float rotationReference = transform.Find(Constants.gameObjectRotationReference).rotation.eulerAngles.y;

        foreach (DirectionAngle directionAngle in directionAngles) {
            if (directionAngle.GetDirection().Equals(Direction.Back)) {
                if (rotationReference >= directionAngle.GetMin() || rotationReference <= directionAngle.GetMax()) {
                    newDirection = directionAngle.GetDirection();
                }
            } else {
                if (rotationReference >= directionAngle.GetMin() && rotationReference <= directionAngle.GetMax()) {
                    newDirection = directionAngle.GetDirection();
                }
            }
        }

        lookDirection = newDirection;
    }

    // Process the direction the player is moving.
    private void ProcessMoveDirection() {
        Direction newDirection = Direction.Front;

        Vector2 input = new Vector2(InputManager.instance.GetVerticalInput(), InputManager.instance.GetHorizontalInput());
        if (playerMovementController.IsRolling()) {
            input = playerMovementController.GetRollInput();
        }
        newDirection = GetDirectionForPlayerInput(input);

        moveDirection = newDirection;
    }

    // Process the player's move state.
    private void ProcessMoveState() {
        PlayerAnimation newAnimation = PlayerAnimation.Idle;

        if (playerMovementController.IsRolling()) {
            newAnimation = PlayerAnimation.Roll;
        } else if (playerMovementController.IsMoving()) {
            newAnimation = PlayerAnimation.Run;
        }

        animation = newAnimation;
    }

    // Process the direction the player is running relative to where they are looking.
    private void ProcessRunDirection() {
        RunDirection newDirection = RunDirection.Forwards;

        if (!animation.Equals(PlayerAnimation.Roll) && IsMoveDirectionBackwards()) {
            newDirection = RunDirection.Backwards;
        }

        runDirection = newDirection;
    }
}
