using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager<T> : MonoBehaviour where T : MonoBehaviour
{
   //Public Functions//
    // GetObject
    // ReturnObject
    // ReturnObjectAfter
    
    [SerializeField] protected List<T> objectPrefabs;
    protected Transform _poolTransform;
    
    protected List<T> _objectsList;
    [SerializeField] private int poolSize = 10;
    [SerializeField] private int maxPoolSize = 30;
    
    private void Awake()
    {
        
    }

    protected virtual void Start()
    {
        _poolTransform = transform;
        PreWarmPool();
    }



    private void PreWarmPool()
    {
        if (objectPrefabs == null || objectPrefabs.Count == 0)
        {
            Debug.LogError("objectPrefab List is Empty or Null! , Cannot PreWarm Pool: " + name);
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            CreateObject();
        }
    }
    
    public T GetObject()
    {
        if (_objectsList.Count == 0)
        {
            CreateObject();
        }
        if (_objectsList.Count == 0) return null;
        int randomIndex = Random.Range(0, _objectsList.Count);
        T go = _objectsList[randomIndex];
        _objectsList.RemoveAt(randomIndex);
        if (go == null)
        {
            Debug.LogError("Failed to Get Object From Pool");
            return null;
        }
        go.transform.SetParent(null);
        go.gameObject.SetActive(true);
        return go;
    }
    
    public void ReturnObject(T go)
    {
        if (!go) return;
        if (_objectsList.Count >= maxPoolSize)
        {
            Destroy(go);
            return;
        }
        ResetObject(go);
        _objectsList.Add(go);
    }

    
    private void ResetObject(T go)
    {
        go.transform.SetParent(_poolTransform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.gameObject.SetActive(false);
    }
    
    public virtual void ReturnObjectAfter(T go, float time)
    {
        StartCoroutine(ReturnAfter(go, time));
    }

    private System.Collections.IEnumerator ReturnAfter(T go, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnObject(go);
    }

    protected virtual void CreateObject()
    {
        T go = Instantiate(objectPrefabs[Random.Range(0, objectPrefabs.Count)], _poolTransform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.gameObject.SetActive(false);
        _objectsList.Add(go);
    }
    
    private void OnDestroy()
    {
        _objectsList?.Clear();
        _objectsList = null;
    }
}
