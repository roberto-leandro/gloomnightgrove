using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton class to manage user input with keybindings that can be changed in runtime.
/// </summary>
public class KeyBinding : MonoBehaviour
{
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    public Dictionary<string, KeyCode> Keys { get { return keys; } }
    [SerializeField] public Text up, down, left, right, jump, switchAnimal;
    private GameObject currentKey;
    private Color32 normal = new Color32(244,78,242,255);
    private Color32 selected = new Color32(109, 26, 108, 255);

    // Singleton instance
    private static KeyBinding _instance;

    public static KeyBinding Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            //Destroy the instance if it is different from an existing instance.
            Destroy(gameObject);
        }

        // Make sure the instance is kept alive at all times
        DontDestroyOnLoad(gameObject);
        SetDefaultKeys();
    }

    public void SetDefaultKeys()
    {
        keys.Add("Up", KeyCode.UpArrow);
        keys.Add("Down", KeyCode.DownArrow);
        keys.Add("Right", KeyCode.RightArrow);
        keys.Add("Left", KeyCode.LeftArrow);

        keys.Add("Jump", KeyCode.Z);
        keys.Add("Switch", KeyCode.X);

        up.text = keys["Up"].ToString();
        down.text = keys["Down"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();

        jump.text = keys["Jump"].ToString();
        switchAnimal.text = keys["Switch"].ToString();
    }

    public bool GetKeyDown(string keyName)
    {
        return Input.GetKeyDown(keys[keyName]);
    }

    public bool GetKey(string keyName)
    {
        return Input.GetKey(keys[keyName]);
    }

    private void OnGUI()
    {
        if (currentKey != null) {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currentKey.name] = e.keyCode;
                currentKey.transform.GetChild(0).GetComponent<Text>().text = e.keyCode.ToString();
                currentKey.GetComponent<Image>().color = normal;
                currentKey = null;
            }
        }
    }

    public void ChangeKey(GameObject key) {

        if (currentKey != null) {
            currentKey.GetComponent<Image>().color = normal;
        }

        currentKey = key;
        currentKey.GetComponent<Image>().color = selected;
    }
  
}
