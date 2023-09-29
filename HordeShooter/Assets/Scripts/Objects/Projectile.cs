using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 trajectory;
    public void SetTrajectory(Vector2 trajectory) { this.trajectory = trajectory; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(trajectory.x, 0, trajectory.y) * GameManager.instance.GetEnemyProjectileSpeed() * Time.deltaTime);
    }
}
