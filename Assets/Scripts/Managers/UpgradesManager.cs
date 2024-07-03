using UnityEngine;

public class UpgradesManager : MonoBehaviour
{
    public static UpgradesManager Instance = null;
    [SerializeField] private GameObject minionsContainer;
    private MinionScript[] _minionScripts;

    private void Awake()
    {
        if (Instance != null) return;
        
        Instance = this;
        
    }

    private void Start()
    {
        MinionLoader.Instance.onMinionLoadComplete += OnMinionsCompletion;
    }

    private void OnMinionsCompletion()
    {
        _minionScripts = minionsContainer.GetComponentsInChildren<MinionScript>();
    }

    public BFN GetClickRevenue()
    {
        BFN damage = BFN.One;
        damage += _minionScripts[0].damageValue;
        return damage;
    }

    public BFN GetUpgradesResultRevenue()
    {
        BFN damage = BFN.Zero;
        for (var i = 1; i < _minionScripts.Length; i++)
        {
            damage += _minionScripts[i].damageValue;
        }

        return damage;
    }
}
