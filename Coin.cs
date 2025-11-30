using System;
using UnityEngine;

public class Coin : Item
{
    //Public Functions//
    // CollectCoin
    // DisableCollider
    // EnableCollider
    // GetCoinValue
    // GetSpawnWeight
    // GetCoinName
    

    [Header("Coin Configuration")]
    [SerializeField] private int coinValue = 1;
    
    

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
       
    }
    private void Start()
    {
        
    }

   

    public override void CollectItem()
    {
        
        foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = false;
        }
        DisableCollider(); // So the player doesn't collect the item twice
        CoinPoolManager.Instance.ReturnObjectAfter(this, 2f);
    }

   
    

    

   

    public int GetCoinValue()
    {
        if (coinValue <= 0)
        {
            Debug.LogError("Coin Value is Zero or Negative");
            return 1; // Default Value
        }
        return coinValue;
    }
    
    
    
}
