using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StandTransitionOut : MonoBehaviour
{
    [SerializeField] private DialogueTrigger _DialogueWindow;
    private readonly string[] _TranslationList = {"None"," médaille de Bronze","médaille d'argent", "médaille d'or"};
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
                ("avec un score de " + FishManager.Instance._StandResults._Points + ", tu mérites la" +
                _TranslationList[(int)FishManager.Instance._StandResults._Medal] +
                    ". félicitations garçon ! tu en auras besoin pour faire un cadeau à ta petite soeur." +
                    " Utilise là au stand de récompense plus tard.");
            }
            else
            {
                _DialogueWindow._DialoguesTexts.Add("Mince alors…" +
                    " Tu n’as pas pêché assez de poissons pour obtenir une médaille… Peut-être devrais-tu réessayer ?");
            }
        }
        else if (MoleWacker.Instance.gameObject.activeSelf)
        {
            if (MoleWacker.Instance._StandResults._Medal != MedalType.None)
            {
                _DialogueWindow._DialoguesTexts.Add
                    ("Wow, tu as battu le record du jour ! " + MoleWacker.Instance._StandResults._Points +
                    " taupes frappées. Tu mérite amplement la " + _TranslationList[(int)MoleWacker.Instance._StandResults._Medal] +
                    " ! A la fin de ta journée, n'hésite pas à t'en servir au stand de récompenses.");
            }
            else
            {
                _DialogueWindow._DialoguesTexts.Add
                    ("Dommage garçon, mais tu n’as pas atteint le score minimal pour avoir une médaille." +
                    " N’hésite pas à repasser pour tenter d’en gagner une.");
            }
        }
        else if (RifleMinigame.Instance.gameObject.activeSelf)
        {
            if (RifleMinigame.Instance._StandResults._Medal != MedalType.None)
            {
                _DialogueWindow._DialoguesTexts.Add(RifleMinigame.Instance._StandResults._Points +
                    " cibles abattue ?! voila ta " + _TranslationList[(int)RifleMinigame.Instance._StandResults._Medal] +
                    ". A croire que t'entraines quelque part. Elle te sera utile au stand récompenses");
            }
            else
            {
                _DialogueWindow._DialoguesTexts.Add(RifleMinigame.Instance._StandResults._Points +
                    "cibles abattues. Ce n'est pas assez du tout. N'hésite pas à revenir quand tu te sens prêt à retenter le coup." +
                    " Cette fois-ci tu obtiendras peut-être une médaille");
            }
        }
        else if (Cups.Instance.gameObject.activeSelf)
        {
            if (Cups.Instance._StandResults._Medal != MedalType.None)
            {
                _DialogueWindow._DialoguesTexts.Add
                    ("tu as les yeux vif gamin, tu as trouvé " + Cups.Instance._StandResults._Points +
                    " balles. Viens donc récupérer ta" + _TranslationList[(int)Cups.Instance._StandResults._Medal] +
                    ". Tu pourras l'échanger plus tard au stand de récompenses.");
            }
            else
            {
                _DialogueWindow._DialoguesTexts.Add
                    ("Ce n’est pas possible d’avoir un score aussi bas… Je ne peux pas te donner de médaille avec ce score." +
                    " Prochaine fois que tu repasses essaye de rester concentré gamin, tu peux le faire.");
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
