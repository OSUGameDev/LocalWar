using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {

    public Text HealthText;
    public Text ShieldText;
    public Text RespawnText;
    public GameObject Crosshair;

    private bool IsPlayerActive = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (CharacterLifeSystems.LocalPlayer == null)
            return;

        if (CharacterLifeSystems.LocalPlayer.gameObject.activeSelf)
        {
            if(!IsPlayerActive)
            {
                ShieldText.enabled = true;
                HealthText.enabled = true;
                RespawnText.enabled = false;
                Crosshair.SetActive(true);
            }
            ShieldText.text = "Shield: " + CharacterLifeSystems.LocalPlayer.shield;
            HealthText.text = "Health: " + CharacterLifeSystems.LocalPlayer.health;
            IsPlayerActive = true;
        }
        else
        {
            if (IsPlayerActive)
            {
                ShieldText.enabled = false;
                HealthText.enabled = false;
                RespawnText.enabled = true;
                Crosshair.SetActive(false);
            }
            RespawnText.text = "Respawning in " + (CharacterLifeSystems.LocalPlayer.nextSpawnTime - System.DateTime.Now).TotalSeconds.ToString("F1");
            IsPlayerActive = false;
        }

        if(Input.GetButtonDown("MultiPlayerMenu"))
        {
        }
        if(Input.GetButtonUp("MultiPlayerMenu"))
        {
        }
    }
}
