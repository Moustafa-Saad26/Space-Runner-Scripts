using System.Collections.Generic;
using UnityEngine;

public class CoinPoolManager : PoolManager<Coin>
{
    public static CoinPoolManager Instance {private set; get;}
    
    private int _totalSpawnWeight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _objectsList = new List<Coin>();
    }
    
    protected override void Start()
    {
        CalculateTotalSpawnWeight();
        base.Start();
        
    }
    
    private void CalculateTotalSpawnWeight()
    {
        _totalSpawnWeight = 0;
        foreach (Coin coin in objectPrefabs)
        {
            _totalSpawnWeight += coin.GetSpawnWeight();
        }
    }
    
    private Coin GetRandomCoin()
    {
        int randomNumber = Random.Range(0, _totalSpawnWeight);
        int currentWeight = 0;
        foreach (Coin coinToBe in objectPrefabs)
        {
            currentWeight += coinToBe.GetSpawnWeight();
            if (randomNumber < currentWeight) 
                return coinToBe;
        }
        Debug.LogError("Fallback Coin Reached");
        return objectPrefabs[0];
    }

    protected override void CreateObject()
    {
        Coin coin = Instantiate(GetRandomCoin(), _poolTransform);
        coin.transform.localPosition = Vector3.zero;
        coin.transform.localRotation = Quaternion.identity;
        coin.gameObject.SetActive(false);
        _objectsList.Add(coin);
    }
    

   
}
