using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pole : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _FishingScoreText;
    [SerializeField] private TextMeshProUGUI _FishingTimerText;
    private bool _Fishing;
    private int _FishingScore;
    private float _FishingTimer;

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

    private void Update()
    {
        _FishingTimer += Time.deltaTime;
        _FishingScoreText.text = "score : " + _FishingScore;
        _FishingTimerText.text = "time : " + Mathf.RoundToInt(30-_FishingTimer);
        if (_FishingTimer > 30)
        {
            Debug.Log("game end");
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
