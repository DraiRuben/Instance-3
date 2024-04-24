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
        _Animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.35f);
    }
}
