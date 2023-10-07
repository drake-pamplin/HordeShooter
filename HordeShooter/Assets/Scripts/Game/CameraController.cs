using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TrackPlayer();
    }

    // Track the target player.
    private void TrackPlayer() {
        if (GameManager.instance.DoesPlayerExist()) {
            transform.position = GameObject.FindGameObjectWithTag(Constants.tagPlayer).transform.position;
        }
    }
}
