using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StandTransitionOut : MonoBehaviour
{
    [SerializeField] private DialogueTrigger _DialogueWindow;
    private readonly string[] _TranslationList = {"None"," m�daille de Bronze","m�daille d'argent", "m�daille d'or"};
    public static StandTransitionOut Instance;
    private void Start()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
    }

    public IEnumerator TransitionOut()
    {
        PlayerControls.Instance._CurrentDialogue = _DialogueWindow;
        _DialogueWindow._DialoguesTexts.Clear();
        if (FishManager.Instance.gameObject.activeSelf)
        {
            
            if (FishManager.Instance._StandResults._Medal != MedalType.None)
            {
                _DialogueWindow._DialoguesTexts.Add
                ("avec un score de " + FishManager.Instance._StandResults._Points + ", tu m�rites la" +
                _TranslationList[(int)FishManager.Instance._StandResults._Medal] +
                    ". f�licitations gar�on ! tu en auras besoin pour faire un cadeau � ta petite soeur." +
                    " Utilise l� au stand de r�compense plus tard.");
            }
            else
            {
                _DialogueWindow._DialoguesTexts.Add("Mince alors�" +
                    " Tu n�as pas p�ch� assez de poissons pour obtenir une m�daille� Peut-�tre devrais-tu r�essayer ?");
            }
        }
        else if (MoleWacker.Instance.gameObject.activeSelf)
        {
            if (MoleWacker.Instance._StandResults._Medal != MedalType.None)
            {
                _DialogueWindow._DialoguesTexts.Add
                    ("Wow, tu as battu le record du jour ! " + MoleWacker.Instance._StandResults._Points +
                    " taupes frapp�es. Tu m�rite amplement la " + _TranslationList[(int)MoleWacker.Instance._StandResults._Medal] +
                    " ! A la fin de ta journ�e, n'h�site pas � t'en servir au stand de r�compenses.");
            }
            else
            {
                _DialogueWindow._DialoguesTexts.Add
                    ("Dommage gar�on, mais tu n�as pas atteint le score minimal pour avoir une m�daille." +
                    " N�h�site pas � repasser pour tenter d�en gagner une.");
            }
        }
        else if (RifleMinigame.Instance.gameObject.activeSelf)
        {
            if (RifleMinigame.Instance._StandResults._Medal != MedalType.None)
            {
                _DialogueWindow._DialoguesTexts.Add(RifleMinigame.Instance._StandResults._Points +
                    " cibles abattue ?! voila ta " + _TranslationList[(int)RifleMinigame.Instance._StandResults._Medal] +
                    ". A croire que t'entraines quelque part. Elle te sera utile au stand r�compenses");
            }
            else
            {
                _DialogueWindow._DialoguesTexts.Add(RifleMinigame.Instance._StandResults._Points +
                    "cibles abattues. Ce n'est pas assez du tout. N'h�site pas � revenir quand tu te sens pr�t � retenter le coup." +
                    " Cette fois-ci tu obtiendras peut-�tre une m�daille");
            }
        }
        else if (Cups.Instance.gameObject.activeSelf)
        {
            if (Cups.Instance._StandResults._Medal != MedalType.None)
            {
                _DialogueWindow._DialoguesTexts.Add
                    ("tu as les yeux vif gamin, tu as trouv� " + Cups.Instance._StandResults._Points +
                    " balles. Viens donc r�cup�rer ta" + _TranslationList[(int)Cups.Instance._StandResults._Medal] +
                    ". Tu pourras l'�changer plus tard au stand de r�compenses.");
            }
            else
            {
                _DialogueWindow._DialoguesTexts.Add
                    ("Ce n�est pas possible d�avoir un score aussi bas� Je ne peux pas te donner de m�daille avec ce score." +
                    " Prochaine fois que tu repasses essaye de rester concentr� gamin, tu peux le faire.");
            }
        }
        yield return FadeInOut.Instance.FadeToBlack();
        _DialogueWindow.TriggerDialogue();
        yield return WaitUntilEvent(_DialogueWindow._TypeWriter.onTextDisappeared);
        PlayerControls.Instance._CurrentDialogue = null;
        yield return FadeInOut.Instance.FadeToBlack();
        PlayerControls.Instance._PlayerInput.SwitchCurrentActionMap("Player");
        StandInteractableTrigger.Map.SetActive(true);

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
