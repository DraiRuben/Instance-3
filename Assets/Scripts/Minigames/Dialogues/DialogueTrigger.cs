using Febucci.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(TextAnimator_TMP))]
[RequireComponent(typeof(TypewriterByCharacter))]
public class DialogueTrigger : MonoBehaviour
{
    private TypewriterByCharacter _TypeWriter;
    private TextMeshProUGUI _TMP;
    [SerializeField] private DialogueSO _DialogueData;
    public List<string> _DialoguesTexts;
    [SerializeField] private GameObject _Minigame;
    private bool _TextFullyDisplayed;
    private int _CurrentTextIndex;
    private bool _IsClosing;
    private List<string> _UsedDialogues;
    // Start is called before the first frame update
    void Start()
    {
        _TypeWriter = GetComponent<TypewriterByCharacter>();
        _TMP = GetComponent<TextMeshProUGUI>();
        _TypeWriter.onTextShowed.AddListener(() => _TextFullyDisplayed = true);
        transform.parent.GetComponent<Image>().enabled = false;
    }

    [Button]
    public void TriggerDialogue()
    {
        //need to change input map to prevent player from moving
        if (_DialogueData)
        {
            _UsedDialogues = _DialogueData._Texts;
        }
        else if (_DialoguesTexts != null)
        {
            _UsedDialogues = _DialoguesTexts;
        }
        else return;

        if (_CurrentTextIndex < _UsedDialogues.Count)
        {
            _IsClosing = false;
            transform.parent.GetComponent<Image>().enabled = true;
            _TypeWriter.ShowText(_UsedDialogues[_CurrentTextIndex++]);
        }
    }
    [Button]
    public void SkipDialogue()
    {
        if (!_IsClosing)
        {
            if (_TextFullyDisplayed)
            {
                if (_CurrentTextIndex < _UsedDialogues.Count)
                {
                    //show next text
                    _TypeWriter.ShowText(_UsedDialogues[_CurrentTextIndex++]);
                    _TextFullyDisplayed = false;
                }
                else
                {
                    //close dialogue
                    StartCoroutine(CloseRoutine());
                }
            }
            else
            {
                _TypeWriter.SkipTypewriter();
            }
        } 
    }
    private IEnumerator CloseRoutine()
    {
        //fade text out, then fade screen out, then start minigame
        _IsClosing = true;
        _TextFullyDisplayed = false;
        _TypeWriter.StartDisappearingText();
        _CurrentTextIndex = 0;
        StandInteractableTrigger.Map.SetActive(false);
        yield return WaitUntilEvent(_TypeWriter.onTextDisappeared);
        yield return new WaitForSeconds(0.5f);
        transform.parent.GetComponent<Image>().enabled = false;
        yield return FadeInOut.Instance.FadeToBlack();
        _Minigame.GetComponent<IInteractable>().Interact();
    }
    public bool CanInteract()
    {
        return _Minigame.GetComponent<IInteractable>().CanInteract();
    }
    private IEnumerator WaitUntilEvent(UnityEvent unityEvent)
    {
        var trigger = false;
        Action action = () => trigger = true;
        unityEvent.AddListener(action.Invoke);
        yield return new WaitUntil(() => trigger);
        unityEvent.RemoveListener(action.Invoke);
    }
}
