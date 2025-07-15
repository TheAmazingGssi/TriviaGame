using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [SerializeField] int ID;
    public TextMeshProUGUI text;
    public event Action<int> OnButtonClick;

    public void Click()
    {
        OnButtonClick?.Invoke(ID);
    }

}
