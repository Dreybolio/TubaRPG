using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConsoleController : MonoBehaviour
{
    private bool showConsole;
    private bool showHelp;
    string input;

    public static ConsoleCommand<string> MAP;
    public static ConsoleCommand HEAL;
    public static ConsoleCommand HELP;
    public List<object> commandList;
    
    private InputManager inputManager;

    private static ConsoleController instance;

    public static ConsoleController Instance
    {
        get { return instance; }
    }
    

    private void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        

        MAP = new ConsoleCommand<string>("map", "change level to name", "map <map_name>", (name) =>
        {
            FindObjectOfType<LevelManager>().LoadScene(name);
        });
        HEAL = new ConsoleCommand("heal", "heal party to full", "heal", () =>
        {
            Debug.LogWarning("TODO: Implement This");
        });
        HELP = new ConsoleCommand("help", "show a list of commands", "help", () =>
        {
            showHelp = true;
        });
        commandList = new List<object>
        {
            MAP,
            HEAL,
            HELP,
        };
    }
    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
    }
    private void Update()
    {
        if (inputManager.GetConsoleToggled())
        {
            OnToggleConsole();
        }
        else if (inputManager.GetConsoleSubmit())
        {
            OnSubmit();
        }
    }
    private void OnToggleConsole()
    {
        showConsole = !showConsole;
        Debug.Log("toggled console");
    }
    private void OnSubmit()
    {
        if (showConsole)
        {
            HandleInput();
            input = "";
        }
    }

    Vector2 scroll;

    private void OnGUI()
    {
        if (!showConsole)
        {
            return;
        }
        float y = 0;

        if (showHelp)
        {
            GUI.Box(new(0, y, Screen.width, 200), "");
            Rect viewport = new(0, 0, Screen.width - 30, 40 * commandList.Count);
            scroll = GUI.BeginScrollView(new(0, y + 5f, Screen.width, 190), scroll, viewport);
            for (int i = 0; i < commandList.Count; i++)
            {
                ConsoleCommandBase command = commandList[i] as ConsoleCommandBase;
                string label = $"{command.commandFormat} - {command.commandDescription}";
                Rect labelRect = new(5, 40 * i, viewport.width - 100, 40);
                GUI.Label(labelRect, label);
                GUI.skin.label.fontSize = 32;
            }
            GUI.EndScrollView();
            y += 200;
        }

        GUI.Box(new(0, y, Screen.width, 60), "");
        GUI.backgroundColor = new Color(0, 0, 0, 0);

        GUI.SetNextControlName("console");
        input = GUI.TextField(new(10f, y + 5f, Screen.width - 20f, 96f), input);
        GUI.skin.textField.fontSize = 32;
        GUI.FocusControl("console");
    }

    private void HandleInput()
    {
        string[] properties = input.Split(" ");
        foreach (object command in commandList)
        {
            ConsoleCommandBase commandBase = command as ConsoleCommandBase;
            if (input.Contains(commandBase.commandID))
            {
                if (command as ConsoleCommand != null)
                {
                    print("Invoking command: " + command);
                    (command as ConsoleCommand).Invoke();
                }
                else if (command as ConsoleCommand<string> != null)
                {
                    (command as ConsoleCommand<string>).Invoke(properties[1]);
                }
            }
        }
    }
}
