using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{
    //Public Functions//
    // SpawnItems
    // ResetBlockForPool
    // GetNextBlockSpawnPoint
    
    [SerializeField] private Transform nextBlockSpawnPoint;
    [SerializeField] private List<Transform> itemsSpawnPoints;
    
    [SerializeField] private Transform coinsParent;
    [SerializeField] private Transform powerUpsParent;
    
    private const float _CoinSpawnChanceInPoint = 0.35f; //Can be made SerializeField if needed later
    private const float _PowerUpSpawnChanceInPoint = 0.35f;
    
    private bool _isPowerUpSpawned; // can be made SerializeField and converted to an int if needed later


    
    private void Start()
    {
        if(nextBlockSpawnPoint == null) Debug.LogError("Next Block Spawn Point is Null or Not Set in Block");
        if(itemsSpawnPoints == null || itemsSpawnPoints.Count == 0) Debug.LogError("Item Spawn Points List is Null or Empty in Block");
        if(coinsParent == null) Debug.LogError("Coins Parent is Null or Not Set in Block");
        if(powerUpsParent == null) Debug.LogError("PowerUps Parent is Null or Not Set in Block");
    }

    public void SpawnItems()
    {
        foreach (Transform itemSpawnPoint in itemsSpawnPoints)
        { 
            if(itemSpawnPoint == null) {Debug.LogError("Item Spawn Point is Null or Not Set in Block"); continue;}
            if(!itemSpawnPoint.gameObject.activeSelf) continue;
            if (!ChanceToSpawnCoinAt(itemSpawnPoint.position))
            {
                if (_isPowerUpSpawned) continue;
                if(ChanceToSpawnPowerUpAt(itemSpawnPoint.position))
                {
                    _isPowerUpSpawned = true;
                }
            }
        }
    }
    
   

    private void DeSpawnCoins()
    {
        if(coinsParent.childCount == 0) return;
        while(coinsParent.childCount > 0)
        {
            Coin coin = coinsParent.GetChild(0).GetComponent<Coin>();
            CoinPoolManager.Instance.ReturnObject(coin);
        }
    }

    private void DespawnPowerUps()
    {
        if(powerUpsParent.childCount == 0) return;
        while(powerUpsParent.childCount > 0)
        {
            PowerUp powerUp = powerUpsParent.GetChild(0).GetComponent<PowerUp>();
            PowersPoolManager.Instance.ReturnObject(powerUp);
        }
    }

    public void ResetBlockForPool()
    {
        DeSpawnCoins();
        DespawnPowerUps();
        _isPowerUpSpawned = false;
    }

    public Transform GetNextBlockSpawnPoint()
    {
        if (nextBlockSpawnPoint != null) return nextBlockSpawnPoint;
        Debug.LogError("Next Block Spawn Point is Not Set"); return null;
    }

    private bool CoinSpawnChance()
    {
        bool spawnChance = Random.value < _CoinSpawnChanceInPoint;
        return spawnChance;
    }
    
    private bool PowerUpSpawnChance()
    {
        bool spawnChance = Random.value < _PowerUpSpawnChanceInPoint;
        return spawnChance;
    }

    private bool ChanceToSpawnCoinAt(Vector3 position)
    {
        if (!CoinSpawnChance()) return false;
        Coin coin = CoinPoolManager.Instance.GetObject();
        coin.ResetForScene();
        coin.transform.position = position;
        coin.transform.SetParent(coinsParent);
        return true;
    }
    
    private bool ChanceToSpawnPowerUpAt(Vector3 position)
    {
        if (!PowerUpSpawnChance()) return false;
        PowerUp powerUp = PowersPoolManager.Instance.GetObject();
        powerUp.ResetForScene();
        powerUp.transform.position = position;
        powerUp.transform.SetParent(powerUpsParent);
        return true;
    }

    
}
