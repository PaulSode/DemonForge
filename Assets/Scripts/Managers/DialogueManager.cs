using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private Animator portraitAnimator;
    [SerializeField] private Animator layoutAnimator;
    private Story _currentStory;
    public bool DialogueIsPlaying { get; private set; }
    private List<TextAsset> _textQueue;
    
    private const string SPEAKER_TAG = "speaker";
    private const string PORTRAIT_TAG = "portrait";
    private const string LAYOUT_TAG = "layout";

    private Coroutine _displayCoroutine;
    private bool _canContinue = false;

   
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private int audioFrequence = 5;
    [SerializeField] private float minPitch = 0.5f;
    [SerializeField] private float maxPitch = 2f;
    [SerializeField] private float textSpeed = 0.01f;
    private int _currentFrequenceProgress = 0;
    
    [Header("Visuals")]
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image continueIcon;
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private GameObject portrait;
    [SerializeField] private GameObject speaker;

    [Header("Transition")]
    [SerializeField] private bool isTransition = false;
    [SerializeField] private TextAsset transitionTextAsset;

    [Header("Choices")] 
    [SerializeField] private GameObject[] choices;

    private TextMeshProUGUI[] _choicesTexts;
    private List<Choice> _currentChoises;
    
    public static event Action onDialogueEnd;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DialogueIsPlaying = false;
        _textQueue = new List<TextAsset>();
    }

    private void Start()
    {
        if (isTransition)
        {
            EnterDialogue(transitionTextAsset);
            return;
        }

        _choicesTexts = new TextMeshProUGUI[choices.Length];
        var index = 0;
        foreach (var choice in choices)
        {
            _choicesTexts[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

    }

    public void EnterDialogue(TextAsset inkJSON)
    {
        if (!DialogueIsPlaying)
        {
            _currentStory = new Story(inkJSON.text);
            DialogueIsPlaying = true;
            continueIcon.gameObject.SetActive(true);
            portrait.SetActive(true);
            speaker.SetActive(true);
            ContinueStory();
        }
        else
        {
            _textQueue.Add(inkJSON);
        }
    }

    private IEnumerator PopQueue()
    {
        yield return new WaitForSeconds(5);
        _currentStory = new Story(_textQueue.First().text);
        _textQueue.Remove(_textQueue.First());
        continueIcon.gameObject.SetActive(true);
        portrait.SetActive(true);
        speaker.SetActive(true);
        ContinueStory();
    }

    private void ContinueStory()
    {
        if (_currentStory.canContinue)
        {
            if (_displayCoroutine != null)
            {
                StopCoroutine(_displayCoroutine);
            }
            _displayCoroutine = StartCoroutine(DisplayLine(_currentStory.Continue()));
            HandleTags(_currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        _canContinue = false;
        continueIcon.gameObject.SetActive(false);
        
        dialogueText.text = "";
        foreach (var character in line)
        {
            dialogueText.text += character;
            if (_currentFrequenceProgress > audioFrequence)
            {
                _currentFrequenceProgress = 0;
                audioSource.pitch = Random.Range(minPitch, maxPitch);
                audioSource.Play();
            }
            else
            {
                _currentFrequenceProgress++;
            }
            yield return new WaitForSeconds(textSpeed);
        }

        DisplayChoices();
        _canContinue = true;
        continueIcon.gameObject.SetActive(true);
    }

    private void ExitDialogueMode()
    {
        if (isTransition)
        {
            isTransition = false;
            SceneManager.LoadScene("GameScene");
        }
        dialogueText.text = "";

        continueIcon.gameObject.SetActive(false);
        portrait.SetActive(false);
        speaker.SetActive(false);

        if (_textQueue.Count > 0)
        {
            StartCoroutine(PopQueue());
        }
        else
        {
            DialogueIsPlaying = false;
        }
        onDialogueEnd?.Invoke();
    }

    private void HandleTags(List<string> tags)
    {
        foreach (var tag in tags)
        {
            var splitTag = tag.Split(":");
            if (splitTag.Length != 2)
            {
                Debug.Log("Tag is not properly parsed : " + tag);
            }

            var key = splitTag[0].Trim();
            var value = splitTag[1].Trim();

            switch (key)
            {
                case SPEAKER_TAG:
                    speakerText.text = value;
                    break;
                case PORTRAIT_TAG:
                    portraitAnimator.Play(value);
                    break;
                case LAYOUT_TAG:
                    layoutAnimator.Play(value);
                    break;
                default:
                    Debug.Log("Unknown tag used : " + key);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
       _currentChoises = _currentStory.currentChoices;
        if (_currentChoises.Count > choices.Length)
        {
            Debug.LogError("More choices than UI can support");
        }

        var index = 0;

        foreach (var choice in _currentChoises)
        {
           choices[index].gameObject.SetActive(true);
           _choicesTexts[index].text = choice.text;
           index++;
        }

        for (var i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
    }

    public void MakeChoice(int index)
    {
        for (var i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
        _currentStory.ChooseChoiceIndex(index);
        ContinueStory();
    }

    public void PointerClick()
    {
        if (_canContinue && _currentChoises.Count == 0)
        {
            ContinueStory();
        }
    }
}
