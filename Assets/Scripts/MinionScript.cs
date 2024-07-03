using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Component = UnityEngine.Component;

public class MinionScript : MonoBehaviour, IDataPersistence
{
    [Header("Values")]
    public int _monsterIndex;
    public string minionName = "Unknown";
    public Sprite image;
    public BFN threshold;
    public string description;
    
    public float multiplier = 1.07f;
    [FormerlySerializedAs("level")] public int monsterLevel = 0;
    public BFN initialDamage;

    [Header("TMP")] [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Button buyButton;

    [Header("Stats")]
    public Stats stats;
    
    public BFN damageValue;
    public BFN priceValue;

    private BuyMode _buyMode = BuyMode.One;
    private BFN _buyModePrice;
    private int _maxBuyCount = 0;
    

    private void Awake()
    {
        buyButton.interactable = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
        RessourcesManager.onGoldUpdate += TryAppear;
        stats.onUpgradeApplied += UpdateFrontStats;
        BulkButton.onBuyModeUpdate += OnBuyModeUpdate;
    }

    private void Start()
    {
        nameText.text = minionName;
        
        //If monster level is 0 then we initialize it, else the LoadData function will take care of it
        if (monsterLevel == 0)
        {
            priceValue = threshold;
            damageValue = BFN.Zero;
            stats.instanceStats[Stat.damage] = damageValue;
            stats.instanceStats[Stat.price] = priceValue;
            _buyModePrice = priceValue;
            
            UpdateUI();
        }
    }

    private void TryAppear()
    {
        if (RessourcesManager.Instance.gold > threshold / 2)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(true);
            RessourcesManager.onGoldUpdate -= TryAppear;
            RessourcesManager.onGoldUpdate += UpdateClickable;
        }
    }

    private void UpdateClickable()
    {
        if (_buyMode == BuyMode.Max && RessourcesManager.Instance.gold >= _buyModePrice)
        {
            _buyModePrice = CalculateMaxPrice();
            UpdateUI();
        }
        buyButton.interactable = RessourcesManager.Instance.gold >= _buyModePrice;
    }

    public void OnClick()
    {
        
                if (RessourcesManager.Instance.gold >= _buyModePrice)
                {
                    RessourcesManager.Instance.AddRemGold(_buyModePrice, false);
                    UpdateClickable();

                    switch (_buyMode)
                    {
                        case BuyMode.One:
                            monsterLevel++;
                            break;
                        case BuyMode.Ten:
                            monsterLevel += 10;
                            break;
                        case BuyMode.Hundred:
                            monsterLevel += 100;
                            break;
                        case BuyMode.Max:
                            monsterLevel += _maxBuyCount;
                            break;
                    }
            
                    
                    
                    damageValue = initialDamage * monsterLevel;
                    stats.instanceStats[Stat.damage] = damageValue;
            
                    priceValue = threshold * Math.Pow(multiplier, monsterLevel);
                    stats.instanceStats[Stat.price] = priceValue;
                    
                    OnBuyModeUpdate(_buyMode);
                }
        
    }

    private void UpdateUI()
    {
        priceText.text = _buyModePrice.compressed.ToString();
        damageText.text = damageValue.compressed.ToString();
        levelText.text = monsterLevel.ToString();
    }

    private void UpdateFrontStats()
    {
        damageValue = stats.GetStat(Stat.damage);
        priceValue = stats.GetStat(Stat.price);
        UpdateUI();
    }

    private void OnBuyModeUpdate(BuyMode mode)
    {
        _buyMode = mode;
        switch (_buyMode)
        {
            case BuyMode.One:
                _buyModePrice = CalculatePriceForLevels(1);
                break;
            case BuyMode.Ten:
                _buyModePrice = CalculatePriceForLevels(10);
                break;
            case BuyMode.Hundred:
                _buyModePrice = CalculatePriceForLevels(100);
                break;
            case BuyMode.Max:
                _buyModePrice = CalculateMaxPrice();
                break;
        }
        
        UpdateClickable();
        UpdateFrontStats();
    }

    private BFN CalculatePriceForLevels(int levels)
    {
        BFN price = BFN.Zero;
        for (int i = 0; i < levels; i++)
        {
            price += threshold * Math.Pow(multiplier, monsterLevel + i);
        }
        return price;
    }

    private BFN CalculateMaxPrice()
    {
        BFN price = BFN.Zero;
        int i = 0;
        while (price < RessourcesManager.Instance.gold)
        {
            price += threshold * Math.Pow(multiplier, monsterLevel + i);
            i++;
        }

        _maxBuyCount = i-1;
        if (_maxBuyCount > 0)
        {
            return price -= threshold * Math.Pow(multiplier, monsterLevel + _maxBuyCount);
        }
        else
        {
            return price;
        }
        
    }

    public void LoadData(GameData gameData)
    {
        if (gameData.minions == null) return;

        foreach (var t in gameData.minions)
        {
            if (t.index == _monsterIndex)
            {
                monsterLevel = t.level;
                
                damageValue = initialDamage * monsterLevel;
                stats.instanceStats[Stat.damage] = damageValue;
            
                priceValue = threshold * Math.Pow(multiplier, monsterLevel);
                stats.instanceStats[Stat.price] = priceValue;

                _buyModePrice = priceValue;

                var statUpgrades = GetAllInstances<StatsUpgrade>();
                foreach (var upgradeName in t.upgrades)
                {
                    
                    var statUp = statUpgrades.FirstOrDefault(su => su.upgradeName == upgradeName);
                    if (statUp != null && !statUp.bought)
                    {
                        statUp.DoLoadUpgrade();
                    }
                }
                UpdateFrontStats();
                
                return;
            }
        }
    }

    public void SaveData(ref GameData gameData)
    {
        var upgrades = new List<string>();
        foreach (var up in stats.appliedUpgrades)
        {
            upgrades.Add(up.upgradeName);
        }
        
        gameData.minions.Add(new GameData.Minion()
        {
            index = _monsterIndex,
            level = monsterLevel,
            upgrades = upgrades
        });
    }
    
    
    
    //Used to find all instances of StatUpgrades
    //Cannot use FindObjectsOfType because by default these are incactive
    private List<T> GetAllInstances<T>() where T : Component
    {
        var results = new List<T>();
        var processedGameObjects = new HashSet<GameObject>(); // To track processed GameObjects
        var allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        foreach (var go in allGameObjects)
        {
            if (go.hideFlags != HideFlags.None || !go.scene.isLoaded) continue; // Filter out prefabs and hidden objects
            if (processedGameObjects.Contains(go)) continue; // Skip already processed GameObjects

            processedGameObjects.Add(go); // Mark this GameObject as processed
            results.AddRange(go.GetComponentsInChildren<T>(true));
        }

        return results;
    }
}