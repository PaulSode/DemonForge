using System.Collections.Generic;
using UnityEngine;

public class StatsUpgrade : Upgrade
{
    [SerializeField]
    public List<MinionScript> unitsToUpgrade = new List<MinionScript>();

   
    public Stat upgradeToApply;
    public BFN upgradeValue;
    public bool isPercentUpgrade = false;

    public bool bought = false;

    private void Start()
    {
        RessourcesManager.onGoldUpdate += CheckGold;
        buyButton.interactable = false;
    }

    public override void DoUpgrade()
    {
        if (RessourcesManager.Instance.gold >= cost)
        {
            RessourcesManager.Instance.AddRemGold(cost, false);
            foreach (var unitToUpgrade in unitsToUpgrade)
            {
                unitToUpgrade.stats.UnlockUpgrade(this);
            }

            RessourcesManager.onGoldUpdate -= CheckGold;
            bought = true;
            gameObject.SetActive(false);
        }
        else
        {
            Debug.Log($"Not enough money : got {RessourcesManager.Instance.gold.ToString()}, need {cost.ToString()}");
        }
    }

    public void DoLoadUpgrade()
    {
        foreach (var unitToUpgrade in unitsToUpgrade)
        {
            unitToUpgrade.stats.UnlockUpgrade(this);
        }
        bought = true;
        gameObject.SetActive(false);
    }

    public void CheckGold()
    {
        buyButton.interactable = RessourcesManager.Instance.gold >= cost;
    }
}