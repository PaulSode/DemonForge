using System;
using TMPro;
using UnityEngine;

public enum BuyMode
{
    One,
    Ten,
    Hundred,
    Max
}

public class BulkButton : MonoBehaviour
{


    public BuyMode mode;
    [SerializeField] private TMP_Text tmpText;


    public static event Action<BuyMode> onBuyModeUpdate; 
    private void Start()
    {
        mode = BuyMode.One;
        tmpText.text = "x1";
    }

    public void RotateMode()
    {
        switch (mode)
        {
            case BuyMode.One:
                mode = BuyMode.Ten;
                UpdateButton("x10");
                break;
            case BuyMode.Ten:
                mode = BuyMode.Hundred;
                UpdateButton("x100");
                break;
            case BuyMode.Hundred:
                mode = BuyMode.Max;
                UpdateButton("MAX");
                break;
            default:
                mode = BuyMode.One;
                UpdateButton("x1");
                break;
        }
        onBuyModeUpdate?.Invoke(mode);
    }

    private void UpdateButton(string text)
    {
        tmpText.text = text;
    }
}
