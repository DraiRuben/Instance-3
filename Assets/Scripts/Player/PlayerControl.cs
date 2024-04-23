using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    FadeInOut _FadeInOut;
    [SerializeField] private float _Speed;
    private Rigidbody2D _RBody;
    private Vector2 _MoveInput;
    private StandInteractableTrigger _CurrentInteractable;
    private PlayerInput _PlayerInput;
    private void Awake()
    {
        _RBody = GetComponent<Rigidbody2D>();
        _PlayerInput = GetComponent<PlayerInput>();
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var interactable = collision.gameObject.GetComponent<StandInteractableTrigger>();
        if (interactable)
        {
            _CurrentInteractable = interactable;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        var interactable = collision.gameObject.GetComponent<StandInteractableTrigger>();
        if(_CurrentInteractable == interactable)
        {
            _CurrentInteractable = null;
        }
    }
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (_CurrentInteractable)
            {
                _CurrentInteractable.Interact();
                _PlayerInput.SwitchCurrentActionMap("Menus");
            }
        }
    }
}
