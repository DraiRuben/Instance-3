using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class FishBehavior : MonoBehaviour
{
    [SerializeField] private float _Speed;
    [SerializeField] private float _Radius;
    private Vector2 _Center;
    private float _Angle;

    private void Start()
    {
        _Center = transform.position;
    }

    void Update()
    {
        _Angle += _Speed * Time.deltaTime;
        var offset = new Vector2(Mathf.Sin(_Angle), Mathf.Cos(_Angle))*_Radius;
        transform.position = _Center + offset;
    }
}
