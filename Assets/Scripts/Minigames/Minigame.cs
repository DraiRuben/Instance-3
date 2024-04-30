using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Minigame : MonoBehaviour, IInteractable
{
    public StandResults _StandResults;
    public float _MinigameDuration;
    protected virtual bool IsBugged()
    {
        return true;
    }
    protected virtual void MakeFakeGameFiles()
    {
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Minigames")) Directory.CreateDirectory("Game/Minigames");
    }
    public virtual bool CanInteract()
    {
        return _StandResults._Medal == MedalType.None;
    }
    protected virtual void SaveStats() { }
    public virtual void Interact()
    {
        throw new System.NotImplementedException();
    }

    public virtual void TriggerMinigameEnd(bool ClosePreEmptively = false) 
    {
        StopAllCoroutines();
        Cursor.visible = true;

        if (!ClosePreEmptively)
        {
            SaveStats();
            StandTransitionOut.Instance.StartCoroutine(StandTransitionOut.Instance.TransitionOut());
            StandInteractableTrigger.Map.SetActive(true);
            PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            FadeInOut.Instance.StartCoroutine(fadeinout());
            PlayerControls.Instance._PlayerInput.SwitchCurrentActionMap("Player");
        }
        gameObject.SetActive(false);
        //TODO: Maybe change how minigame end is done so that we have a fade in and out of minigame instead of instant deactivation
    }

    private IEnumerator fadeinout()
    {
        yield return FadeInOut.Instance.FadeToBlack();
        StandInteractableTrigger.Map.SetActive(true);
        PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = true;
        yield return FadeInOut.Instance.FadeToBlack();
    }
}
