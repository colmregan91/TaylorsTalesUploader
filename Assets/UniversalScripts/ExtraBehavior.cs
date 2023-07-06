using UnityEngine;

public class ExtraBehavior : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource source;
    [SerializeField] private Animator anim;
    public void ParticleBehavior()
    {

        if (particle != null)
        {
            particle.Play(true);
        }

        if (clip != null)
        {
            source.PlayOneShot(clip);
        }
    }

    public void AnimBehavior()
    {
        if (anim != null)
        {
            anim.SetTrigger("AnimTrigger");
        }

        if (clip != null)
        {
            source.PlayOneShot(clip);
        }
    }
   
}
