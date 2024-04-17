using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private float _Speed;
    [SerializeField] private Rigidbody2D _RBody;
    private Vector2 _MoveInput;
    private bool _IsWalking = false;

    public void movement(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _IsWalking = true;
            _MoveInput = context.ReadValue<Vector2>();
        }
        if (context.canceled)
        {
            _IsWalking = false;
        }
    }

    private void Update()
    {
        if (_IsWalking)
        {
            _RBody.velocity = _MoveInput * _Speed;
        }
    }
}
