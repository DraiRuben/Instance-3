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
    private float _LastWackTime;
    public void Awake()
    {
        _Animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        PlayerControls.Instance?.OnSelect.AddListener(() =>
        {
            if(Time.time- _LastWackTime > 0.2f && Time.timeScale == 1)
            {
                _LastWackTime = Time.time;
                _Animator.SetTrigger("Wack");
            }
        });
    }
    private void OnDisable()
    {
        PlayerControls.Instance?.OnSelect.RemoveAllListeners();
    }
    public void Update()
    {
        if(Time.timeScale == 1)
        {
            _MousePosition = Mouse.current.position.ReadValue();
            _WorldMousePosition = Camera.main.ScreenToWorldPoint(_MousePosition);
            transform.position = new Vector3(_WorldMousePosition.x, _WorldMousePosition.y, 0);
            Cursor.visible = false;
        }
    }
}
