using UnityEngine;

public class Lookatcursor : MonoBehaviour
{
    [SerializeField]GameObject _Target;

    private void Update()
    {
        transform.up = _Target.transform.position - transform.position;
    }
}
