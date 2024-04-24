using Febucci.UI;
using Febucci.UI.Actions;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrizeStall : MonoBehaviour,IInteractable
{
    private int _FinalScore;
    private TypewriterByCharacter _TypeWriter;
    private bool _TextFullyDisplayed;


    private void Start()
    {
        _TypeWriter = GetComponent<TypewriterByCharacter>();
        _TypeWriter.onTextShowed.AddListener(() => _TextFullyDisplayed = true);

    }
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

    private void Update()
    {
        if (_TextFullyDisplayed)
        {
            StartCoroutine(Delay());
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadSceneAsync(0);
    }

    public void getReward()
    {
        CalculateScore(MoleWacker.Instance._StandResults._Medal);
        CalculateScore(Cups.Instance._StandResults._Medal);
        CalculateScore(RifleMinigame.Instance._StandResults._Medal);
        CalculateScore(FishManager.Instance._StandResults._Medal);
        if (_FinalScore >= 160)
        {
            _TypeWriter.ShowText("Wow avec autant de tickets tu peux avoir cette ours en peluche");
            Debug.Log("you got the bear");
        }
        else if(_FinalScore >= 80)
        {
            _TypeWriter.ShowText("Avec ton nombre de tickets je peux te proposer ce lapin en peluche");
            Debug.Log("you got the rabbit");
        }
        else if(_FinalScore >=35)
        {
            _TypeWriter.ShowText("Avec si peu de ticket tu peux avoir cette peluche de rat");
            Debug.Log("you got the rat");
        }
        else
        {
            _TypeWriter.ShowText("Je suis désolé mais tu n'as passez de tickets pour avoir quoi que ce soit");
            Debug.Log("you can't get anything");
        }
        _FinalScore = 0;
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
