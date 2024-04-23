using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    FadeInOut _FadeInOut;
    public static PlayerControls Instance;
    [SerializeField] private float _Speed;
    private Rigidbody2D _RBody;
    private Animator _Animator;
    private SpriteRenderer _SpriteRenderer;
    private Vector2 _MoveInput;
    private DialogueTrigger _CurrentDialogue;
    private StandInteractableTrigger _CurrentInteractable;
    [System.NonSerialized] public PlayerInput _PlayerInput;
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;

        _RBody = GetComponent<Rigidbody2D>();
        _PlayerInput = GetComponent<PlayerInput>();
        _Animator = GetComponent<Animator>();
        _SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Movement(InputAction.CallbackContext context)
    {
        _MoveInput = context.ReadValue<Vector2>();
        if (_MoveInput.x <= -1){
            _SpriteRenderer.flipX = true;
        }
        else if(_MoveInput.x >= 1)
        {
            _SpriteRenderer.flipX = false;
        }

        if(_MoveInput.x != 0.0f || _MoveInput.y != 0.0f)
        {
            _Animator.SetBool("isWalking", true);
        }
        else
        {
            _Animator.SetBool("isWalking", false);
        }
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
                if (_CurrentInteractable.CanInteract())
                {
                    _PlayerInput.SwitchCurrentActionMap("Menus");
                    if (_CurrentInteractable._Dialogue) _CurrentDialogue = _CurrentInteractable._Dialogue;
                }
                _CurrentInteractable.Interact();
            }
        }
    }
    public void SkipDialogue(InputAction.CallbackContext context)
    {
        if (_CurrentDialogue && context.started)
        {
            _CurrentDialogue.SkipDialogue();
        }
    }
 }
