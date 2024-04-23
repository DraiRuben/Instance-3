using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandInteractableTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueTrigger _Dialogue;
    [SerializeField] private GameObject _Minigame;
    public void Interact()
    {
        if (CanInteract())
        {
            StartCoroutine(StandInteract());
        }
    }
    public bool CanInteract()
    {
        if (_Dialogue)
        {
            return _Dialogue.CanInteract();
        }
        else
        {
            return _Minigame.GetComponent<IInteractable>().CanInteract();
        }
        
    }
    private IEnumerator StandInteract()
    {
        yield return FadeInOut.Instance.FadeToBlack();
        if (_Dialogue)
        {
            _Dialogue.TriggerDialogue();
        }
        else
        {
            _Minigame.GetComponent<IInteractable>().Interact();
        }
    }
}
