using TMPro;
using UnityEngine;

public class TooltipScript : MonoBehaviour
{
    [SerializeField] private bool isMinion;
    [SerializeField] private MinionScript _minion;
    [SerializeField] private StatsUpgrade _upgrade;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private TMP_Text _subText;
    [SerializeField] private RectTransform backgroundRect;

    public void Show()
    {
        backgroundRect.gameObject.SetActive(true);
        _text.text = isMinion ? _minion.description : _upgrade.description;
        _subText.text = isMinion ? _minion.initialDamage.ToString() : _upgrade.cost.ToString();
    }

    public void Hide()
    {
        backgroundRect.gameObject.SetActive(false);
    }
}