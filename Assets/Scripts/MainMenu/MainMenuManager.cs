using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    [FormerlySerializedAs("_animator")] [SerializeField] private Animator animator;
    [FormerlySerializedAs("_fg")] [SerializeField] private Image fg;
    [FormerlySerializedAs("_as")] [SerializeField] private AudioSource audioSource;
    
    public void StartGame()
    {
        StartCoroutine(TransitionCoroutine(false));
        StartCoroutine(MusicFade(1f, 0));
    }

    private IEnumerator TransitionCoroutine(bool quit)
    {
        animator.SetBool("Fade", true);
        yield return new WaitUntil( () => fg.color.a == 1);
        if (quit)
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene("TransitionScene");
        }
    }
    
    private IEnumerator MusicFade(float duration, float targetVolume)
    {
        float currentTime = 0;
        var start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
    }

    public void Options()
    {
        throw new NotImplementedException();
        SceneManager.LoadScene("OptionsMenu");
    }

    public void Achievements()
    {
        throw new NotImplementedException();
        SceneManager.LoadScene("AchievementsMenu");
    }

    public void Quit()
    {
        StartCoroutine(TransitionCoroutine(true));
    }
}
