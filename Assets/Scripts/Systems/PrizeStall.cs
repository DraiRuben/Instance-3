using Febucci.UI;
using Sirenix.OdinInspector;
using UnityEngine;

public class PrizeStall : MonoBehaviour,IInteractable
{
    private int _FinalScore;
    private TypewriterByCharacter _TypeWriter;

    private void CalculateScore(MedalType medal)
    {
        switch(medal)
        {
            case MedalType.Gold:
                {
                    _FinalScore += 40;
                    break;
                }
            case MedalType.Silver:
                {
                    _FinalScore += 20;
                    break;
                }
            case MedalType.Bronze:
                {
                    _FinalScore += 5;
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public void getReward()
    {
        CalculateScore(MoleWacker.Instance._StandResults._Medal);
        CalculateScore(Cups.Instance._StandResults._Medal);
        CalculateScore(RifleMinigame.Instance._StandResults._Medal);
        CalculateScore(FishManager.Instance._StandResults._Medal);
        if (_FinalScore >= 160)
        {
            _TypeWriter.ShowText("wow avec autant de ticket tu peu avoir cette ours en peluche");
            Debug.Log("you got the bear");
        }
        else if(_FinalScore >= 80)
        {
            _TypeWriter.ShowText("avec ton nombre de ticket je peu te proposer ce lapin en peluche");
            Debug.Log("you got the rabbit");
        }
        else if(_FinalScore >=35)
        {
            _TypeWriter.ShowText("avec si peu de ticket tu peu avoir cette peluche de rat");
            Debug.Log("you got the rat");
        }
        else
        {
            _TypeWriter.ShowText("je suis désolé mais tu n'as passez de ticket pour avoir quoi ce soit");
            Debug.Log("you can't get anything");
        }
    }

    public bool CanInteract()
    {
        return true;
    }

    [Button]
    public void Interact()
    {
            PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.SetActive(true);
            StandInteractableTrigger.Map.SetActive(false);
            getReward();
    }
}
