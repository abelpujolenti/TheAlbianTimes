using Managers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance => _instance;
    private AudioSource oneShotAudioSource;

    [SerializeField] private AudioClip changeBiasSound;
    [SerializeField] private AudioClip dropPaperSound;
    [SerializeField] private AudioClip grabPaperSound;
    [SerializeField] private AudioClip submitPaperSound;
    [SerializeField] private AudioClip dropPieceSound;
    [SerializeField] private AudioClip frictionSound;
    [SerializeField] private AudioClip thudSound;
    [SerializeField] private AudioClip snapPieceSound;
    [SerializeField] private AudioClip grabPieceSound;
    [SerializeField] private AudioClip openDrawerSound;
    [SerializeField] private AudioClip closeDrawerSound;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(_instance);
        }
        oneShotAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
    }
    private void OnDestroy()
    {
    }

    public void ChangeBiasSound()
    {
        oneShotAudioSource.PlayOneShot(changeBiasSound, .6f);
    }
    public void SubmitPaperSound()
    {
        oneShotAudioSource.PlayOneShot(grabPaperSound, .3f);
    }
    public void ReturnPaperSound()
    {
        oneShotAudioSource.PlayOneShot(dropPaperSound, .4f);
        oneShotAudioSource.PlayOneShot(thudSound, .3f);
    }
    public void DropPaperSound()
    {
        oneShotAudioSource.PlayOneShot(dropPaperSound, .2f);
    }
    public void GrabPaperSound()
    {
        oneShotAudioSource.PlayOneShot(dropPaperSound, .1f);
        oneShotAudioSource.PlayOneShot(grabPaperSound, .6f);
    }
    public void DropPieceSound()
    {
        oneShotAudioSource.PlayOneShot(thudSound, .6f);
    }
    public void GrabPieceSound()
    {
        oneShotAudioSource.PlayOneShot(grabPieceSound, .4f);
        oneShotAudioSource.PlayOneShot(thudSound, .3f);
    }
    public void SnapPieceSound()
    {
        oneShotAudioSource.PlayOneShot(snapPieceSound, .7f);
        oneShotAudioSource.PlayOneShot(thudSound, .6f);
    }
    public void OpenDrawerSound()
    {
        oneShotAudioSource.PlayOneShot(openDrawerSound);
    }
    public void CloseDrawerSound()
    {
        oneShotAudioSource.PlayOneShot(closeDrawerSound);
    }
}
