using UnityEngine;

public class FadeInOut : MonoBehaviour
{

    [SerializeField] private Animator _Animator;
    private Collider2D _Collider;

    public void FadeToBlack(Collider2D collision)
    {
        _Collider = collision;
        _Animator.SetTrigger("FadeOut");
    }


    public void FadeInComplete()
    {
        
    }
}
