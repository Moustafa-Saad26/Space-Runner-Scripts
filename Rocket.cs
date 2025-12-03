using UnityEngine;

public class Rocket : MonoBehaviour
{
    
    public void Launch(float speed, float duration)
    {
        StartCoroutine(LaunchRoutine(speed, duration));
    }

    private System.Collections.IEnumerator LaunchRoutine(float speed, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            transform.position +=  Vector3.right * (speed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Obstacle obstacle))
        {
            obstacle.Explode();
            Destroy(gameObject);
        }
    }
}
