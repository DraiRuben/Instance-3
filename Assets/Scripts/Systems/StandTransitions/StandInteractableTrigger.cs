using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandInteractableTrigger : MonoBehaviour, IInteractable
{
    public static GameObject Map;
    public DialogueTrigger _Dialogue;
    public GameObject _Minigame;
    private void Start()
    {
        if (!Map) Map = transform.parent.parent.gameObject;
    }
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
