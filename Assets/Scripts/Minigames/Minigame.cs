using System.Collections;
using System.IO;
using UnityEngine;

public class Minigame : MonoBehaviour, IInteractable
{
    public StandResults _StandResults;
    public float _MinigameDuration;
    [SerializeField] protected MedalRequirements _MedalRequirements;
    [SerializeField] protected Sprite _PointsImage;
    public bool _DisplayGuy;
    [System.NonSerialized] public int _Points;
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
        PlayerControls.Instance?.OnSelect.RemoveAllListeners();
        RequiredMedalsDisplay.Instance.StopDisplay();
        if (!ClosePreEmptively)
        {
            SaveStats();

            StandTransitionOut.Instance._DialogueWindow.SetGuyVisibility(_DisplayGuy);
            StandTransitionOut.Instance.StartCoroutine(StandTransitionOut.Instance.TransitionOut());
        }
        else
        {
            FadeInOut.Instance.StartCoroutine(FadeInOut.Instance.FadeToBlackThenTransparent());
            PlayerControls.Instance.SetVisibility(true, 0.35f / 0.60f);
            PlayerControls.Instance._PlayerInput.SwitchCurrentActionMap("Player");
            _Points = 0;
        }
        this.Invoke(()=> { gameObject.SetActive(false); Cursor.visible = true; },0.35f /0.6f);
    }
}
