using System.Collections;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public static FadeInOut Instance;
    private Animator _Animator;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
    }
    private void Start()
    {
        _Animator = transform.GetChild(0).GetComponent<Animator>();
    }
    public IEnumerator FadeToBlack()
    {
        yield return null;
        _Animator.SetBool("HideScreen", true);
        yield return new WaitForSeconds(0.35f / 0.6f);
    }
    public IEnumerator FadeToBlackThenTransparent()
    {
        yield return FadeToBlack();
        yield return FadeToTransparent();
    }
    public IEnumerator FadeToTransparent()
    {
        yield return null;
        _Animator.SetBool("HideScreen", false);
        yield return new WaitForSeconds(0.35f / 0.6f);
    }
}
