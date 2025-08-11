using TMPro;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour
{
    public static MainMenuUIManager Instance { get; private set; }

    public string playerName { get; private set; } = "john doe";
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject waitingPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    public void OpenWaitingPanel()
    {
        mainPanel.SetActive(false);
        waitingPanel.SetActive(true);
    }

    public void ChangeName(string name)
    {
        playerName = name;
    }
}
