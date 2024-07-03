using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")] 
    [SerializeField] private string fileName;
    
    public GameData gameData;
    private List<IDataPersistence> _dataPersistenceObjects;

    private FileDataHandler _dataHandler;
    
    public static DataPersistenceManager Instance;

    public bool isReset = false;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
        UpgradeLoader.Instance.onLoadComplete += WaitForStart;
    }

    private void WaitForStart()
    {
        _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        _dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        gameData = new GameData();
    }

    public void LoadGame()
    {
        Debug.Log(Application.persistentDataPath);
        gameData = _dataHandler.Load(); 
        
        if (gameData == null)
        {
            Debug.Log("No save found, new game");
            NewGame();
        }
        
        foreach (var obj in _dataPersistenceObjects)
        {
            obj.LoadData(gameData);
        }

    }
    
    public void SaveGame()
    {
        if (isReset)
        {
            _dataHandler.Save(new GameData());
            return;
        }

        gameData.minions = new List<GameData.Minion>();
        foreach (var obj in _dataPersistenceObjects)
        {
            obj.SaveData(ref gameData);
        }  
        
        _dataHandler.Save(gameData);

    }
    
    

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> objects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(objects);
    }
}
