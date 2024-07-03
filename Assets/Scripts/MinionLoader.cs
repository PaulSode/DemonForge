using System;
using UnityEngine;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class Minion
{
    public string name;
    public int priceCoef;
    public int priceExp;
    public int damageCoef;
    public int damageExp;
    public string image;
    public string description;
}

[System.Serializable]
public class MinionList
{
    public List<Minion> minions;
}

public class MinionLoader : MonoBehaviour
{
    public static MinionLoader Instance = null;
    
    public List<Minion> minions;
    [SerializeField] private TextAsset minionsTextAsset;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject minionPrefab;

    public List<MinionScript> minionScripts;

    public event Action onMinionLoadComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        LoadMinions();
    }

    void LoadMinions()
    {
        
        if (minionsTextAsset != null)
        {
            var minionList = JsonUtility.FromJson<MinionList>(minionsTextAsset.text);
            minions = minionList.minions;
            
            int i = 0;
            foreach (Minion minion in minions)
            {
                CreateMinion(minion, i);
                i++;
            }

           
        }
        else
        {
            Debug.LogError("Cannot load minions.json");
        }
        onMinionLoadComplete?.Invoke();
    }

    void CreateMinion(Minion minion, int index)
    {
        StoryManager.Instance.thresholds.Add(new BFN(minion.priceCoef, minion.priceExp));
        
        var minionObject = Instantiate(minionPrefab, container.transform);
        
        minionObject.name = minion.name;

        minionObject.GetComponent<MinionScript>()._monsterIndex = index;
        minionObject.GetComponent<MinionScript>().minionName = minion.name;
        minionObject.GetComponent<MinionScript>().threshold = new BFN(minion.priceCoef, minion.priceExp);
        minionObject.GetComponent<MinionScript>().initialDamage = new BFN(minion.damageCoef, minion.damageExp);
        minionObject.GetComponent<MinionScript>().description = minion.description;
        //image = Resources.Load<Sprite>(minion.image);
        
        minionScripts.Add(minionObject.GetComponent<MinionScript>());
    }
}