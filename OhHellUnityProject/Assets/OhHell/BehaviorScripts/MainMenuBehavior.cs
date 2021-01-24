using UnityEngine;
using System.Collections;
using TMPro;

public class MainMenuBehavior : MonoBehaviour
{
    public bool DebugMode;
    public bool DefaultToDavidIP;
    public TMP_InputField nameField;
    public TMP_InputField ipField;
    public SelectableObjectBehavior joinHostSelBeh;
    public SelectableObjectBehavior startHostSelBeh;
    public GameObject HostGameButton;
    
    // Use this for initialization
    void Start()
    {
        if (DebugMode)
        {
            nameField.text = "David" + Random.Range(1, 1000);
            ipField.text = "localhost";
        }
        if (DefaultToDavidIP)
        {
            ipField.text = "172.220.105.134";
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            HostGameButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        joinHostSelBeh.ClickEnabled = (nameField.text.Length > 0 && ipField.text.Length > 0);
        startHostSelBeh.ClickEnabled = (nameField.text.Length > 0);
    }
}
