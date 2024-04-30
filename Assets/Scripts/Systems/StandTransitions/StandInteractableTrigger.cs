using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class StandInteractableTrigger : MonoBehaviour, IInteractable
{
    public static GameObject Map;
    public DialogueTrigger _Dialogue;
    [SerializeField] private DialogueTrigger _FailDialogue;
    public DialogueTrigger _CurrentDialogue;
    public GameObject _Minigame;

    [SerializeField] private bool _OpenPromptBefore;
    [ShowIf(nameof(_OpenPromptBefore)), SerializeField] private ConfirmationPrompt _ConfirmationPrompt;
    [ShowIf(nameof(_OpenPromptBefore)), SerializeField, TextArea] private string _PromptDescription;
    private void Start()
    {
        if (!Map) Map = transform.parent.parent.gameObject;
        _CurrentDialogue = _Dialogue;

    }
    public void Interact()
    {
        _CurrentDialogue = _Dialogue;
        if (CanInteract())
        {
            if(_OpenPromptBefore)
            {
                _ConfirmationPrompt.OpenConfirmationPrompt(
                    () =>
                    {
                        _ConfirmationPrompt.ChangePromptState();
                        StartCoroutine(StandInteract());
                    }, 
                    () => {
                        _ConfirmationPrompt.ChangePromptState();
                        PlayerControls.Instance._PlayerInput.SwitchCurrentActionMap("Player");

                    }, 
                    _PromptDescription);
            }
            else
            {
                StartCoroutine(StandInteract());
            }
        }
        else
        {
            _CurrentDialogue = _FailDialogue;
            StartCoroutine(StandInteract());
        }
    }
    public bool CanInteract()
    {
        if (_CurrentDialogue)
        {
            return _CurrentDialogue.CanInteract();
        }
        else
        {
            return _Minigame.GetComponent<IInteractable>().CanInteract();
        }
        
    }

    
    private IEnumerator StandInteract()
    {
        yield return FadeInOut.Instance.FadeToBlack();
        if (_CurrentDialogue)
        {
            _CurrentDialogue.TriggerDialogue();
        }
        else
        {
            _Minigame.GetComponent<IInteractable>().Interact();
        }
    }
}
