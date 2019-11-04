using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEditor;
using System.IO;

public class MainMenu : MonoBehaviour {

    public GameObject CurrentMenu;
    public List<GameObject> Menus = new List<GameObject>();
    public GameObject NetworkManagerPrefab;
    public Dropdown LevelDropDown;
    private GameObject NetworkManager;

    public LevelInfo[] PlayableLevels;

	// Use this for initialization
	void Start () {
        MenuStack = new Stack<GameObject>();
        NetworkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        if(NetworkManager != null)
        {
            DestroyImmediate(NetworkManager);
        }
        NetworkManager = Instantiate(NetworkManagerPrefab);
        DontDestroyOnLoad(NetworkManager);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(Application.isBatchMode)
        {
            Application.targetFrameRate = 30;
            NetworkManager.GetComponent<NetworkManager>().StartServer();
        }
        PlayableLevels = LevelInfoCollection.DefaultLevels;
        foreach (var scene in PlayableLevels)
            LevelDropDown.options.Add(new Dropdown.OptionData(scene.Name));
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
        
        NetworkManager.GetComponent<NetworkManager>().networkAddress = text;
        NetworkManager.GetComponent<NetworkManager>().StartClient();
    }

    public void ConnectToLocalHost()
    {
        NetworkManager.GetComponent<NetworkManager>().networkAddress = "127.0.0.1";
        NetworkManager.GetComponent<NetworkManager>().StartClient();
    }


    public void HostServer()
    {
        Debug.Log("Hosting Server");


        NetworkManager.GetComponent<NetworkManager>().onlineScene = PlayableLevels[LevelDropDown.value].Name;
        NetworkManager.GetComponent<NetworkManager>().StartHost();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

}
