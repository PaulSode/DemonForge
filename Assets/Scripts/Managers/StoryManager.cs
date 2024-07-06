using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class StoryManager : MonoBehaviour, IDataPersistence
{
    public static StoryManager Instance = null;

    private Action _updateDelegate;
    
    public bool introductionDone = false;
    [SerializeField] private List<TextAsset> inkJSONAssets;
    private List<string> _assetNames;
    public List<string> _assetNamesQueue;
    private string _currentAsset;
    

    public List<BFN> thresholds;
    private BFN _currentThreshold;
    private List<BFN> _thresholdsQueue;

    private float _updateTimer;
    
    private string folderPath = "Dialogues/Minions";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _assetNamesQueue = new List<string>();
        _assetNames = new List<string>();

        _thresholdsQueue = new List<BFN>();

        _updateDelegate = Empty;
        DialogueManager.onDialogueEnd += ActivateMainButton;
        DialogueManager.onDialogueEnd += RemoveCurrent;
        
        thresholds.Add(BFN.Zero);

        //Load Dialogues
       

        var guids = Resources.LoadAll<TextAsset>(folderPath);

        foreach (var guid in guids)
        {
            if (guid != null)
            {
                _assetNames.Add(guid.name);
                inkJSONAssets.Add(guid);
            }
        }
    }

    private void Start()
    {
        _updateTimer = 2f;
    }

    private void Update()
    {
        _updateTimer -= Time.deltaTime;
        if (inkJSONAssets.Count > 0 && _updateTimer <= 0)
        {
           OnTimerEnd();
            _updateTimer = 1f;
        }
    }


    private void OnTimerEnd()
    {
        _updateDelegate();
        for (var i = 0; i < thresholds.Count; i++)
        {
            if (RessourcesManager.Instance.gold < thresholds[i]) return;

            if (inkJSONAssets.Count < 1) return;
            
            DialogueManager.Instance.EnterDialogue(inkJSONAssets[i]);
            if (_currentAsset != null)
            {
                _assetNamesQueue.Add(_assetNames[i]);
                _thresholdsQueue.Add(thresholds[i]);
            }
            else
            {
                _currentAsset = _assetNames[i];
                _currentThreshold = thresholds[i];
            }
            _assetNames.RemoveAt(i);
            thresholds.RemoveAt(i);
            inkJSONAssets.RemoveAt(i);
        }
    }

    private void RemoveCurrent()
    {
        if (_assetNamesQueue.Count == 0)
        {
            _currentAsset = null;
        }
        else
        {
            _currentAsset = _assetNamesQueue.First();
            _assetNamesQueue.RemoveAt(0);
        }
         
        if (_thresholdsQueue.Count == 0)
        {
            _currentThreshold = BFN.No;
        }
        else
        {
            _currentThreshold = _thresholdsQueue.First();
            _thresholdsQueue.RemoveAt(0);
        }
        
    }

    #region Updates
    
    void ActivateMainButton()
    {
        MainCanvasManager.Instance.ShowMainButton();
        introductionDone = true;
        DialogueManager.onDialogueEnd -= ActivateMainButton;
    }

    void Empty()
    {
        
    } 
    
    #endregion

    public void LoadData(GameData gameData)
    {
        if (gameData.inkJSONNames.Count > 0)
        {
            inkJSONAssets = new List<TextAsset>();
            _assetNames = new List<string>();
            foreach (var jsonName in gameData.inkJSONNames)
            {
                var guid = Resources.Load<TextAsset>(Path.Combine(folderPath, jsonName));
                if (guid != null)
                {
                    _assetNames.Add(guid.name);
                    inkJSONAssets.Add(guid);
                }
            } 
        }

        if (gameData.inkJSONThresholds.Count > 0)
        {
            thresholds = gameData.inkJSONThresholds;
        }

        if (gameData.mainButtonShown)
        {
            ActivateMainButton();
        }
    }

    public void SaveData(ref GameData gameData)
    {
        if (_assetNames.Count == 0 && _currentAsset == null)
        {
            gameData.inkJSONNames = new List<string>();
            gameData.inkJSONThresholds = new List<BFN>();
        }
        else
        {
            var assetNames = new List<string>();
            if (_currentAsset != null)
            {
                assetNames.Add(_currentAsset);
            }

            if (_assetNamesQueue.Count > 0)
            {
                assetNames.AddRange(_assetNamesQueue);
            }

            if (_assetNames.Count > 0)
            {
                assetNames.AddRange(_assetNames); 
            }
            gameData.inkJSONNames = assetNames;
            
            var thres = new List<BFN>();
            if (_currentThreshold != BFN.No)
            {
                thres.Add(_currentThreshold);
            }

            if (_thresholdsQueue.Count > 0)
            {
                thres.AddRange(_thresholdsQueue);
            }

            if (thresholds.Count > 0)
            {
                thres.AddRange(thresholds); 
            }
            gameData.inkJSONThresholds = thres;
        }

        gameData.mainButtonShown = introductionDone;
    }
}
