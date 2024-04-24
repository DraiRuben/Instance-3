using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoleHammer : MonoBehaviour
{
    private Vector2 _MousePosition;
    private Vector3 _WorldMousePosition;
    private Animator _Animator;

    public void Awake()
    {
        _Animator = GetComponent<Animator>();
    }

    public void Update()
    {
        _MousePosition = Mouse.current.position.ReadValue();
        _WorldMousePosition = Camera.main.ScreenToWorldPoint(_MousePosition);
        transform.position = new Vector3(_WorldMousePosition.x, _WorldMousePosition.y,0);
        Cursor.visible = false;
        if (Input.GetMouseButtonDown(0))
        {
            _Animator.SetTrigger("Wack");
        } 
    }
}
