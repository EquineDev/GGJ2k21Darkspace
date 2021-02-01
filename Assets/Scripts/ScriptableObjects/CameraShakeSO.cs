using UnityEngine;

[CreateAssetMenu(fileName = "CamerShakeConfig", menuName = "ScriptableObjects/CamerShakeConfig", order = 2)]
public class CameraShakeSO : ScriptableObject
{
    [SerializeField]
    public CameraShakeData CameraShakeSettings;

}
[System.Serializable]
public struct CameraShakeData
{
    public float Magitude;
    public float Roughness;
    public float TimeInFade;
    public float TimeOutfade;
}