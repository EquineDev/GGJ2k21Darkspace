using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
public class EndingDeath : MonoBehaviour
{
    [SerializeField]
    private AudioClip _clip;
    [SerializeField]
    private UnityEvent _deathFinished;
    [SerializeField]
    private float _minMaxRange = 5f, _amount = .2f;
    [SerializeField]
    private ShaderEffect_CorruptedVram _corruptedVram;

    #region UnityAPI
    void Awake()
    {
        Assert.IsNotNull(_corruptedVram);
        _corruptedVram.enabled = false;
    }
    #endregion

    public void StartDeath(float time)
    {
        _corruptedVram.enabled = true;
        StartCoroutine(ScreenDeath(time));
        for (int i = 0; i < 3; i++)
            this.GetComponent<AudioSource>().PlayOneShot(_clip);
    }

    IEnumerator ScreenDeath(float time)
    {
        float timeFinish = Time.time + time;
        bool direction = true;
        _corruptedVram.shift = 0;

        while (timeFinish > Time.time)
        {
            if (direction)
            {
               // Debug.Log("right");
                _corruptedVram.shift += _amount;
                if (_corruptedVram.shift >= _minMaxRange)
                {
                    direction = false;
                    _corruptedVram.shift = _minMaxRange;
                }

            }
            else
            {
               // Debug.Log("left");
                _corruptedVram.shift -= _amount;
                if (_corruptedVram.shift <= - _minMaxRange)
                {
                    direction = true;
                    _corruptedVram.shift = 0 - _minMaxRange;
                }
            }
             
            yield return null;
        }
        _corruptedVram.shift = 13;
        _deathFinished.Invoke();
    }
}