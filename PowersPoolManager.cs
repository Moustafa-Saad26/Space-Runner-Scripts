using System.Collections.Generic;
using UnityEngine;

public class PowersPoolManager : PoolManager<PowerUp>
{
   public static PowersPoolManager Instance {private set; get;}

   private int _totalSpawnWeight;
   private void Awake()
   {
      if (Instance != null && Instance != this)
      {
         Destroy(gameObject);
         return;
      }
      Instance = this;
      _poolTransform = transform;
      _objectsList = new List<PowerUp>();
   }
   
   protected override void Start()
   {
      CalculateTotalSpawnWeight();
      base.Start();
      
   }
   
   protected override void CreateObject()
   {
      PowerUp powerUp = Instantiate(GetRandomPowerUp(), _poolTransform);
      powerUp.transform.localPosition = Vector3.zero;
      powerUp.transform.localRotation = Quaternion.identity;
      powerUp.gameObject.SetActive(false);
      _objectsList.Add(powerUp);
   }
   
   private void CalculateTotalSpawnWeight()
   {
      _totalSpawnWeight = 0;
      foreach (PowerUp powerUp in objectPrefabs)
      {
         _totalSpawnWeight += powerUp.GetSpawnWeight();
      }
   }
   
   private PowerUp GetRandomPowerUp()
   {
      int randomNumber = Random.Range(0, _totalSpawnWeight);
      int currentWeight = 0;
      foreach (PowerUp powerUpToBe in objectPrefabs)
      {
         currentWeight += powerUpToBe.GetSpawnWeight();
         if (randomNumber < currentWeight) 
            return powerUpToBe;
      }
      Debug.LogError("Fallback PowerUp Reached");
      return objectPrefabs[0];
   }
}
