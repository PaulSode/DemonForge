using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    [SerializeField] private TabGroup group;
    public Image bg;

    private void Start()
    {
        group.Subscribe(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        group.OnTabEnter(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        group.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        group.OnTabExit(this);
    }
}
