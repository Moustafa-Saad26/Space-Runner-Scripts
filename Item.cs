using UnityEngine;

public class Item : MonoBehaviour
{
    //Public Functions//
    // CollectCoin
    // DisableCollider
    // EnableCollider
    // GetCoinValue
    // GetSpawnWeight
    // GetCoinName
    
    protected Collider _collider;

    [SerializeField] private string itemName = "Default Item";
    [SerializeField] private int spawnWeight = 50;

    [SerializeField] protected MeshRenderer[] _meshRenderers;
    
   
    private void Start()
    {
        
    }
   
    
    public void DisableCollider()
    {
        _collider.enabled = false;
    }

    public void EnableCollider()
    {
        _collider.enabled = true;
    }

    public virtual void CollectItem()
    {
        DisableCollider();
        Debug.LogError("Reached Base Item Collect Function");
    }
    

    public void ResetForScene()
    {
        foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = true;
        }
        EnableCollider();
    }
    
    
    public int GetSpawnWeight() => spawnWeight;

    public string GetItemName() => itemName;
    
    
}
