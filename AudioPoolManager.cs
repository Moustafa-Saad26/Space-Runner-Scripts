using System.Collections.Generic;
using UnityEngine;

public class AudioPoolManager : PoolManager<SoundSource>
{
    public static AudioPoolManager Instance {private set; get;}

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _objectsList = new List<SoundSource>();
    }

    public override void ReturnObjectAfter(SoundSource soundSource, float time)
    {
        StartCoroutine(ReturnAfter(soundSource, time));
    }

    private System.Collections.IEnumerator ReturnAfter(SoundSource soundSource, float time)
    {
        yield return new WaitForSeconds(time);
        soundSource.ResetForPool();
        ReturnObject(soundSource);
    }
}
