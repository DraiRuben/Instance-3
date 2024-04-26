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
                OutDialogue._Texts.Add("avec un score de " + Score + ", tu mérites la" + Medal +
                    ". félicitations garçon ! tu en auras besoin pour faire un cadeau à ta petite soeur. Utilise là au stand de récompense plus tard.");
            }
            else
            {

            }
        }

    }
}
