using Febucci.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(TextAnimator_TMP))]
[RequireComponent(typeof(TypewriterByCharacter))]
public class DialogueTrigger : MonoBehaviour
{
    private TypewriterByCharacter _TypeWriter;
    private TextMeshProUGUI _TMP;
    [SerializeField] private DialogueSO _DialogueData;

    private bool _TextFullyDisplayed;
    private int _CurrentTextIndex;
    // Start is called before the first frame update
    void Start()
    {
        _TypeWriter = GetComponent<TypewriterByCharacter>();
        _TMP = GetComponent<TextMeshProUGUI>();
        _TypeWriter.onTextShowed.AddListener(() => _TextFullyDisplayed = true);
    }

    [Button]
    public void TriggerDialogue()
    {
        //need to change input map to prevent player from moving
        if(_CurrentTextIndex<_DialogueData._Texts.Count)
        _TypeWriter.ShowText(_DialogueData._Texts[_CurrentTextIndex++]);
    }
    [Button]
    public void SkipDialogue()
    {
        if (_TextFullyDisplayed)
        {
            if(_CurrentTextIndex< _DialogueData._Texts.Count)
            {
                //show next text
                _TypeWriter.ShowText(_DialogueData._Texts[_CurrentTextIndex++]);
                _TextFullyDisplayed = false;
            }
            else
            {
                //close dialogue
                _TypeWriter.StartDisappearingText();
            }
        }
        else
        {
            _TypeWriter.SkipTypewriter();
        }
    }
}
