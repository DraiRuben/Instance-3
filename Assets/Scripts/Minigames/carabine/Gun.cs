using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    private Vector2 _MousePosition;
    private Vector3 _WorldMousePosition;
    [SerializeField] private float _OffsetXBase;
    [SerializeField] private float _OffsetYBase;
    [SerializeField] private float _OffsetXDuration;
    [SerializeField] private float _OffsetYDuration;
    private float _OffsetX;
    private float _OffsetY;
    [SerializeField] private AnimationCurve _OffsetXEvolution;
    [SerializeField] private AnimationCurve _OffsetYEvolution;
    private float _OffsetXTimer;
    private float _OffsetYTimer;
    private float _IntialY;

    private void Start()
    {
        _IntialY = transform.position.y;
    }

    private void Update()
    {
        _OffsetXTimer += Time.deltaTime/_OffsetXDuration;
        _OffsetYTimer += Time.deltaTime/_OffsetYDuration;

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

        _MousePosition = Mouse.current.position.ReadValue();
        _WorldMousePosition = Camera.main.ScreenToWorldPoint(_MousePosition);
        transform.position = new Vector3(_WorldMousePosition.x+_OffsetX, _IntialY+_OffsetY);
    }


}
