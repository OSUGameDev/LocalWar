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
        if (LifeSys.playerLifeSystem == null)
            return;

        if (LifeSys.playerLifeSystem.gameObject.activeSelf)
        {
            if(!IsPlayerActive)
            {
                ShieldText.enabled = true;
                HealthText.enabled = true;
                RespawnText.enabled = false;
                Crosshair.SetActive(true);
            }
            ShieldText.text = "Shield: " + LifeSys.playerLifeSystem.shield;
            HealthText.text = "Health: " + LifeSys.playerLifeSystem.health;
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
            RespawnText.text = "Respawning in " + (LifeSys.playerLifeSystem.nextSpawnTime - System.DateTime.Now).TotalSeconds.ToString("F1");
            IsPlayerActive = false;
        }
    }
}
