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

    private void Start()
    {
        _currentActivePanel = _buttons;
    }

    public void Play() 
    {
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
        SoundManager.Instance.Play2DSound(CLICK_BUTTON_SOUND);
        backgroundAnimator.Play("Start", 0);
        buttonAnimator.Play("ButtonsFadeOut", 0);
        yield return new WaitForFixedUpdate();
        SoundManager.Instance.Play2DSound(INTRO_SOUND);
        AnimatorClipInfo[] currentClipInfo = backgroundAnimator.GetCurrentAnimatorClipInfo(0);
        yield return new WaitForSeconds(currentClipInfo[0].clip.length);
        yield return new WaitForSeconds(2.5f);
        GameManager.Instance.sceneLoader.SetScene("WorkspaceScene");
    }
}
