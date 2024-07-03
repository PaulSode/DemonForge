using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public void ResetGame()
    {
        Debug.Log("reset");
        DataPersistenceManager.Instance.isReset = true;
        Application.Quit();
    }
}
