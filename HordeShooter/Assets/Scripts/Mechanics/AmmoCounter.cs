using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounter : MonoBehaviour
{
    private PlayerAttackController playerAttackController;
    private Text ammoText;
    
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.instance.DoesPlayerExist()) {
            playerAttackController = GameObject.FindGameObjectWithTag(Constants.tagPlayer).GetComponent<PlayerAttackController>();
        }

        ammoText = transform.Find(Constants.gameObjectText).GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.DoesPlayerExist()) {
            ammoText.text = playerAttackController.GetAmmoInClip() + " / " + GameManager.instance.GetPlayerClipSize();
        }
    }
}
