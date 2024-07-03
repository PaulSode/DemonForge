using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<GameObject> tabs;

    [SerializeField] private List<TabButton> tabButtons;
    [SerializeField] private  Color colorIdle;
    [SerializeField] private  Color colorHover;
    [SerializeField] private  Color colorSelected;

    private TabButton _selectedTab;
    public void Subscribe(TabButton button)
    {
        tabButtons ??= new List<TabButton>();

        tabButtons.Add(button);
        if (_selectedTab == null)
        {
            _selectedTab = button;
        }
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if (_selectedTab == null || button != _selectedTab)
        {
            button.bg.color = colorHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        _selectedTab = button;
        ResetTabs();
        button.bg.color = colorSelected;

        int index = button.transform.GetSiblingIndex();
        for (var i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetActive(i == index);
        }
    }

    private void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            if (_selectedTab != null && _selectedTab == button) continue;
            button.bg.color = colorIdle;
        }
    }
}
