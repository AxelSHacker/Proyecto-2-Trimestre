using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShakeCameraDataSO", menuName = "TopDownShooter/new ShakeCameraDataSO")]
public class ShakeCameraDataSO : ScriptableObject
{
    //Tipo de ruido a aplicar a la cámara
    public NoiseSettings noiseSettings;
    //Curva para definir la amplitzud a lo largo del tiempo
    public AnimationCurve amplitudCurve;
    //Duración del shake
    public AnimationCurve frecuencyCurve;
    //Duración total del shake
    [Range(0, 4)]
    public float shakeDuration = 1f;
}
