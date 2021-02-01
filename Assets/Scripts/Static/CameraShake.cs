using UnityEngine;
using System.Collections;

public static class CameraShake 
{

    private static bool _holdShake;
    private static float _fadeTimeCurrent = 0f, _tickAmount = 0f;
    private static CameraShakeData _data = new CameraShakeData(), _settings = new CameraShakeData();
    private static Vector3 _shakeAmount, _influencePosition, _influenceRotation;
    private static Vector3 _defaultPosInfluence = new Vector3(0.01f, 0.01f, 0.00f);
    private static Vector3 _deaultRotInfluence = new Vector3(0.00f, 0.00f, 0.0f);
    private static GameObject _shakerCamera = null;

    #region public
    public static bool CreateShakeSetup(CameraShakeSO Setting, GameObject Object)
    {
        if (_shakerCamera != null)
            return false;
        //setup for shake by zering out vectors
        _shakerCamera = Object;
        _shakeAmount = Vector3.zero;

        _data = Setting.CameraShakeSettings;
        _settings = _data;

        CheckFadeInOrOut();

        _influencePosition = _defaultPosInfluence;
        _influenceRotation = _deaultRotInfluence;

        _tickAmount = Random.Range(-100, 100);

        return true;
    }

    public static bool PositionMovmentInfluence(Vector3 Amount)
    {
        if (_shakerCamera == null)
            return false;
        _influencePosition = Amount;
        return true;
    }

    public static bool RotationMovmentInfluence(Vector3 Amount)
    {
        if (_shakerCamera == null)
            return false;
        _influenceRotation = Amount;
        return true;
    }
    #endregion

    #region private
    private static Vector3 UpdateShake()
    {

        if (_data.TimeInFade > 0 && _holdShake)
        {
            if (_fadeTimeCurrent < 1)
                _fadeTimeCurrent += Time.deltaTime / _data.TimeInFade;
            else if (_data.TimeOutfade > 0)
                _holdShake = false;
        }

        if (!_holdShake)
            _fadeTimeCurrent -= Time.deltaTime / _data.TimeOutfade;

        if (_holdShake)
            _tickAmount += Time.deltaTime * _data.Roughness;
        else
            _tickAmount += Time.deltaTime * _data.Roughness;


        return ShakeAmount() * _data.Magitude * _fadeTimeCurrent;
    }

    private static Vector3 ShakeAmount()
    {

        _shakeAmount.x = Mathf.PerlinNoise(_tickAmount, 0) - 0.5f;
        _shakeAmount.y = Mathf.PerlinNoise(0, _tickAmount) - 0.5f;
        _shakeAmount.z = Mathf.PerlinNoise(_tickAmount, _tickAmount) - 0.5f;

        return _shakeAmount;
    }

    private static Vector3 VectorMulitplyBySecondVector(Vector3 source, Vector3 second)
    {
        source.x *= second.x;
        source.y *= second.y;
        source.z *= second.z;

        return source;
    }
    private static bool IsDoneShaking()
    {
        if (_fadeTimeCurrent > 0 || _holdShake)
            return true;
        else if (!_holdShake && _fadeTimeCurrent > 0)
            return true;
        else if (_fadeTimeCurrent < 1 && _holdShake && _data.TimeInFade > 0)
            return true;
        else
            return false;

    }

    private static void Reset()
    {
        _data = _settings;
        CheckFadeInOrOut();
    }

    private static void CheckFadeInOrOut()
    {
        if (_data.TimeInFade > 0)
        {
            _holdShake = true;
            _fadeTimeCurrent = 0;
        }
        else
        {
            _holdShake = false;
            _fadeTimeCurrent = 1;
        }
    }



    #endregion

    public static IEnumerator CameraShaker()
    {
        if (_shakerCamera == null)
            yield break;
        Debug.Log("Start shaking");

        Vector3 postion = Vector3.zero;
        Vector3 rotation = Vector3.zero;
        Vector3 camPos = _shakerCamera.transform.position;
        Quaternion camRot = _shakerCamera.transform.rotation;

        while (IsDoneShaking())
        {

            postion = VectorMulitplyBySecondVector(UpdateShake(), _influencePosition);
            rotation = VectorMulitplyBySecondVector(UpdateShake(), _influenceRotation);
            _shakerCamera.transform.localPosition += postion;
            _shakerCamera.transform.localEulerAngles += rotation;
            yield return null;
        }
        while (Mathf.Abs(camPos.x + _shakerCamera.transform.position.x) <= 0.01f)
        {
            _shakerCamera.transform.rotation = Quaternion.Slerp(_shakerCamera.transform.rotation, camRot, 4.5f * Time.deltaTime);
            _shakerCamera.transform.position = Vector3.Slerp(_shakerCamera.transform.position, camPos, 4.5f * Time.deltaTime);

            yield return null;
        }

        _shakerCamera.transform.rotation = camRot;
        _shakerCamera.transform.position = camPos;
        Reset();
        Debug.Log("finished shaking");
    }

}
