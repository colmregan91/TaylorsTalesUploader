
using UnityEngine;
public class AnimationBehavior : MonoBehaviour
{
    private Interaction interaction;

    [SerializeField]private Animator anim;
    private int triggerHash = Animator.StringToHash("AnimTrigger");

    private void Awake()
    {
        interaction = GetComponent<Interaction>();
    }

    private void Start()
    {
        interaction.SubscribeBehavior(behavior);
    }

    private void behavior()
    {
        anim.SetTrigger(triggerHash);
    }

    private void OnDisable()
    {
        interaction.UnsubscribeBehavior(behavior);
    }

}

//public interface IBehave
//{
//    public void Subscribe();
//    public void Unsubscribe();
//    public void PerformAction();
//}
