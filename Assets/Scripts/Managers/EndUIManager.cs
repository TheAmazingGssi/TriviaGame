using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndUIManager : MonoBehaviour
{
    public static EndUIManager Instance { get; private set; }
    [SerializeField] private TMP_Text textBlock;

    public string playerName { get; private set; } = "john doe";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
        GameManager.Instance.PrepareWinLoseText();
    }

    public void SetText(string text)
    {
        textBlock.text = text;
    }
}
