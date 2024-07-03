using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvasManager : MonoBehaviour
{
    public static MainCanvasManager Instance = null;
    
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private Button mainButton;
    [SerializeField] private GameObject tabsButtonsContainer;

    private void Awake()
    {
        if (Instance != null) return;
        
        Instance = this;
        mainButton.gameObject.SetActive(false);
        tabsButtonsContainer.SetActive(false);
        RessourcesManager.onGoldUpdate += UpdateGold;
        RessourcesManager.onGoldUpdate += CheckForTabs;
    }


    private void UpdateGold()
    {
        goldText.text = RessourcesManager.Instance.gold.ToString();
    }

    public void ShowMainButton()
    {
        mainButton.gameObject.SetActive(true);
    }

    private void CheckForTabs()
    {
        if (RessourcesManager.Instance.gold >= BFN.Thousand)
        {
            tabsButtonsContainer.SetActive(true);
        }
        
    }
}
