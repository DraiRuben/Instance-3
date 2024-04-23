using Sirenix.OdinInspector;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class Gun : MonoBehaviour
{
    private Vector2 _MousePosition;
    private Vector2 _PreviousMouseWorldPosition;
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
    [SerializeField] private float _YReach;
    [SerializeField, MinMaxSlider(-10,0)] private Vector2 _GunYRange;
    [System.NonSerialized] private float _MouseYMovementDelta;

    private void Update()
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


        _PreviousMouseWorldPosition = _WorldMousePosition;
        _MousePosition = Mouse.current.position.ReadValue();
        _WorldMousePosition = Camera.main.ScreenToWorldPoint(_MousePosition);
        _MouseYMovementDelta = _WorldMousePosition.y - _PreviousMouseWorldPosition.y;
        _OffsetX = _OffsetXBase * _OffsetXEvolution.Evaluate(_OffsetXTimer);
        _OffsetY = _OffsetYBase * _OffsetYEvolution.Evaluate(_OffsetYTimer);
        transform.position = new Vector3(_WorldMousePosition.x + _OffsetX, _WorldMousePosition.y - transform.lossyScale.y / 2 - _YReach + _OffsetY);

        //constrains cursor in the Y axis to the shootable zone
        var screenPoint = Mathf.Clamp(_MousePosition.y,
            Camera.main.WorldToScreenPoint(new(0,transform.parent.position.y+ _GunYRange.x + transform.lossyScale.y / 2 + _YReach)).y , 
            Camera.main.WorldToScreenPoint(new(0, transform.parent.position.y + _GunYRange.y + transform.lossyScale.y / 2 + _YReach)).y);
        if(screenPoint!= _MousePosition.y)
        Mouse.current.WarpCursorPosition(new(_MousePosition.x, screenPoint));
    }
}