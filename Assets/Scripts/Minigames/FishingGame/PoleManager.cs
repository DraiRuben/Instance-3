using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PoleManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI _FishingCountText;

    [Header("Params")]
    [SerializeField] private float _FishingCooldown;
    [SerializeField] private float _FishingMaxCount;
    //Mouse follow params
    [SerializeField] private float _OffsetXBase;
    [SerializeField] private float _OffsetYBase;
    [SerializeField] private float _OffsetXDuration;
    [SerializeField] private float _OffsetYDuration;

    [SerializeField] private AnimationCurve _OffsetXEvolution;
    [SerializeField] private AnimationCurve _OffsetYEvolution;
    [SerializeField] private Vector2 _SpriteOffset;

    [Header("Mouse confinement range")]
    [SerializeField] private Vector2 _MouseYRange;
    [SerializeField] private Vector2 _MouseXRange;

    private Vector2 _MousePosition;
    private Vector2 _PreviousMousePosition;
    private Vector3 _WorldMousePosition;
    private float _OffsetX;
    private float _OffsetY;
    private float _OffsetXTimer;
    private float _OffsetYTimer;


    private bool _IsFishing;
    private bool _IsInFishAnim;

    [System.NonSerialized] public int _CurrentFishingCount;
    private Animator _Animator;
    private float _LastFishTime;


    //Events
    private UnityEvent OnHookDown = new();
    private UnityEvent OnHookReel = new();

    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        _FishingCountText.text = "Tentatives :" + (_FishingMaxCount - _CurrentFishingCount).ToString();
    }
    private void OnDisable()
    {
        _IsFishing = false;
        _IsInFishAnim = false;
        _Animator.SetTrigger("Reset");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (_IsFishing)
        {
            if (other.CompareTag("Fish"))
            {
                AudioManager._Instance.PlaySFX("fishCatch", true);
                FishManager.Instance._FishList.Remove(other.gameObject);
                FishManager.Instance._Points++;
                _IsFishing = false;
                other.gameObject.GetComponent<Fish>().DoDestructionFeedback();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {

        if (_IsFishing)
        {
            if (other.CompareTag("Fish"))
            {
                FishManager.Instance._FishList.Remove(other.gameObject);
                FishManager.Instance._Points++;
                _IsFishing = false;
                other.gameObject.GetComponent<Fish>().DoDestructionFeedback();
            }
        }
    }

    private void Update()
    {
        //follow mouse and constrain mouse to defined zone
        if (Time.timeScale == 1)
        {
            if (!_IsInFishAnim)
            {
                Cursor.visible = false;
                _MousePosition = Mouse.current.position.ReadValue();
                _PreviousMousePosition = _MousePosition;
                _WorldMousePosition = Camera.main.ScreenToWorldPoint(_MousePosition);

                _OffsetXTimer += Time.deltaTime / _OffsetXDuration;
                _OffsetYTimer += Time.deltaTime / _OffsetYDuration;

                if (_OffsetXTimer > 1)
                {
                    _OffsetXTimer = 0;
                }
                if (_OffsetYTimer > 1)
                {
                    _OffsetYTimer = 0;
                }
                _OffsetX = _OffsetXBase * _OffsetXEvolution.Evaluate(_OffsetXTimer);
                _OffsetY = _OffsetYBase * _OffsetYEvolution.Evaluate(_OffsetYTimer);

                transform.parent.position = new Vector3(_WorldMousePosition.x + _OffsetX + _SpriteOffset.x, _WorldMousePosition.y - _OffsetY + _SpriteOffset.y);
                Vector2 screenPoint = new Vector2();

                //constrains cursor in the Y axis to the shootable zone
                screenPoint.y = Mathf.Clamp(_MousePosition.y,
                    Camera.main.WorldToScreenPoint(new(0, transform.parent.parent.position.y + _MouseYRange.x)).y,
                    Camera.main.WorldToScreenPoint(new(0, transform.parent.parent.position.y + _MouseYRange.y)).y);

                //constrains cursor in the X axis to the shootable zone
                screenPoint.x = Mathf.Clamp(_MousePosition.x,
                    Camera.main.WorldToScreenPoint(new(transform.parent.parent.position.x + _MouseXRange.x, 0)).x,
                    Camera.main.WorldToScreenPoint(new(transform.parent.parent.position.x + _MouseXRange.y, 0)).x);
                if (screenPoint != _MousePosition) Mouse.current.WarpCursorPosition(screenPoint);
            }
            else
            {
                //stop cursor from moving around so that when fishing rod stops its anim, the sprite doesn't teleport around
                Mouse.current.WarpCursorPosition(_PreviousMousePosition);
            }
        }

    }
    public void Fishing(InputAction.CallbackContext context)
    {
        if (context.started && !_IsFishing && _CurrentFishingCount < _FishingMaxCount && Time.timeScale == 1)
        {
            if (Time.time - _LastFishTime > _FishingCooldown)
            {
                _LastFishTime = Time.time;
                _CurrentFishingCount++;
                _FishingCountText.text = "Tentatives :" + (_FishingMaxCount - _CurrentFishingCount).ToString();
                StartCoroutine(Tofish());
            }
        }
    }
    public void StartDownTime()
    {
        OnHookDown.Invoke();
    }
    public void StartReeling()
    {
        OnHookReel.Invoke();
        AudioManager._Instance.PlaySFX("fishFail");
    }
    IEnumerator Tofish()
    {
        //time for hook to be down
        _Animator.SetTrigger("Reel");
        _IsInFishAnim = true;
        yield return WaitUntilEvent(OnHookDown);
        _IsFishing = true;
        yield return WaitUntilEvent(OnHookReel);
        _IsFishing = false;
        _IsInFishAnim = false;
        if (_CurrentFishingCount >= _FishingMaxCount)
        {
            FishManager.Instance._ElapsedTime = 9999999;
        }
    }
    private IEnumerator WaitUntilEvent(UnityEvent unityEvent)
    {
        bool trigger = false;
        Action action = () => trigger = true;
        unityEvent.AddListener(action.Invoke);
        yield return new WaitUntil(() => trigger);
        unityEvent.RemoveListener(action.Invoke);
    }
}
