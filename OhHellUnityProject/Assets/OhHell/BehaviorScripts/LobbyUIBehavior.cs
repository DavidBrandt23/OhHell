using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyUIBehavior : NetworkBehaviour
{
    public GameObject LobbyPlayerPrefab;
    public GameObject canvasObject;
    public SelectableObjectBehavior startButtonSelectableBeh;
    public GameObject startButtonObject;
    public GameObject waitingForHostMessageObject;
    public UnityEvent StartEvent;
    private int numPlayers;
    public bool DebugMode;
    // Start is called before the first frame update
    void Start()
    {
        startButtonSelectableBeh.clicked = new UnityEvent();
        startButtonSelectableBeh.clicked.AddListener(OnStartClick);
        if (DebugMode)
        {
            SetNames(new List<string> { "sdfsf", "dsfsdfds", "sss" });
        }
    }
    public void OnStartClick()
    {
        StartEvent.Invoke();
    }
    [ClientRpc]
    public void SetNames(List<string> names)
    {
        foreach (Transform child in canvasObject.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < 8; i++)
        {
            GameObject newPlayerOb = Instantiate(LobbyPlayerPrefab, canvasObject.transform);
            string playerName = "";
            if(names.Count > i)
            {
                playerName = names[i];
            }
            newPlayerOb.GetComponent<TextMeshPro>().text = playerName;
            newPlayerOb.transform.localPosition = new Vector3(0.0f, 12.0f+ i * -5.0f, 0.0f);
            if (playerName == "")
            {
                newPlayerOb.GetComponentInChildren<SpriteRenderer>().color = new Color(0.3f,0.3f,0.3f);
            }
        }

        startButtonObject.SetActive(isServer);
        waitingForHostMessageObject.SetActive(!isServer);

        numPlayers = names.Count;
    }

    // Update is called once per frame
    void Update()
    {
        startButtonSelectableBeh.ClickEnabled = numPlayers > 1;
    }
}
