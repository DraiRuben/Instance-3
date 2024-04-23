using Sirenix.OdinInspector;
using UnityEngine;

public class PrizeStall : MonoBehaviour,IInteractable
{
    private int _FinalScore;

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
            Debug.Log("you got the bear");
        }
        else if(_FinalScore >= 80)
        {
            Debug.Log("you got the rabbit");
        }
        else if(_FinalScore >=35)
        {
            Debug.Log("you got the rat");
        }
        else
        {
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
