using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private EnemyBehaviorController enemyBehaviorController;
    
    private enum EnemyAnimation {
        Idle,
        Run,
        Walk
    }
    private EnemyAnimation animation = EnemyAnimation.Idle;

    private enum EnemyDirection {
        Back,
        Front,
        FrontLeft,
        FrontRight,
        Left,
        RearLeft,
        RearRight,
        Right
    }
    private EnemyDirection direction = EnemyDirection.Front;

    private Animator animator;
    private string currentlyPlayingAnimation = "";

    private Vector3 previousPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyBehaviorController = GetComponent<EnemyBehaviorController>();
        
        animator = transform.Find(Constants.gameObjectSprite).GetComponent<Animator>();

        previousPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        ProcessAnimation();
        ProcessDirection();
        ProcessMoveState();
    }

    // Get the name of the animation that needs to be playing.
    private string GetAnimationName() {
        string animationName = "";

        // Get the animation name.
        animationName += animation.ToString();

        // Get the direction.
        animationName += Constants.splitCharUnderscore + direction.ToString();

        return animationName;
    }

    // Check if a specified animation is playing.
    private bool IsAnimationPlaying(string animationName) {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    // Check if the animation playing is equal to the current animation to be played.
    private bool IsAnimationActionEqual() {
        string animationPrefix = "";
        animationPrefix += animation.ToString();
        return currentlyPlayingAnimation.Split(Constants.splitCharUnderscore)[0].Equals(animationPrefix);
    }

    // Process the animation the enemy is playing.
    private void ProcessAnimation() {
        string animationName = GetAnimationName();

        if (!IsAnimationPlaying(animationName)) {
            float animationTime = IsAnimationActionEqual() ? animator.GetCurrentAnimatorStateInfo(0).normalizedTime : 0.0f;
            animator.Play(animationName, 0, animationTime);
            currentlyPlayingAnimation = animationName;
        }
    }

    // Process the direction the enemy needs to face.
    private void ProcessDirection() {
        Vector3 currentPosition = transform.position;

        Vector3 aimOffset = (currentPosition - previousPosition).normalized;
        if (enemyBehaviorController.IsAttacking()) {
            aimOffset = GameObject.FindGameObjectWithTag(Constants.tagPlayer).transform.position - transform.position;
        }
        
        EnemyDirection newDirection = direction;
        // Back
        if (aimOffset.z > 0 && Mathf.Abs(aimOffset.x) <= 0.5f) {
            newDirection = EnemyDirection.Back;
        }
        // RearRight
        else if ((aimOffset.x > 0 && aimOffset.z > 0) && (Mathf.Abs(aimOffset.x) > 0.5f && Mathf.Abs(aimOffset.z) > 0.5f)) {
            newDirection = EnemyDirection.RearRight;
        }
        // Right
        else if (aimOffset.x > 0 && Mathf.Abs(aimOffset.z) <= 0.5f) {
            newDirection = EnemyDirection.Right;
        }
        // FrontRight
        else if ((aimOffset.x > 0 && aimOffset.z < 0) && (Mathf.Abs(aimOffset.x) > 0.5f && Mathf.Abs(aimOffset.z) > 0.5f)) {
            newDirection = EnemyDirection.FrontRight;
        }
        // Front
        else if (aimOffset.z < 0 && Mathf.Abs(aimOffset.x) <= 0.5f) {
            newDirection = EnemyDirection.Front;
        }
        // FrontLeft
        else if ((aimOffset.x < 0 && aimOffset.z < 0) && (Mathf.Abs(aimOffset.x) > 0.5f && Mathf.Abs(aimOffset.z) > 0.5f)) {
            newDirection = EnemyDirection.FrontLeft;
        }
        // Left
        else if (aimOffset.x < 0 && Mathf.Abs(aimOffset.z) <= 0.5f) {
            newDirection = EnemyDirection.Left;
        }
        // RearLeft
        else if ((aimOffset.x < 0 && aimOffset.z > 0) && (Mathf.Abs(aimOffset.x) > 0.5f && Mathf.Abs(aimOffset.z) > 0.5f)) {
            newDirection = EnemyDirection.RearLeft;
        }

        previousPosition = currentPosition;
        
        direction = newDirection;
    }

    // Process the animation state of the enemy.
    private void ProcessMoveState() {
        EnemyAnimation newAnimation = EnemyAnimation.Idle;

        if (enemyBehaviorController.IsMoving() && !enemyBehaviorController.IsPaused()) {
            newAnimation = EnemyAnimation.Walk;
        } else if (enemyBehaviorController.IsScattering() && !enemyBehaviorController.IsPaused()) {
            newAnimation = EnemyAnimation.Run;
        }

        animation = newAnimation;
    }
}
