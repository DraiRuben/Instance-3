using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    FadeInOut _FadeInOut;
    public static PlayerControls Instance;
    [SerializeField] private float _Speed;
    private Rigidbody2D _RBody;
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
    }
    public void Movement(InputAction.CallbackContext context)
    {
        _MoveInput = context.ReadValue<Vector2>();
    }


    public void Pause(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (Time.timeScale == 1  && !PauseMenu.instance.gameObject.activeSelf)
            {
                PauseMenu.instance.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            else if (!SettingsMenu.instance.gameObject.activeSelf)
            {
                Time.timeScale = 1;
                PauseMenu.instance.gameObject.SetActive(false);
            }
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
