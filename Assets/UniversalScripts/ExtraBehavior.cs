using UnityEngine;
using UnityEngine.Audio;

public class ExtraBehavior : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource source;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        if (source != null)
        {
            AudioMixer mixerGroup = Resources.Load<AudioMixer>("main");
            var group = mixerGroup.FindMatchingGroups("TouchNoises")[0];
            if (mixerGroup != null)
            {
                source.outputAudioMixerGroup = group;
            }

        }
    }
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
