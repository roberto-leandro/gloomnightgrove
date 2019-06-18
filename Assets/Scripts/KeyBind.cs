using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBind : MonoBehaviour
{
    private Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    [SerializeField] public Text up, down, left, right, jump, switchAnimal;
    private GameObject currentKey;
    private Color32 normal = new Color32(244,78,242,255);
    private Color32 selected = new Color32(109, 26, 108, 255);

    // Start is called before the first frame update
    void Start()
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

    public void changeKey(GameObject key) {

        if (currentKey != null) {
            currentKey.GetComponent<Image>().color = normal;
        }

        currentKey = key;
        currentKey.GetComponent<Image>().color = selected;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keys["Up"]))
        {
            Debug.Log("Up we go");
        }

        if (Input.GetKeyDown(keys["Down"]))
        {
            Debug.Log("Down we go");
        }
        if (Input.GetKeyDown(keys["Right"]))
        {
            Debug.Log("Going to the Right");
        }

        if (Input.GetKeyDown(keys["Left"]))
        {
            Debug.Log("Going to the Left");
        }

        if (Input.GetKeyDown(keys["Jump"]))
        {
            Debug.Log("Juuuuump... desu mo");
        }
        if (Input.GetKeyDown(keys["Switch"])) {
            Debug.Log("Animal Change!");
        }
    }
}
