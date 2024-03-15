using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvas : MonoBehaviour
{
    private const string INTRO_SOUND = "Intro";
    private const string CLICK_BUTTON_SOUND = "Click Button";

    [SerializeField] private GameObject _buttons;
    [SerializeField] private GameObject _chapters;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _credits;

    [SerializeField] Animator backgroundAnimator;
    [SerializeField] Animator buttonAnimator;

    private GameObject _currentActivePanel;

    private Coroutine startGameCoroutine;

    private AudioSource _audioSourceIntro;
    private AudioSource _audioSourceChangeBias;

    private void Start()
    {
        _currentActivePanel = _buttons;
    }

    public void Play() 
    {
        _audioSourceIntro = gameObject.AddComponent<AudioSource>();
        _audioSourceChangeBias = gameObject.AddComponent<AudioSource>();
        (AudioSource, string)[] tuples =
        {
            (_audioSourceIntro, INTRO_SOUND),
            (_audioSourceChangeBias, CLICK_BUTTON_SOUND)
        };
        SoundManager.Instance.SetMultipleAudioSourcesComponents(tuples);
        if (startGameCoroutine != null) return;
        startGameCoroutine = StartCoroutine(StartGameCoroutine());
    }

    public void Chapters() 
    {
        ChangeCurrentActivePanel(_chapters);
    }

    public void Settings()
    {
        ChangeCurrentActivePanel(_settings);
    }

    public void Credits()
    {
        ChangeCurrentActivePanel(_credits);
    }

    public void Exit() 
    {
        Application.Quit();
    }

    private void ChangeCurrentActivePanel(GameObject panelToActive) 
    {
        _currentActivePanel.SetActive(false);
        panelToActive.SetActive(true);
        _currentActivePanel = panelToActive;
    }


    private IEnumerator StartGameCoroutine()
    {
        _audioSourceChangeBias.Play();
        backgroundAnimator.Play("Start", 0);
        buttonAnimator.Play("ButtonsFadeOut", 0);
        yield return new WaitForFixedUpdate();
        _audioSourceIntro.Play();
        AnimatorClipInfo[] currentClipInfo = backgroundAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        yield return new WaitForSeconds(2.5f);
        GameManager.Instance.sceneLoader.SetScene("WorkspaceScene");
    }
}
