using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pole : MonoBehaviour
{
    private bool _Fishing;
    private int _FishingScore;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_Fishing)
        {
            if (other.CompareTag("Fish"))
            {
                FishingManager._FishInstance._FishList.Remove(other.gameObject);
                _FishingScore++;
                _Fishing = false;
                Destroy(other.gameObject);
            }
        }
    }
    public void Fishing(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartCoroutine(Tofish());
        }
    }

    IEnumerator Tofish()
    {
        _Fishing = true;
        yield return new WaitForSeconds(0.5f);
        _Fishing = false;
    }
}
