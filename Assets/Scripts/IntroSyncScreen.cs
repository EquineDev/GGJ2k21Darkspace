using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class IntroSyncScreen : MonoBehaviour
{
    
    [SerializeField]
    private AudioClip _clip; 
    [SerializeField]
    private UnityEvent _fullySynced;
    [SerializeField]
    private float _minMaxRange = 12f, _amount = .45f, _length = 3f;
    private float _endTarget = 0.01f;
    [SerializeField]
    private ShaderEffect_Unsync  _unsync;
    #region UnityAPI
    void Awake()
    {
        Assert.IsNotNull(_unsync);
      

    }
    private void Start()
    {
        
        StartCoroutine(IntroSync(_length));
        GetComponent<AudioSource>().PlayOneShot(_clip);
    }
    #endregion

    IEnumerator IntroSync(float time)
    {
        float timeFinish = Time.time + time;
        bool direction = true;
        _unsync.speed = 12;

        while (timeFinish > Time.time)
        {
            if (direction)
            {
               // Debug.Log("right");
                _unsync.speed += _amount;
                if (_unsync.speed >= _minMaxRange)
                {
                    direction = false;
                    _unsync.speed = _minMaxRange;
                }

            }
            else
            {
              //  Debug.Log("left");
                _unsync.speed -= _amount;
                if (_unsync.speed <= -_minMaxRange)
                {
                    direction = true;
                    _unsync.speed = 0 - _minMaxRange;
                }
            }

            yield return null;
        }

        timeFinish = Time.time + 0.5f;
        while (timeFinish > Time.time)
        {
            if (direction)
            {
              //  Debug.Log("right");
                _unsync.speed += _amount/5;
                if (_unsync.speed >= _endTarget)
                {

                    _unsync.speed = _endTarget;
                    break;
                }

            }
            else
            {
              //  Debug.Log("left");
                _unsync.speed -= _amount/5;
                if (_unsync.speed <= _endTarget)
                {
                    _unsync.speed = _endTarget;
                    break;
                }
            }
        }
        
        _fullySynced.Invoke();
    }
}
