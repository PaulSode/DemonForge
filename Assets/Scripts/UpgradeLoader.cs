using System;
using UnityEngine;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Debug = UnityEngine.Debug;

[Serializable]
public class LoadedUpgrade
{
    public string name;
    public int[] target;
    public string stat;
    public int[] value;
    public bool isPercentUpgrade;
    public int[] cost;
    public string description;
    public string image;
}

[System.Serializable]
public class UpgradeList
{
    public List<LoadedUpgrade> upgrades;
}

public class UpgradeLoader : MonoBehaviour
{
    public static UpgradeLoader Instance = null;
    
    public List<LoadedUpgrade> upgrades;
    [SerializeField] private TextAsset upgradesTextAsset;
    [SerializeField] private GameObject container;
    [SerializeField] private GameObject upgradePrefab;

    public event Action onLoadComplete;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            MinionLoader.Instance.onMinionLoadComplete += LoadUpgrades;
        }
    }

    void LoadUpgrades()
    {
        if (upgradesTextAsset != null)
        {
            var upgradeList = JsonUtility.FromJson<UpgradeList>(upgradesTextAsset.text);
            upgrades = upgradeList.upgrades;

            int i = 0;
            foreach (LoadedUpgrade upgrade in upgrades)
            {
                CreateUpgrade(upgrade, i);
                i++;
            }

            onLoadComplete?.Invoke();
        }
        else
        {
            Debug.LogError("Cannot load upgrades.json");
        }
    }

    private List<MinionScript> GetMinionsToUpgrade(LoadedUpgrade upgrade)
    {
        var minions = MinionLoader.Instance.minionScripts;
        var list = new List<MinionScript>();
        
        foreach (var index in upgrade.target)
        {
            list.Add(minions[index]);
        }

        return list;
    }

    private Stat GetStatToUpgrade(LoadedUpgrade upgrade)
    {
        switch (upgrade.stat)
        {
            case "damage":
                return Stat.damage;
            case "revenue":
                return Stat.revenue;
            case "price":
                return Stat.price;
            default:
                throw new SyntaxErrorException($"Stat {upgrade.stat} doesn't exist");
        }
    }

    void CreateUpgrade(LoadedUpgrade upgrade, int index)
    {
        
        var upgradeObject = Instantiate(upgradePrefab, container.transform);
        upgradeObject.name = upgrade.name;

        upgradeObject.GetComponent<StatsUpgrade>().upgradeName = upgrade.name;
        upgradeObject.GetComponent<StatsUpgrade>().cost = new BFN(upgrade.cost[0], upgrade.cost[1]);
        upgradeObject.GetComponent<StatsUpgrade>().unitsToUpgrade = GetMinionsToUpgrade(upgrade);
        upgradeObject.GetComponent<StatsUpgrade>().upgradeToApply = GetStatToUpgrade(upgrade);
        upgradeObject.GetComponent<StatsUpgrade>().upgradeValue = new BFN(upgrade.value[0], upgrade.value[1]);
        upgradeObject.GetComponent<StatsUpgrade>().isPercentUpgrade = upgrade.isPercentUpgrade;
        upgradeObject.GetComponent<StatsUpgrade>().description = upgrade.description;

        upgradeObject.GetComponent<UpgradeListCanvas>().index = index;

        upgradeObject.GetComponentInChildren<TMP_Text>().text = upgrade.name;
        //image

    }
}