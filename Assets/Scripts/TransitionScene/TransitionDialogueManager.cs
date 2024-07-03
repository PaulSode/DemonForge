using System.Collections;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TransitionDialogueManager : MonoBehaviour
{
    
    private Story _currentStory;
    

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
    [SerializeField] private TextAsset transitionTextAsset;
    

    private void Start()
    {
        EnterDialogue(transitionTextAsset);
    }

    public void EnterDialogue(TextAsset inkJSON)
    {
        _currentStory = new Story(inkJSON.text);
        ContinueStory();
        StartCoroutine(AutomaticContinue());
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
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        _canContinue = false;
        
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

        _canContinue = true;
    }

    private void ExitDialogueMode()
    {
        SceneManager.LoadScene("GameScene");
        dialogueText.text = "";
    }
    

    public void PointerClick()
    {
        if (_canContinue)
        {
            ContinueStory();
        }
    }

    private IEnumerator AutomaticContinue()
    {
        yield return new WaitForSeconds(5);
        PointerClick();
    }
}
