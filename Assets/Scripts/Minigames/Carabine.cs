using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class Carabine : MonoBehaviour
{
    private Vector2 _Mousepos;
    private Vector2 _Worldpos;

    private void Update()
    {
        _Mousepos = Input.mousePosition;
        _Worldpos = Camera.main.ScreenToWorldPoint(_Mousepos);
        transform.position = _Worldpos;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }
    IEnumerator 
}