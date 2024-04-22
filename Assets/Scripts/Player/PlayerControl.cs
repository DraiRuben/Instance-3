using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private float _Speed;
    private Rigidbody2D _RBody;
    private Vector2 _MoveInput;

    private void Awake()
    {
        _RBody = GetComponent<Rigidbody2D>();
    }
    public void Movement(InputAction.CallbackContext context)
    {

        _MoveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {

        _RBody.AddForce(_MoveInput * _Speed * Time.fixedDeltaTime);
        _RBody.velocity = Vector2.ClampMagnitude(_RBody.velocity, 50);

    }
}
