using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 trajectory = Vector3.zero;
    public void SetTrajectory(Vector3 trajectory) { this.trajectory = trajectory; }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(trajectory * GameManager.instance.GetEnemyProjectileSpeed() * Time.deltaTime);

        Vector3 raycastPosition = transform.position;
        Vector3 raycastDirection = transform.Find(Constants.gameObjectSprite).up;
        RaycastHit hit;
        if (Physics.Raycast(raycastPosition, raycastDirection, out hit, 0.7f)) {
            Collide(hit);
        }
    }

    private void Collide(RaycastHit hit) {
        if (hit.collider.gameObject.CompareTag(Constants.tagWall) || hit.collider.gameObject.CompareTag(Constants.tagPlayer) || hit.collider.gameObject.CompareTag(Constants.tagObject)) {
            Debug.Log("Hit " + hit.collider.gameObject.name);
            Destroy(gameObject);
        }
    }
}
