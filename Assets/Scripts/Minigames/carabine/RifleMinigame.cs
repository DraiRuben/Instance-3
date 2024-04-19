using System.Collections;
using UnityEngine;
using System.IO;

public class RifleMinigame : MonoBehaviour
{
    public static RifleMinigame Instance;
    [Header("sound")]
    [SerializeField] private AudioClip _ReloadSound;
    [SerializeField] private AudioClip _ShootSound;
    [SerializeField] private bool _IsBugged;
    private int _Points;
    private float _ReloadTime;
    private float _MinigameTimer;

    private void Awake()
    {
        if(Instance) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Animations")) Directory.CreateDirectory("Game/Animations");
        if (!Directory.Exists("Game/Animations/Rifle")) Directory.CreateDirectory("Game/Animations/Rifle");
        _IsBugged = !File.Exists("Game/Animations/Rifle/Target.anim");
    }

    private void Update()
    {
        //_MinigameTimer = Time.deltaTime;
        //if (_MinigameTimer >= 45.0f)
        //{
            
        //}

        ReloadTimer();
        if (Input.GetMouseButtonDown(0) && _ReloadTime <= 0)
        {
            StartCoroutine(Shoot());
        }
    }

    public void Handletargethit(GameObject target)
    {
        if (_ReloadTime <= 0)
        {
            if (!_IsBugged)
            {
                Destroy(target);
                _Points++;
            }
        }
    }

    private IEnumerator Shoot()
    {
        FindObjectOfType<AudioManager>().PlaySound("shoot");
        yield return null;
        _ReloadTime = _ReloadSound.length + _ShootSound.length;
        yield return new WaitForSeconds(_ShootSound.length);
        FindObjectOfType<AudioManager>().PlaySound("reload");
    }

    private void ReloadTimer()
    {
        if (_ReloadTime > 0)
        {
            _ReloadTime -= Time.deltaTime;
        }
    }

    private void Aimshake()
    {

    }

    
}