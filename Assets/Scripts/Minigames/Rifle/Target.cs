using UnityEngine;
using UnityEngine.EventSystems;

public class Target : MonoBehaviour, IPointerDownHandler
{
    private Animator _Animator;
    private void Awake()
    {
        _Animator = transform.parent.GetComponent<Animator>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        RifleMinigame.Instance.HandleTargetHit(this);
    }
    public void DoDestructionFeedback()
    {
        //play anim
        _Animator.SetTrigger("ChangeState");
        //TODO: play sound
    }
}
