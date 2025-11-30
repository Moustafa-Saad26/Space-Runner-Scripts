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
        Shield
    }
    
    [SerializeField] private PowerUpType powerUpType;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
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
        DisableCollider();
        PowersPoolManager.Instance.ReturnObjectAfter(this,2f);
    }
    
    public PowerUpType GetPowerUpType() => powerUpType;

    
    
}
