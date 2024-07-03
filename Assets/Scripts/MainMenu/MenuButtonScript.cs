using TMPro;
using UnityEngine;

public class MenuButtonScript : MonoBehaviour
{
    private TMP_Text _tmpText;
    private string _textContent;

    private void Awake()
    {
        _tmpText = GetComponentInChildren<TMP_Text>();
        if (_tmpText == null)
        {
            Debug.Log("ButtonScript used on an object with no TMP_Text in children");
            return;
        }
        _textContent = _tmpText.text;
    }

    public void MouseEnter()
    {
        _tmpText.fontStyle = FontStyles.Bold;
    }

    public void MouseExit()
    {
        _tmpText.fontStyle = FontStyles.Normal;
    }
}
