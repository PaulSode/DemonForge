using UnityEngine;

public class UpgradeListCanvas : MonoBehaviour
{
    public int index;

    
    //This is a script made to change the drawing order of items in a list (here the UpgradeLoader upgrades list) so that
    //The fist item of the list is drawn in front of the others 
    // By default, Unity puts items lower in the hierarchy (AKA later items) in from of the objects higher (AKA the firsts)
    private void Start()
    {
        GetComponent<Canvas>().sortingOrder = UpgradeLoader.Instance.upgrades.Count - index + 10;
    }
}
