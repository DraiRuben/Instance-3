using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPrompt : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ConfirmationText;
    [SerializeField] private Button _ConfirmButton;
    [SerializeField] private Button _DenyButton;

    private Animator _Animator;
    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    public void OpenConfirmationPrompt(
        UnityEngine.Events.UnityAction confirmAction, 
        UnityEngine.Events.UnityAction denyAction, 
        string confirmationText)
    {
        gameObject.SetActive(true);
        _ConfirmButton.onClick.RemoveAllListeners();
        _DenyButton.onClick.RemoveAllListeners();
        _ConfirmButton.onClick.AddListener(confirmAction);
        _DenyButton.onClick.AddListener(denyAction);
        _ConfirmationText.text = confirmationText;
        ChangePromptState();
    }
    public void ChangePromptState()
    {
        _Animator.SetTrigger("Appear");

    }

}
