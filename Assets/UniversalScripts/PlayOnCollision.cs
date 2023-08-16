using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class PlayOnCollision : MonoBehaviour
{

    //[SerializeField] private AudioClip clip;
    //private bool played;
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (played) return;
    //    if (collision.gameObject.name.Contains("apple"))
    //    {
    //        source.clip = clip;
    //        source.Play();
    //        played = true;
    //    }
    //}
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

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool hasPlayed;

    private const string GROUNDCOLLIDER = "GroundCollider";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger)
        {
            playClipChomp();
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {

        playClipAppleThud(collision.gameObject.name);
    }
    private void playClipAppleThud(string name)
    {
        if (hasPlayed) return;

        if (name.Equals(GROUNDCOLLIDER))
        {
            source.PlayOneShot(clip);
            hasPlayed = true;
        }

    }
    private void playClipChomp()
    {
        if (source.isPlaying) return;

        source.PlayOneShot(clip);
    }
}
