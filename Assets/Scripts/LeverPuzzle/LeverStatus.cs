using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
namespace Puzzle.Lever
{
    public class LeverStatus : MonoBehaviour
    {
        [SerializeField]
        private AudioPlayback _playback;
        [Header("VO postion")]
        [SerializeField]
        private AudioClip _acLow, _acMid, _acHigh;
        [Header("VO LeverLocation")]
        [SerializeField]
        private AudioClip _acLeft, _acMiddle, _acRight;
        private LeverPosition[] _levers = new LeverPosition[] { LeverPosition.mid, LeverPosition.mid, LeverPosition.mid };

        private void Awake()
        {
            Assert.IsNotNull(_playback);
        }

        public void GetLeverStatus(LeverLocation LeverLocation)
        {
            switch (LeverLocation)
            {
                case LeverLocation.Left:
                    _playback.EnqueueQueue(ref _acLeft, AudioType.VoiceOver);
                    break;
                case LeverLocation.Middle:
                    _playback.EnqueueQueue(ref _acMiddle, AudioType.VoiceOver);
                    break;
                case LeverLocation.Right:
                    _playback.EnqueueQueue(ref _acRight, AudioType.VoiceOver);
                    break;
            }


            switch (_levers[(int)LeverLocation])
            {
                case LeverPosition.low:
                    _playback.EnqueueQueue(ref _acLow, AudioType.VoiceOver);
                    break;
                case LeverPosition.mid:
                    _playback.EnqueueQueue(ref _acMid, AudioType.VoiceOver);
                    break;
                case LeverPosition.high:
                    _playback.EnqueueQueue(ref _acHigh, AudioType.VoiceOver);
                    break;
            }

            _playback.PlayAudioQueue(AudioType.VoiceOver);
        }

    }

    
    public enum LeverPosition
    {
        low,
        mid,
        high,
    }

    public enum LeverLocation
    {
        Left ,
        Middle,
        Right,
    }
}

