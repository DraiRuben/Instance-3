using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public static PlayerControls Instance;
    [SerializeField] private float _Speed;
    [SerializeField] private DialogueTrigger _DialogueWindow;
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
            Vector3Json position = dataService.LoadData<Vector3Json>("PlayerPosition");
            transform.position = new Vector3(position.x, position.y, position.z);
            FadeInOut.Instance.StartCoroutine(FadeInOut.Instance.FadeToTransparent());
        }
        else
        {
            StartCoroutine(LoreDialogue());
        }
    }

    private IEnumerator LoreDialogue()
    {
        SetVisibility(false,0.0f);
        _PlayerInput.SwitchCurrentActionMap("Menus");
        _CurrentDialogue = _DialogueWindow;
        yield return null;
        _DialogueWindow.TriggerDialogue();
    }
    private void FixedUpdate()
    {
        if (_MoveInput.y == 0.1f || _MoveInput.y == -0.1f)
            _RBody.AddForce(new Vector2(_MoveInput.x, 0) * _Speed * Time.fixedDeltaTime);
        else
            _RBody.AddForce(_MoveInput * _Speed * Time.fixedDeltaTime);

        _RBody.velocity = Vector2.ClampMagnitude(_RBody.velocity, 50);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        StandInteractableTrigger interactable = collision.gameObject.GetComponent<StandInteractableTrigger>();
        if (interactable)
        {
            _CurrentInteractable = interactable;
            _CurrentInteractable._Highlight.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        StandInteractableTrigger interactable = collision.gameObject.GetComponent<StandInteractableTrigger>();
        if (_CurrentInteractable == interactable && interactable)
        {
            if(_CurrentInteractable._Highlight !=null) _CurrentInteractable._Highlight.SetActive(false);

            _CurrentInteractable = null;
        }
    }
    //overworld action map
    public void Movement(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 1)
        {
            Vector2 tempInput = context.ReadValue<Vector2>();
            UpdatePlayerAnim(tempInput);
        }
    }
    private void UpdatePlayerAnim(Vector2 movementInput)
    {
        if (movementInput.x < -0)
        {
            _SpriteRenderer.flipX = true;
        }
        else if (movementInput.x > 0)
        {
            _SpriteRenderer.flipX = false;
        }


        if (movementInput.y == 0 && movementInput.x == 0)
        {
            if (_MoveInput.y > 0)
            {
                movementInput.y = 0.1f;
            }
            else if (_MoveInput.y < 0)
            {
                movementInput.y = -0.1f;
            }
        }
        _MoveInput = movementInput;
        _Animator.SetFloat("X", _MoveInput.x);
        _Animator.SetFloat("Y", _MoveInput.y);
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.started && !PauseMenu.instance._IsPauseBlocked)
        {
            UpdatePlayerAnim(new(0, 0));
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
    public void SetVisibility(bool visible, float delay)
    {
        this.Invoke(()=> { GetComponent<SpriteRenderer>().enabled = visible; StandInteractableTrigger.Map.SetActive(visible); },delay);
        
    }
    public void PlayWalkSound()
    {
        AudioManager._Instance.PlaySFX("playerWalk", true);
    }

    private IEnumerator WaitUntilEvent(UnityEvent unityEvent)
    {
        bool trigger = false;
        Action action = () => trigger = true;
        unityEvent.AddListener(action.Invoke);
        yield return new WaitUntil(() => trigger);
        unityEvent.RemoveListener(action.Invoke);
    }
}
