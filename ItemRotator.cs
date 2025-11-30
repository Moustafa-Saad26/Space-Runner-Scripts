using UnityEngine;

public class ItemRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f; 
    [SerializeField] private Vector3 rotationAxis = Vector3.up; 

    private void Update()
    {
        transform.Rotate(rotationAxis * (rotationSpeed * Time.deltaTime) );
    }
}
