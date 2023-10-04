using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalRicochet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayRandomRicochet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Play a random ricochet animation.
    private void PlayRandomRicochet() {
        int index = Random.Range(1, 4);
        string animationName = Constants.animationMetalRicochet + Constants.splitCharUnderscore + index;
        transform.Find(Constants.gameObjectSprite).GetComponent<Animator>().Play(animationName);
    }
}
