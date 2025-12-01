using UnityEngine;

public class SoundSource : MonoBehaviour
{
     private AudioSource _audioSource;

     
     private void Awake()
     {
          _audioSource = GetComponent<AudioSource>();
     }

     private void Start()
     {
          
     }

     public void SetPitch(float pitch)
     {
          _audioSource.pitch = pitch;
     }

     public void ResetForPool()
     {
          _audioSource.volume = 1;
          _audioSource.pitch = 1;
          _audioSource.Stop();
          _audioSource.clip = null;
          _audioSource.loop = false;
     }
     public void PlaySound2D(AudioClip clip, float volume = 1)
     {
          _audioSource.volume = volume;
          _audioSource.spatialBlend = 0;
          _audioSource.PlayOneShot(clip);
     }
     
     public void PlayLongSound2D(AudioClip clip, float volume = 1)
     {
          _audioSource.volume = volume;
          _audioSource.spatialBlend = 0;
          _audioSource.clip = clip;
          _audioSource.Play();
     }
     
     public void PlaySound2DLooped(AudioClip clip, float volume = 1)
     {
         
          _audioSource.clip = clip;
          _audioSource.loop = true;
          _audioSource.spatialBlend = 0;
          _audioSource.volume = volume;
          _audioSource.pitch = 0.9f;
          _audioSource.Play();
     }
     
     public void PlaySound3D(AudioClip clip)
     {
          _audioSource.spatialBlend = 1;
          _audioSource.PlayOneShot(clip);
     }
     
     public void changeVolume(float volume)
     {
          _audioSource.volume = volume;
     }
}
