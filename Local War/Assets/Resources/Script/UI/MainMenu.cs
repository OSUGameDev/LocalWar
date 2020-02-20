using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEditor;
using System.IO;
using System.Linq;


public class MainMenuSettings
{
    public string username;
    static MainMenuSettings()
    {
        CustomSerializer.AddSerializer<MainMenuSettings>();
    }
    public MainMenuSettings()
    {
    }

    public static MainMenuSettings primary
    {
        get {
            if (File.Exists("MainMenuSettings.xml"))
            {
                using (var fp = File.OpenRead("MainMenuSettings.xml"))
                    return CustomSerializer.Deserialize<MainMenuSettings>(fp);
            }
            else
            {
                MainMenuSettings settings = new MainMenuSettings();
                using (var fp = File.Create("MainMenuSettings.xml"))
                    CustomSerializer.Serialize(settings, fp);
                return settings;
            }
        }
    }   
}



public class MainMenu : MonoBehaviour {

    public GameObject CurrentMenu;
    public List<GameObject> Menus = new List<GameObject>();
    public GameObject NetworkManagerPrefab;
    public Dropdown LevelDropDown;
    public InputField UserNameInput;
    private GameObject NetworkManager;
    public LevelInfo[] PlayableLevels;
    MainMenuSettings settings;

    public TextAsset NounList;

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
        
        if(File.Exists("MainMenuSettings.xml"))
        {
            using (var fp = File.OpenRead("MainMenuSettings.xml"))
                settings = CustomSerializer.Deserialize<MainMenuSettings>(fp);
        }
        else
        {
            settings = new MainMenuSettings();
            var list = NounList.text.Split(new[] { ',', '\n' }).Where(item => !string.IsNullOrEmpty(item)).ToArray();
            System.Random rand = new System.Random();
            settings.username = list[rand.Next(list.Length)];
            using (var fp = File.Create("MainMenuSettings.xml"))
                CustomSerializer.Serialize(settings, fp);
        }
        UserNameInput.text = settings.username;

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

    public void SaveSettings()
    {
        settings.username = UserNameInput.text;
        using (var fp = File.Create("MainMenuSettings.xml"))
            CustomSerializer.Serialize(settings, fp);
        BackButton();
    }

    public void ChangeMenuLoadSettings(int value)
    {
        UserNameInput.text = settings.username;
        ChangeMenu(value);
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
