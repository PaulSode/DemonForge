using System;
using UnityEngine;

public class RessourcesManager : MonoBehaviour, IDataPersistence
{
    public static RessourcesManager Instance = null;
    public BFN gold;

    public static event Action onGoldUpdate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    public void LoadData(GameData gameData)
    {
        gold = gameData.gold;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.gold = gold;
    }

    public void AddRemGold(BFN difference, bool isAddition)
    {
        if (isAddition)
        {
            gold += difference;
        }
        else
        {
            gold -= difference;
        }

        onGoldUpdate?.Invoke();

    }

    public void SetGold(BFN value)
    {
        gold = value;
        onGoldUpdate?.Invoke();

    }

}
