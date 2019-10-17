using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MainMenu : MonoBehaviour {

    public GameObject CurrentMenu;
    public List<GameObject> Menus = new List<GameObject>();
    public NetworkManager NetworkManager;

	// Use this for initialization
	void Start () {
        MenuStack = new Stack<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
    }

    Stack<GameObject> MenuStack;

    public void BackButton()
    {
        if (MenuStack.Count > 0)
        {
            CurrentMenu.SetActive(false);
            CurrentMenu = MenuStack.Pop();
            CurrentMenu.SetActive(true);
        }
    }

    public void ChangeMenu(int value)
    {
        if (Menus.Count > value && value >= 0)
        {
            CurrentMenu.SetActive(false);
            MenuStack.Push(CurrentMenu);
            CurrentMenu = Menus[value];
            CurrentMenu.SetActive(true);
        }
    }


    public void ConnectToServer()
    {
        string text = GameObject.Find("IPInputField").GetComponent<InputField>().text;
        Debug.Log("Connecting to server: " + text);
        
        NetworkManager.networkAddress = text;
        NetworkManager.StartClient();
    }

    public void ConnectToLocalHost()
    {
        NetworkManager.networkAddress = "127.0.0.1";
        NetworkManager.StartClient();
    }


    public void HostServer()
    {
        Debug.Log("Hosting Server");
        NetworkManager.StartHost();
    }

}
