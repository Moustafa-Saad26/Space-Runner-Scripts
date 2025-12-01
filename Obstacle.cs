using System;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private ParticleSystem _explosionPT;
    private MeshRenderer _meshRenderer;
    private SphereCollider _collider;


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

    public void Explode()
    {
        _collider.enabled = false;
        _meshRenderer.enabled = false;
        _explosionPT.Play();
    }
}
