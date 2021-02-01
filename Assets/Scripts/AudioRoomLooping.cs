using System.Collections;
using UnityEngine;


public class AudioRoomLooping : MonoBehaviour
{
    private AudioSource _audioSourceA, _audioSourceB;
    private AudioClip _musicClipA = null, _musicClipB = null;
    protected Coroutine _fadeA = null, _fadeB = null;

    public bool IsCrossFadeHappening
    {
        get
        {
            if (_fadeA != null || _fadeB != null)
                return false;
            return true;
        }
    }
    public bool IsClipPlaying
    {
        get
        {
            if (_audioSourceA.isPlaying || _audioSourceB.isPlaying)
                return true;
            return false;
        }
    }

    #region UnityAPI

    private void Awake()
    {
        CheckForAudioSetup(ref _audioSourceA);
        CheckForAudioSetup(ref _audioSourceB);
        _audioSourceB.loop = true;
        _audioSourceB.playOnAwake = false;

    }
    #endregion

    #region public 
    public void PlayMusicLooping(ref AudioClip clip)
    {
        SwapPlaybackAudioSource(ref clip);
        HaveMusicLoop();
    }

    public void PlayMusicOnce(ref AudioClip clip)
    {
        SwapPlaybackAudioSource(ref clip);
        StopMusicLooping();
    }

    public bool StartCrossFade(ref AudioClip clip, float maxVolume, float fadingTime, float delayUnitFade = 0)
    {

         if (_fadeA != null || _fadeB != null)
            return false;
        SetupFade(clip, maxVolume, fadingTime, delayUnitFade);

        return true;
    }

    public void FadeMusic(float maxVolume, float fadingTime, float delayUnitFade = 0)
    {
        if (_audioSourceA.isPlaying)
            _fadeA = StartCoroutine(CrossFade(_audioSourceA, _audioSourceA.volume, maxVolume, fadingTime, delayUnitFade));
        else if (_audioSourceB.isPlaying)
            _fadeB = StartCoroutine(CrossFade(_audioSourceB, _audioSourceB.volume, maxVolume, fadingTime, delayUnitFade));
    }


    public void HaveMusicLoop()
    {
        if (_audioSourceA.isPlaying)
            _audioSourceA.loop = true;
        else if (_audioSourceB.isPlaying)
            _audioSourceB.loop = true;
    }

    public void StopMusicLooping()
    {
        if (_audioSourceA.isPlaying)
            _audioSourceA.loop = false;
        else if (_audioSourceB.isPlaying)
            _audioSourceB.loop = false;
    }

    public void StopMusicPlaying()
    {
        if (_audioSourceA.isPlaying)
            _audioSourceA.Stop();
        else if (_audioSourceB.isPlaying)
            _audioSourceB.Stop();
    }


    public void StopFade()
    {
        if (_fadeA != null)
            StopCoroutine(_fadeA);
        if (_fadeB != null)
            StopCoroutine(_fadeB);
    }
    #endregion

    #region Private

    private void SwapPlaybackAudioSource(ref AudioClip clip)
    {
        if (_audioSourceA.isPlaying)
        {
            _audioSourceA.Stop();
            _musicClipA = clip;
            _audioSourceB.clip = _musicClipA;
            _audioSourceB.volume = 0.3f;
            _audioSourceB.Play();

        }
        else
        {
            _audioSourceB.Stop();
            _musicClipB = clip;
            _audioSourceA.clip = _musicClipB;
            _audioSourceA.volume = 0.3f;
            _audioSourceA.Play();
        }
    }
    private void StopAudioA()
    {

        _audioSourceA.Stop();

    }
    private void StopAudioB()
    {

        _audioSourceB.Stop();

    }


    private void SetupFade(AudioClip clip, float maxVolume, float fadingTime, float delay)
    {
        if (_audioSourceA.isPlaying)
        {

            _audioSourceB.volume = 0f;
            _musicClipB = clip;
            _audioSourceB.clip = clip;
            _audioSourceB.Play();
            Debug.Log("Source B playing");
            _fadeB = StartCoroutine(CrossFade(_audioSourceB, _audioSourceB.volume, maxVolume, fadingTime, delay));
            _fadeA = StartCoroutine(CrossFade(_audioSourceA, _audioSourceA.volume, 0f, fadingTime, delay));
            Invoke("StopAudioA", fadingTime);
        }
        else
        {

            _audioSourceA.volume = 0f;
            _musicClipA = clip;
            _audioSourceA.clip = clip;
            _audioSourceA.Play();
            Debug.Log("Source A playing");
            _fadeB = StartCoroutine(CrossFade(_audioSourceA, _audioSourceA.volume, maxVolume, fadingTime, delay));
            _fadeA = StartCoroutine(CrossFade(_audioSourceB, _audioSourceB.volume, 0f, fadingTime, delay));
            Invoke("StopAudioB", fadingTime);

        }

    }


    private void CheckForAudioSetup(ref AudioSource source)
    {
        if (source != null)
            return;

        source = this.gameObject.AddComponent<AudioSource>();

        if (Application.isEditor)
            Debug.Log(source + " Didn't have an audio source assigned so audio source was created ");
    }

    IEnumerator CrossFade(AudioSource sourceToFade, float beginVolume, float endVolume, float duration, float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        float startTime = Time.time;

        while (true)
        {
            float elapsed = Time.time - startTime;
            sourceToFade.volume = Mathf.Clamp01(Mathf.Lerp(beginVolume, endVolume, elapsed / duration));
            if (sourceToFade.volume == endVolume)
            {
                StopFade();
                break;
            }

            yield return null;
        }
    }
    #endregion
}
