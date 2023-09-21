using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationReference : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = InputManager.instance.GetEnvironmentRaycastToMouse();
        transform.LookAt(mousePosition);
        Debug.DrawRay(transform.position, transform.forward * 5, Color.red);
    }
}
