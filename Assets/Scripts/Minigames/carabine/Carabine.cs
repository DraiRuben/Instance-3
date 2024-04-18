using UnityEngine;

public class Carabine : MonoBehaviour
{
    [SerializeField] private AudioClip _Reloadsoud;
    private GameObject _Target;
    private Vector2 _Mousepos;
    private Vector2 _Worldpos;
    private float _Reloadtime;
    public bool _Isbugged;

    private void Update()
    {
        _Mousepos = Input.mousePosition;
        _Worldpos = Camera.main.ScreenToWorldPoint(_Mousepos);
        transform.position = _Worldpos;
        ReloadTimer();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetMouseButtonDown(0) && _Reloadtime <= 0)
        {
            Destroy(collision.gameObject);
            _Reloadtime = _Reloadsoud.length;
            print("bang");
            FindObjectOfType<AudioManager>().PlaySound("a");
        }
    }

    private void ReloadTimer()
    {
        if (_Reloadtime > 0)
        {
            _Reloadtime -= Time.deltaTime;
        }
    }

    private void Aimshake()
    {

    }
}