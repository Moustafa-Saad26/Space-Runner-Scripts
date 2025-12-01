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
    
    private bool _isCollected;
    
    [SerializeField] private Vector3 newPosition = new Vector3(0, 2, 0);
    [SerializeField] private float collectSpeed = 2f;
    [SerializeField] private float returnToPoolAfter = 2f;

    private float _timer;

    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
       
    }


    private void Update()
    {
        if (_isCollected)
        {
            _timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, transform.position + newPosition, Time.deltaTime * collectSpeed);
            if (_timer >= returnToPoolAfter)
            {
                _timer = 0;
                _isCollected = false;
            }
        }
    }


    public override void CollectItem()
    {
        
        /*foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = false;
        }*/
        DisableCollider(); // So the player doesn't collect the item twice
        _isCollected = true;
        CoinPoolManager.Instance.ReturnObjectAfter(this, returnToPoolAfter + 0.5f);
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
