using System;
using UnityEngine;

public class SpeedManager : MonoBehaviour
{
   public static SpeedManager Instance {private set; get;}

   
   [SerializeField] private float changeSpeedEvery = 10f;
   [SerializeField] private float speedIncreaseAmount = 1.5f;
   [SerializeField] private float maxPlayerSpeed = 40f;
   [SerializeField] private float startingSpeed = 15f;
   
 
   
   private float _changeSpeedTimer;

   public event EventHandler<OnGameSpeedChangedEventArgs> OnGameSpeedChanged;
   public class OnGameSpeedChangedEventArgs : EventArgs
   {
      public float speedIncreaseAmount;
   }
   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }
      Instance = this;
   }
   
   private void Start()
   {
      
   }

   private void Update()
   {
      _changeSpeedTimer += Time.deltaTime;
      if (_changeSpeedTimer >= changeSpeedEvery){
         OnGameSpeedChanged?.Invoke(this, new OnGameSpeedChangedEventArgs
         {
            speedIncreaseAmount = speedIncreaseAmount,
         });
         _changeSpeedTimer = 0f;
      }
   }
   
   public float GetMaxPlayerSpeed()
   {
      return maxPlayerSpeed;
   }

   public float GetStartingSpeed()
   {
      return startingSpeed;
   }
   
   
}
