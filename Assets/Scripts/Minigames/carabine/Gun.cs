using Sirenix.OdinInspector;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Gun : MonoBehaviour
{
    private Vector2 _MousePosition;
    private Vector3 _WorldMousePosition;
    [Header("Params")]
    [SerializeField] private float _OffsetXBase;
    [SerializeField] private float _OffsetYBase;
    [SerializeField] private float _OffsetXDuration;
    [SerializeField] private float _OffsetYDuration;

    [SerializeField] private AnimationCurve _OffsetXEvolution;
    [SerializeField] private AnimationCurve _OffsetYEvolution;

    [SerializeField] private Vector2 _SpriteOffset;
    [SerializeField] private Vector2 _MouseYRange;
    [SerializeField] private Vector2 _MouseXRange;

    private float _OffsetXTimer;
    private float _OffsetYTimer;
    private float _OffsetX;
    private float _OffsetY;

    private Vector3 _InitialCamPos;
    private Vector3 _CameraShakeOffset;

    private void OnEnable()
    {
        _InitialCamPos = Camera.main.transform.position;

    }
    private void Update()
    {
        if(Time.timeScale == 1)
        {
            Cursor.visible = false;
            _CameraShakeOffset = _InitialCamPos - Camera.main.transform.position;
            _CameraShakeOffset.z = 0;
            _MousePosition = Mouse.current.position.ReadValue();
            _WorldMousePosition = Camera.main.ScreenToWorldPoint(_MousePosition);
            if(_CameraShakeOffset.magnitude==0)
            {
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
            }
            transform.position = new Vector3(_WorldMousePosition.x + _OffsetX + _SpriteOffset.x, _WorldMousePosition.y - _OffsetY + _SpriteOffset.y) + _CameraShakeOffset;
            var screenPoint = new Vector2();

            //constrains cursor in the Y axis to the shootable zone
            screenPoint.y = Mathf.Clamp(_MousePosition.y,
                Camera.main.WorldToScreenPoint(new(0, transform.parent.position.y + _MouseYRange.x)).y,
                Camera.main.WorldToScreenPoint(new(0, transform.parent.position.y + _MouseYRange.y)).y);

            //constrains cursor in the X axis to the shootable zone
            screenPoint.x = Mathf.Clamp(_MousePosition.x,
                Camera.main.WorldToScreenPoint(new(transform.parent.position.x + _MouseXRange.x, 0)).x,
                Camera.main.WorldToScreenPoint(new(transform.parent.position.x + _MouseXRange.y, 0)).x);
            if (screenPoint != _MousePosition) Mouse.current.WarpCursorPosition(screenPoint);
        }
    }
}