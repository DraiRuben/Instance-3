using Febucci.UI;
using Sirenix.OdinInspector;
using System;
using System.Collections;
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
    [SerializeField] private GameObject _Minigame;
    private bool _TextFullyDisplayed;
    private int _CurrentTextIndex;
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
        if (_CurrentTextIndex < _DialogueData._Texts.Count)
        {
            transform.parent.GetComponent<Image>().enabled = true;
            _TypeWriter.ShowText(_DialogueData._Texts[_CurrentTextIndex++]);
        }
    }
    [Button]
    public void SkipDialogue()
    {
        if (transform.parent.gameObject.activeSelf)
        {
            if (_TextFullyDisplayed)
            {
                if (_CurrentTextIndex < _DialogueData._Texts.Count)
                {
                    //show next text
                    _TypeWriter.ShowText(_DialogueData._Texts[_CurrentTextIndex++]);
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
        _TypeWriter.StartDisappearingText(); 
        StandInteractableTrigger.Map.SetActive(false);
        yield return WaitUntilEvent(_TypeWriter.onTextDisappeared);
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
