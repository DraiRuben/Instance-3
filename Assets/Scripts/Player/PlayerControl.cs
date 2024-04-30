using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance;
    [SerializeField] private float _Speed;
    private Rigidbody2D _RBody;
    private Animator _Animator;
    private SpriteRenderer _SpriteRenderer;
    private Vector2 _MoveInput;
    [System.NonSerialized] public DialogueTrigger _CurrentDialogue;
    private StandInteractableTrigger _CurrentInteractable;
    [System.NonSerialized] public UnityEvent OnSelect = new();
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
    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/PlayerPosition.json"))
        {
            JsonDataService dataService = new JsonDataService();
            var position = dataService.LoadData<Vector3Json>("PlayerPosition");
            transform.position = new Vector3(position.x, position.y, position.z);
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
        if (_CurrentInteractable == interactable)
        {
            _CurrentInteractable = null;
        }
    }
    //overworld action map
    public void Movement(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 1)
        {
            _MoveInput = context.ReadValue<Vector2>();
            if (_MoveInput.x < -0)
            {
                _SpriteRenderer.flipX = true;
            }
            else if (_MoveInput.x > 0)
            {
                _SpriteRenderer.flipX = false;
            }

            if (_MoveInput.x != 0.0f || _MoveInput.y != 0.0f)
            {
                _Animator.SetBool("isWalking", true);
            }
            else
            {
                _Animator.SetBool("isWalking", false);
            }
        }
    }


    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started && !PauseMenu.instance._IsPauseBlocked)
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            //check if sub menus are active then deactivate the deepest menu
            if (PauseMenu.instance.gameObject.activeSelf)
            {
                if (SettingsMenu.instance.gameObject.activeSelf)
                {
                    if (AudioMenu.instance.gameObject.activeSelf)
                    {
                        AudioMenu.instance.gameObject.SetActive(false);
                        return;//return so only one menu at a time gets deactivated 
                    }
                    SettingsMenu.instance.gameObject.SetActive(false);
                    return;//return so only one menu at a time gets deactivated 
                }
                else if (ControlsMenu.Instance.gameObject.activeSelf)
                {
                    ControlsMenu.Instance.gameObject.SetActive(false);
                    return;
                }
                else
                {
                    Time.timeScale = 1;
                    PauseMenu.instance.gameObject.SetActive(Time.timeScale == 0);
                }
            }
            //if pause menu not active then active it, sub menus are inacessible except with pauseMenu
            else
            {
                PauseMenu.instance.gameObject.SetActive(Time.timeScale == 0);
            }
            //Time.timeScale = Time.timeScale == 0 ? 1 : 0;


        }
    }
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started && Time.timeScale == 1)
        {
            if (_CurrentInteractable)
            {
                _CurrentInteractable.Interact();
                _PlayerInput.SwitchCurrentActionMap("Menus");
                if (_CurrentInteractable._CurrentDialogue) _CurrentDialogue = _CurrentInteractable._CurrentDialogue;

            }
        }
    }
    public void SkipDialogue(InputAction.CallbackContext context)
    {
        if (_CurrentDialogue && context.started && Time.timeScale == 1)
        {
            _CurrentDialogue.SkipDialogue();
        }
    }
    //menu action map
    public void Select(InputAction.CallbackContext context)
    {
        if (context.started)
            OnSelect.Invoke();
        if (_CurrentDialogue && context.started && Time.timeScale == 1)
        {
            _CurrentDialogue.SkipDialogue();
        }
    }
    private void OnApplicationQuit()
    {
        JsonDataService dataService = new JsonDataService();
        dataService.SaveData("PlayerPosition", new Vector3Json(transform.position.x, transform.position.y, transform.position.z));
    }
    //newtonsoft can't serialize vector3's because of all values in the struct
    public struct Vector3Json
    {
        public float x;
        public float y;
        public float z;
        public Vector3Json(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
    public void PlayWalkSound()
    {
        AudioManager._Instance.PlaySFX("playerWalk", true);
    }
}
