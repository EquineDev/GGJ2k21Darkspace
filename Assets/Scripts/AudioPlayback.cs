using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayback : MonoBehaviour
{
    private AudioSource _asVoiceOver, _asSoundEffect, _asAmbientEffect, _asOtherEffect;
    private AudioSource _asVoiceOver2, _asSoundEffect2, _asAmbientEffect2, _asOtherEffect2;
    private IEnumerator _coroutineVO, _coroutineSE, _coroutineAE, _coroutineOE;
    private Queue<AudioClip> _voCLips = new Queue<AudioClip>(), _seClips = new Queue<AudioClip>(), _aeClips = new Queue<AudioClip>(), _oeClips = new Queue<AudioClip>();
    #region UnityAPI

    private void Awake()
    {
        _asOtherEffect = this.GetComponent<AudioSource>();
    }
    #endregion

    #region Public
    
    public void EnqueueQueue(ref AudioClip clip, AudioType type)
    {

        switch (type)
        {
            case AudioType.SoundEffect:
                _seClips.Enqueue(clip);
                break;

            case AudioType.VoiceOver:
                _voCLips.Enqueue(clip);
                break;

            case AudioType.Ambient:
                _aeClips.Enqueue(clip);
                break;

            case AudioType.Other:
                _oeClips.Enqueue(clip);
                break;
        }

    }
    public void PlayAudioOnce(ref AudioClip clip, AudioType type)
    {
        switch (type)
        {
            case AudioType.SoundEffect:
                CheckForAudioSetup(ref _asSoundEffect);
                _asSoundEffect.PlayOneShot(clip);
                break;

            case AudioType.VoiceOver:
                CheckForAudioSetup(ref _asVoiceOver);
                _asVoiceOver.PlayOneShot(clip);
                break;

            case AudioType.Ambient:
                CheckForAudioSetup(ref _asAmbientEffect);
                _asAmbientEffect.PlayOneShot(clip);
                break;

            case AudioType.Other:
                CheckForAudioSetup(ref _asOtherEffect);
                _asOtherEffect.PlayOneShot(clip);
                break;

        }
    }

    public void PlayAudioQueue(AudioType type)
    {
        switch (type)
        {
            case AudioType.SoundEffect:
                if (_coroutineSE == null)
                    return;
                _coroutineSE = AudioQueuePlayback(_seClips, _asSoundEffect, _asSoundEffect2, _coroutineSE);
                break;

            case AudioType.VoiceOver:
                if (_coroutineVO == null)
                    return;
                _coroutineVO = AudioQueuePlayback(_voCLips, _asVoiceOver, _asVoiceOver2, _coroutineVO);
                break;

            case AudioType.Ambient:
                if (_coroutineAE == null)
                    return;
                _coroutineAE = AudioQueuePlayback(_aeClips, _asAmbientEffect, _asAmbientEffect2, _coroutineAE);
                break;

            case AudioType.Other:
                if (_coroutineOE == null)
                    return;
                _coroutineOE = AudioQueuePlayback(_oeClips, _asOtherEffect, _asOtherEffect2, _coroutineOE);
                break;
        }

    }

    public void StopAudio(AudioType type)
    {
        switch (type)
        {
            case AudioType.SoundEffect:
                _asSoundEffect?.Stop();
                _asSoundEffect2?.Stop();
                break;

            case AudioType.VoiceOver:
                _asVoiceOver?.Stop();
                _asVoiceOver2?.Stop();
                break;

            case AudioType.Ambient:
                _asAmbientEffect?.Stop();
                _asAmbientEffect?.Stop();
                break;

            case AudioType.Other:
                _asOtherEffect?.Stop();
                _asOtherEffect2?.Stop();
                break;
        }
    }

    public void ClearAudioClipQueue(AudioType type)
    {
        switch (type)
        {
            case AudioType.SoundEffect:
                _seClips?.Clear();
                break;

            case AudioType.VoiceOver:
                _voCLips?.Clear();
                break;

            case AudioType.Ambient:
                _aeClips?.Clear();
                break;

            case AudioType.Other:
                _oeClips?.Clear();
                break;
        }
    }

    #endregion

    #region Private
    
    private void CheckForAudioSetup(ref AudioSource source)
    {
        if (source != null)
            return;

        source = this.gameObject.AddComponent<AudioSource>();

        if (Application.isEditor)
            Debug.Log(source + " Didn't have an audio source assigned so audio source was created ");
    }

    IEnumerator AudioQueuePlayback(Queue<AudioClip> clipQueue, AudioSource source1, AudioSource source2, IEnumerator type)
    {
        CheckForAudioSetup(ref source1);
        CheckForAudioSetup(ref source2);

        double Clipduration = 0.2f;
        bool queueFinished = false;

        while (!queueFinished)
        {

            //setup audio source1 to be played
            source1.clip = clipQueue.Dequeue();
            source1.PlayScheduled(AudioSettings.dspTime + Clipduration);
            if (clipQueue.Count < 1)
                queueFinished = true;
        
            else
            {
                Clipduration = (double)source1.clip.samples / source1.clip.frequency;
                source2.clip = clipQueue.Dequeue();
                source2.PlayScheduled(AudioSettings.dspTime + Clipduration);

                if (clipQueue.Count < 1)
                    queueFinished = true;
                else
                    Clipduration = (double)source2.clip.samples / source2.clip.frequency;
               
                   
                
            }

            while (source1.isPlaying) //wait for source 1 to finish playing 
                yield return null;
            
            if(!queueFinished)
                source1.PlayScheduled(AudioSettings.dspTime + Clipduration);

            while (source2.isPlaying) //wait for source 2 to finish playing 
                yield return null;

        }

        type = null;

    }
    #endregion

}

public enum AudioType
{
    SoundEffect,
    VoiceOver,
    Ambient,
    Other
};