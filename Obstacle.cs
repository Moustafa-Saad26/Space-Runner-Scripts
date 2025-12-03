using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private ParticleSystem _explosionPT;
    private MeshRenderer _meshRenderer;
    private SphereCollider _collider;
    
    public static event EventHandler OnObstacleExploded;


    private void Awake()
    {
        _collider = GetComponent<SphereCollider>();
        _explosionPT = GetComponent<ParticleSystem>();
        _meshRenderer = transform.GetChild(0).GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        _meshRenderer.enabled = true;
        _collider.enabled = true;
    }

    private void Start()
    {
        _explosionPT.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player) || other.TryGetComponent(out Rocket rocket))
        {
            Explode();
        } 
    }
    public void Explode()
    {
        OnObstacleExploded?.Invoke(this, EventArgs.Empty);
        _collider.enabled = false;
        _meshRenderer.enabled = false;
        _explosionPT.Play();
    }

    public void ResetForScene()
    {
        _meshRenderer.enabled = true;
        _collider.enabled = true;
        _explosionPT.Stop();
    }
}
