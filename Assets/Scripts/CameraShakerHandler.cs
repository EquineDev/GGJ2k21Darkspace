using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
public class CameraShakerHandler : MonoBehaviour
{
    [SerializeField]
    private CameraShakeSO _cameraShakeConfig;
    #region UnityAPI
    private void Awake()
    {
        Assert.IsNotNull(_cameraShakeConfig);
    }
    #endregion
    public void  ShakeCamera()
    {
        CameraShake.CreateShakeSetup(_cameraShakeConfig, Camera.main.gameObject);
        StartCoroutine(CameraShake.CameraShaker());
    }
}
