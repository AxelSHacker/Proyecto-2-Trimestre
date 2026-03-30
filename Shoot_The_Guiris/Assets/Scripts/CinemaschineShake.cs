
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;


public class CinemaschineShake : CustomMonoBehaviour
{
   private static CinemaschineShake _instance;
   public static CinemaschineShake Instance => _instance;

   [SerializeField] CinemachineCamera _cinemachineCamera;
   CinemachineBasicMultiChannelPerlin _cameraNoise;
   [SerializeField] ShakeCameraDataSO _defaultShakeCameraDataSO;
   NoiseSettings _originalNoiseSettings;
   float _originalAmplitd;
   float _originalFrecuency;
   Coroutine _shakeCoroutine;

   public override void EditorInit()
   {

   }
   void Awake()
   {
      if (_instance == null)
      {
         _instance = this;
      }
      else
      {
         Destroy(this);
      }
   }
   void Start()
   {
      _cameraNoise = _cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

   }

   void Update()
   {

   }
   public void StartShake(ShakeCameraDataSO shakeCameraDataSO = null)
   {
      if (shakeCameraDataSO == null) shakeCameraDataSO = _defaultShakeCameraDataSO;
      if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);

      _shakeCoroutine = StartCoroutine(Shake(shakeCameraDataSO));
   }

   private IEnumerator Shake(ShakeCameraDataSO shakeCameraDataSO)
   {
      float timer = shakeCameraDataSO.shakeDuration;

      _originalNoiseSettings = _cameraNoise.NoiseProfile;
      _originalAmplitd = _cameraNoise.AmplitudeGain;
      _originalFrecuency = _cameraNoise.FrequencyGain;

      _cameraNoise.NoiseProfile = shakeCameraDataSO.noiseSettings;
      while (timer > 0)
      {
         float t = timer / shakeCameraDataSO.shakeDuration;
         _cameraNoise.AmplitudeGain = shakeCameraDataSO.amplitudCurve.Evaluate(t);
         _cameraNoise.FrequencyGain = shakeCameraDataSO.frecuencyCurve.Evaluate(t);
         timer -= Time.deltaTime;
         yield return null;
      }
      _cameraNoise.NoiseProfile = _originalNoiseSettings;
      _cameraNoise.AmplitudeGain = _originalAmplitd;
      _cameraNoise.FrequencyGain = _originalFrecuency;
   }

}



