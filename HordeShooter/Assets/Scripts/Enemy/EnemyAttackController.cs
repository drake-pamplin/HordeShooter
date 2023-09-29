using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Trigger an attack on the player.
    public void AttackPlayer() {
        // Get player location.
        Vector3 playerPosition = GameObject.FindGameObjectWithTag(Constants.tagPlayer).transform.position;
        
        // Create projectile.
        Vector3 direction = playerPosition - transform.position;
        direction = direction.normalized;
        GameObject projectile = Instantiate(
            PrefabManager.instance.GetPrefab(Constants.gameObjectProjectile),
            transform.position,
            Quaternion.identity
        );
        Vector3 rotation = transform.Find(Constants.gameObjectRotationReference).eulerAngles;
        rotation.x = 90;
        projectile.transform.Find(Constants.gameObjectSprite).eulerAngles = rotation;

        // Set projectile trajectory.
        projectile.GetComponent<Projectile>().SetTrajectory(transform.Find(Constants.gameObjectRotationReference).forward);
    }
}
