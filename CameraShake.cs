using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
   private CinemachineBasicMultiChannelPerlin _noiseComponent;

   private void Start()
   {
      _noiseComponent = GetComponent<CinemachineBasicMultiChannelPerlin>();
      
      if (Player.Instance != null)
      {
         Player.Instance.OnGameOver += Player_OnGameOver;
      }
   }

   private void Player_OnGameOver(object sender, EventArgs e)
   {
      StartCoroutine(Shake(1f));
   }

   private System.Collections.IEnumerator Shake(float seconds)
   {
      _noiseComponent.AmplitudeGain = 3f;
      _noiseComponent.FrequencyGain = 10f;
      yield return new WaitForSeconds(seconds);
      _noiseComponent.AmplitudeGain = 1f;
      _noiseComponent.FrequencyGain = 1f;
   }
}
