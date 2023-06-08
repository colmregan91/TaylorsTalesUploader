using UnityEngine;

public class ExtraBehavior : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource source;
    public void Behavior()
    {
        particle.Play(true);
        source.PlayOneShot(clip);
    }
}
