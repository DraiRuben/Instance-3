using Febucci.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PrizeStall : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueTrigger _DialogueWindow;
    [SerializeField] private ParticleSystem _ConfettiEffect;
    [SerializeField] private GameObject _BearPlush;
    [SerializeField] private GameObject _RabbitPlush;
    [SerializeField] private GameObject _RatPlush;
    private int _FinalScore;
    [SerializeField] private TypewriterByCharacter _TypeWriter;
    private Vector3 _InitialOffset;

    private void Awake()
    {
        _InitialOffset = transform.position - Camera.main.transform.position;
        _InitialOffset.z = 0;
    }
    private void Start()
    {
        _TypeWriter.onTextDisappeared.AddListener(() => this.Invoke(() => SceneManager.LoadSceneAsync(0), 3f));
        gameObject.SetActive(false);
    }
    private void CalculateScore(MedalType medal)
    {
        switch (medal)
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
        PlayerControls.Instance._CurrentDialogue = _DialogueWindow;
        CalculateScore(MoleWacker.Instance._StandResults._Medal);
        CalculateScore(Cups.Instance._StandResults._Medal);
        CalculateScore(RifleMinigame.Instance._StandResults._Medal);
        CalculateScore(FishManager.Instance._StandResults._Medal);
        if (_FinalScore >= 160)
        {
            _ConfettiEffect.transform.position = _BearPlush.transform.position;
            _ConfettiEffect.Play();
            _DialogueWindow._DialoguesTexts.Add("Wow avec autant de tickets tu peux avoir cet ours en peluche");
        }
        else if (_FinalScore >= 80)
        {
            _ConfettiEffect.transform.position = _RabbitPlush.transform.position;
            _ConfettiEffect.Play();
            _DialogueWindow._DialoguesTexts.Add("Avec ton nombre de tickets je peux te proposer ce lapin en peluche");
        }
        else if (_FinalScore >= 35)
        {
            _ConfettiEffect.transform.position = _RatPlush.transform.position;
            _ConfettiEffect.Play();
            _DialogueWindow._DialoguesTexts.Add("Avec si peu de tickets tu peux avoir cette peluche de rat");
        }
        else
        {
            _DialogueWindow._DialoguesTexts.Add("Je suis d�sol� mais tu n'as passez de tickets pour avoir quoi que ce soit");
        }
        _DialogueWindow.TriggerDialogue();
        _FinalScore = 0;
    }

    public bool CanInteract()
    {
        return true;
    }

    [Button]
    public void Interact()
    {
        PlayerControls.Instance.SetVisibility(false, 0.0f);
        transform.position = Utility.GetWorldScreenCenterPos() + _InitialOffset;
        gameObject.SetActive(true);
        StandInteractableTrigger.Map.SetActive(false);
        getReward();
    }
}
