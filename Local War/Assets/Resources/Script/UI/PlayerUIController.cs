using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour {

    public Text HealthText;
    public Text ShieldText;
    public Text RespawnText;
    public GameObject Crosshair;
    public bool isNetworkDebug = false;
    private bool IsPlayerActive = true;
    public bool ShowMultiPlayerUI;
    public GameObject MultiplayerRowPrefab;
    private GameObject MultiplayerMenu;
    // Use this for initialization
    void Start () {
        MultiplayerMenu = transform.Find("MultiplayerDisplayGrid").gameObject;
        SessionInformationManager.Players.PropertyChanged += regenUIMultiplayer;
    }
	
    private void regenUIMultiplayer(object sender, PropertyChangedEventArgs args)
    {
        
        for (int i = MultiplayerMenu.transform.GetChild(0).childCount; i > 1; i--)
        {
            GameObject.DestroyImmediate(MultiplayerMenu.transform.GetChild(0).GetChild(i-1).gameObject);
        }
        MultiplayerMenu.transform.GetChild(0).GetChild(0).GetChild(4).gameObject.SetActive(isNetworkDebug);

        foreach (var playerInfo in SessionInformationManager.Players.getPlayers())
        {
            var newRow = GameObject.Instantiate(MultiplayerRowPrefab, MultiplayerMenu.transform.GetChild(0));
            newRow.transform.GetChild(0).GetComponent<Text>().text = playerInfo.Name;
            newRow.transform.GetChild(1).GetComponent<Text>().text = playerInfo.Kills.ToString();
            newRow.transform.GetChild(2).GetComponent<Text>().text = playerInfo.Deaths.ToString();
            newRow.transform.GetChild(3).GetComponent<Text>().text = playerInfo.Team.ToString();
            newRow.transform.GetChild(4).GetComponent<Text>().text = playerInfo.HashCode.ToString();
            newRow.transform.GetChild(4).gameObject.SetActive(isNetworkDebug); 
        }
    }

    private void OnDestroy()
    {
        SessionInformationManager.Players.PropertyChanged -= regenUIMultiplayer;
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

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ShowMultiPlayerUI = !ShowMultiPlayerUI;
            MultiplayerMenu.gameObject.SetActive(ShowMultiPlayerUI);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CharacterLifeSystems.LocalPlayer.CmdSetTeamRequest(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            CharacterLifeSystems.LocalPlayer.CmdSetTeamRequest(2);
        }
    }
}
