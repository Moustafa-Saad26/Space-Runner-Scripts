   using System;
   using System.Collections.Generic;
   using UnityEngine;

   public class BlockSpawner : MonoBehaviour
   { 
      public static BlockSpawner Instance {private set; get;}
      
      private Block _currentActiveBlock;
      private Queue<Block> _activeBlocks;
      private Transform _blockSpawnerTransform;
      [SerializeField] private Block startingBlock;
      private bool _isStartingBlockActive = true;
      [SerializeField] private int initialSpawnCount = 2;
      [SerializeField] private int numberOfBlocksToSpawn = 2;
      [SerializeField] private int maxNumberOfBlocksActive = 6;

      private void Awake()
      {
         if (Instance != null && Instance != this)
         {
            Destroy(gameObject);
            return;
         }
         Instance = this;
         _blockSpawnerTransform = transform;
         _activeBlocks = new Queue<Block>();
      }
      
      private void Start()
      {
         if(startingBlock == null) Debug.LogError("No starting block set in BlockSpawner");
         _currentActiveBlock = startingBlock;
         _activeBlocks.Enqueue(startingBlock);
         if (Player.Instance != null)
         {
            Player.Instance.OnSpawnTriggered += Player_OnSpawnTriggered;
            Player.Instance.OnDespawnTriggered += Player_OnDeSpawnTriggered;
         }
         if(numberOfBlocksToSpawn <= 0)
         {
            Debug.LogError("Number of blocks to spawn must be greater than 0 in BlockSpawner");
            numberOfBlocksToSpawn = 1; // default fallback
         }
      }

      
      private void Player_OnSpawnTriggered(object sender, EventArgs e)
      {
         if (_isStartingBlockActive)
         {
            for (int i = 0; i < initialSpawnCount; i++)
            {
               SpawnBlockAt(_currentActiveBlock.GetNextBlockSpawnPoint().position);
            }
            return;
         }

         for (int i = 0; i < numberOfBlocksToSpawn; i++)
         {
            if (_activeBlocks.Count >= maxNumberOfBlocksActive) return;
            SpawnBlockAt(_currentActiveBlock.GetNextBlockSpawnPoint().position);
         }
      }
      
      private void Player_OnDeSpawnTriggered(object sender, EventArgs e)
      {
         Block blockToRemove = _activeBlocks.Dequeue();
         if (_isStartingBlockActive)
         {
            Destroy(blockToRemove.gameObject);
            _isStartingBlockActive = false;
            return;
         }
         blockToRemove.ResetBlockForPool();
         BlockPoolManager.Instance.ReturnObjectAfter(blockToRemove, 1f); // can be returned immediately if starting block does not have despawn trigger
      }


      private void SpawnBlockAt(Vector3 position)
      {
         Block newBlock = BlockPoolManager.Instance.GetObject();
         newBlock.transform.SetParent(_blockSpawnerTransform);
         newBlock.transform.position = position;
         newBlock.SpawnItems();
         _currentActiveBlock = newBlock;
         _activeBlocks.Enqueue(newBlock);
      }
      private void OnDestroy()
      {
         if (Player.Instance != null)
         {
            Player.Instance.OnSpawnTriggered -= Player_OnSpawnTriggered;
            Player.Instance.OnDespawnTriggered -= Player_OnDeSpawnTriggered;
         }
         _activeBlocks?.Clear();
         _activeBlocks = null;
      }
   }
