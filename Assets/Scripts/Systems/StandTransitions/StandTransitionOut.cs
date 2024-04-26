using System.Collections;
using UnityEngine;

public class StandTransitionOut : MonoBehaviour
{
    DialogueSO OutDialogue = new DialogueSO();
    public IEnumerator TransitionOut(int Score, string Medal, GameObject Minigame)
    {
        yield return FadeInOut.Instance.FadeToBlack();
        if (Minigame.name == "FishingGame")
        {
            if (FishManager.Instance._StandResults._Medal != MedalType.None)
            {
                OutDialogue._Texts.Add("avec un score de " + Score + ", tu m�rites la" + Medal +
                    ". f�licitations gar�on ! tu en auras besoin pour faire un cadeau � ta petite soeur. Utilise l� au stand de r�compense plus tard.");
            }
            else
            {

            }
        }

    }
}
