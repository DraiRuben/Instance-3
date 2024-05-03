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
    [NonSerialized] public TypewriterByCharacter _TypeWriter;
    private TextMeshProUGUI _TMP;
    [SerializeField] private DialogueSO _DialogueData;
    public List<string> _DialoguesTexts;
    [SerializeField] private GameObject _Minigame;
    private bool _TextFullyDisplayed;
    private int _CurrentTextIndex;
    private bool _IsClosing;
    [SerializeField] private bool _EnableMapOnClose;
    private List<string> _UsedDialogues;
    public Sprite _Image;
    // Start is called before the first frame update
    void Awake()
    {
        _TypeWriter = GetComponent<TypewriterByCharacter>();
        _TMP = GetComponent<TextMeshProUGUI>();
        _TypeWriter.onTextShowed.AddListener(() => _TextFullyDisplayed = true);   
    }

    private void Start()
    {
        transform.parent.parent.gameObject.SetActive(false);
    }
    [Button]
    public void TriggerDialogue()
    {
        AssignImage();
        transform.parent.parent.gameObject.SetActive(true);
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

    private void AssignImage()
    {
        if (_Image)
        {
            transform.parent.parent.GetChild(0).GetComponent<Image>().color = Color.white;
            transform.parent.parent.GetChild(0).GetComponent<Image>().sprite = _Image;
        }
        else
        {
            transform.parent.parent.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
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
        yield return FadeInOut.Instance.FadeToBlack();
        if (_Minigame) _Minigame.GetComponent<IInteractable>().Interact();
        if (PlayerControls.Instance._CurrentDialogue == this) PlayerControls.Instance._CurrentDialogue = null;
        if (_EnableMapOnClose)
        {
            PlayerControls.Instance.SetVisibility(true, 0.0f);
            PlayerControls.Instance._PlayerInput.SwitchCurrentActionMap("Player");
        }
        FadeInOut.Instance.StartCoroutine(FadeInOut.Instance.FadeToTransparent());
        transform.parent.parent.gameObject.SetActive(false);
    }
    public bool CanInteract()
    {
        return _Minigame.GetComponent<IInteractable>().CanInteract();
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
