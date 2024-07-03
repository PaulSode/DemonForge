
using UnityEngine;
using UnityEngine.UI;


public abstract class Upgrade : MonoBehaviour
{
    public Sprite icon { get; private set; }
    public string upgradeName;
    public string description;
    public BFN cost;
    public Button buyButton;

    public abstract void DoUpgrade();
}