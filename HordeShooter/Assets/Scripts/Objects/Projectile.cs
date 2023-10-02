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

    // Handle collision.
    private void Collide(RaycastHit hit) {
        if (hit.collider.gameObject.CompareTag(Constants.tagWall) || hit.collider.gameObject.CompareTag(Constants.tagPlayer) || hit.collider.gameObject.CompareTag(Constants.tagObject)) {
            if (!hit.collider.gameObject.CompareTag(Constants.tagPlayer)) {
                CreatePlasmaRicochet(hit);
            }
            Destroy(gameObject);
        }
    }

    // Create plasma ricochet on hit.
    private void CreatePlasmaRicochet(RaycastHit hit) {
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
            PrefabManager.instance.GetPrefab(Constants.gameObjectPlasmaRicochet),
            ricochetLocation,
            Quaternion.Euler(0, rotation, 0)
        );

        // Destroy ricochet prefab.
        Destroy(ricochetObject, 1);
    }
}
