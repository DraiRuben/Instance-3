using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private float _Speed;
    [SerializeField] private Rigidbody2D _RBody;
    private Vector2 _MoveInput;

    public void Movement(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _MoveInput = context.ReadValue<Vector2>();
        }
        if (context.canceled)
        {
            _MoveInput = Vector2.zero;
        }
    }

    private void Update()
    {

            _RBody.velocity = _MoveInput * _Speed*Time.deltaTime;

    }
}
