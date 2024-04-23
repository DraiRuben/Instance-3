using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class PrizeStall : MonoBehaviour
{
    private int _FinalScore;

    private void CalculateScore()
    {
        switch(MoleWacker.Instance._StandResults._Medal)
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
        CalculateScore();
        if(_FinalScore >= 160)
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
            Debug.Log("not enough points");
        }
    }
}
