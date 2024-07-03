using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stats : MonoBehaviour
{
    public Dictionary<Stat, BFN> instanceStats = new Dictionary<Stat, BFN>()
    {
        {Stat.damage, BFN.Zero},
        {Stat.revenue, BFN.Zero},
        {Stat.price , BFN.Zero}
    };
    public List<StatsUpgrade> appliedUpgrades = new List<StatsUpgrade>();

    public event Action onUpgradeApplied;

    public BFN GetStat(Stat stat)
    {
        if (stat != Stat.price)
        {
            if (instanceStats.TryGetValue(stat, out var instanceValue))
                return GetUpgradedValue(stat, instanceValue);
            else
            {
                return BFN.Zero;
            }
        }
        else
        {
            if (instanceStats.TryGetValue(stat, out var instanceValue))
                return GetPriceUpgradedValue(stat, instanceValue);
            else
            {
                return BFN.Zero;
            }
        }
    }

    public void UnlockUpgrade(StatsUpgrade upgrade)
    {
        if (!appliedUpgrades.Contains(upgrade))
        {
            appliedUpgrades.Add(upgrade);
            onUpgradeApplied?.Invoke();
        }
    }

    private BFN GetUpgradedValue(Stat stat, BFN baseValue)
    {
        foreach (var upgrade in appliedUpgrades)
        {
            if (upgrade.upgradeToApply == stat)
            {
                if (upgrade.isPercentUpgrade)
                    baseValue *= (upgrade.upgradeValue / BFN.Hundred);
                else
                    baseValue += upgrade.upgradeValue;
            }
        }
        return baseValue;
    }

    private BFN GetPriceUpgradedValue(Stat stat, BFN baseValue)
    {
        foreach (var upgrade in appliedUpgrades)
        {
            if (upgrade.upgradeToApply == stat)
            {
                var upgradeValue = upgrade.upgradeValue;

                if (upgrade.isPercentUpgrade)
                    baseValue -= baseValue - (upgradeValue / BFN.Hundred * baseValue);
                else
                    baseValue -= upgradeValue;
            }
        }

        return baseValue;
    }
    
    public void ResetAppliedUpgrades()
    {
        appliedUpgrades.Clear();
    }
}