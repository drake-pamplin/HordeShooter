using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerMovementController playerMovementController;
    
    private enum PlayerAnimation {
        Idle,
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
    private Direction direction = Direction.Front;

    private struct DirectionAngle {
        public DirectionAngle(Direction newDirection, float newMin, float newMax) {
            direction = newDirection;
            min = newMin;
            max = newMax;
        }

        private Direction direction;
        public Direction GetDirection() { return direction; }
        private float min;
        public float GetMin() { return min; }
        private float max;
        public float GetMax() { return max; }
    }
    private List<DirectionAngle> directionAngles;

    private Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        
        directionAngles = new List<DirectionAngle>();
        directionAngles.Add(new DirectionAngle(Direction.Back, 337.5f, 22.5f));
        directionAngles.Add(new DirectionAngle(Direction.RearRight, 22.5f, 67.5f));
        directionAngles.Add(new DirectionAngle(Direction.Right, 67.5f, 112.5f));
        directionAngles.Add(new DirectionAngle(Direction.FrontRight, 112.5f, 157.5f));
        directionAngles.Add(new DirectionAngle(Direction.Front, 157.5f, 202.5f));
        directionAngles.Add(new DirectionAngle(Direction.FrontLeft, 202.5f, 247.5f));
        directionAngles.Add(new DirectionAngle(Direction.Left, 247.5f, 292.5f));
        directionAngles.Add(new DirectionAngle(Direction.RearLeft, 292.5f, 337.5f));
        
        animator = transform.Find(Constants.gameObjectSprite).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessAnimation();
        ProcessDirection();
        ProcessMoveState();
    }

    // Get animation name.
    private string GetAnimationName() {
        string animationName = animation.ToString() + Constants.splitCharUnderscore + direction.ToString();
        return animationName;
    }

    // Check if animation is playing.
    private bool IsAnimationPlaying(string animationName) {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    // Processing the animation to play.
    private void ProcessAnimation() {
        string animationName = GetAnimationName();

        if (!IsAnimationPlaying(animationName)) {
            animator.Play(animationName);
        }
    }

    // Process the direction the player is facing.
    private void ProcessDirection() {
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

        direction = newDirection;
    }

    // Process the player's move state.
    private void ProcessMoveState() {
        PlayerAnimation newAnimation = PlayerAnimation.Idle;

        if (playerMovementController.IsMoving()) {
            newAnimation = PlayerAnimation.Run;
        }

        animation = newAnimation;
    }
}
