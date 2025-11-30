
using System.Collections.Generic;

public class BlockPoolManager : PoolManager<Block>
{

    public static BlockPoolManager Instance {private set; get;}
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _objectsList = new List<Block>();
    }
    
    
    
}
