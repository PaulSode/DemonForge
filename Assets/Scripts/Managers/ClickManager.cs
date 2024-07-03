using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public static ClickManager Instance = null;
    [SerializeField] private AudioSource _audioSource;
    
    private float _updateTimer = 1f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Click()
    {
        UpdateMoney(UpgradesManager.Instance.GetClickRevenue());
        _audioSource.Play();
    }

    private void Update()
    {
        _updateTimer -= Time.deltaTime;
        if (_updateTimer <= 0)
        {
            UpdateMoney(UpgradesManager.Instance.GetUpgradesResultRevenue());
            _updateTimer = 1f;
        }
    }

    private void UpdateMoney(BFN newMoney)
    {
        RessourcesManager.Instance.AddRemGold(newMoney, true);
        
    }
}
