using UnityEngine;

public class PowerUp : Item
{
    //Public Functions//
    // CollectItem
    // DisableCollider
    // EnableCollider
    // GetPowerUpType
    public enum PowerUpType
    {
        None,
        SlowMotion, 
        IncreaseSpeed,
        DecreaseSpeed,
        Levitation,
        Shield,
        Rocket
    }
    
    [SerializeField] private PowerUpType powerUpType;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
    }

    
    public override void CollectItem()
    {
        
        foreach (MeshRenderer meshRenderer in _meshRenderers)
        {
            meshRenderer.enabled = false;
        }
        DisableCollider();
        PowersPoolManager.Instance.ReturnObjectAfter(this,2f);
    }
    
    public PowerUpType GetPowerUpType() => powerUpType;

    
    
}
