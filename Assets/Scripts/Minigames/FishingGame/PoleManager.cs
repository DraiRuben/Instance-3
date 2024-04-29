using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PoleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _FishingScoreText;
    [SerializeField] private TextMeshProUGUI _FishingTimerText;
    [SerializeField] private float _FishingCooldown;
    private bool _Fishing;
    public int _FishingScore;
    [System.NonSerialized] public float _FishingTimer;
    private Animator _Animator;
    private float _LastFishTime;
    private UnityEvent OnHookDown = new();
    private UnityEvent OnHookReel = new();

    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/FishSaveFile.json"))
        {
            JsonDataService FishSaveData = new JsonDataService();
            FishManager.Instance._StandResults = FishSaveData.LoadData<StandResults>("FishSaveFile");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (_Fishing)
        {
            Debug.Log(other.gameObject.name + " enter");
            if (other.CompareTag("Fish"))
            {
                AudioManager._Instance.PlaySFX("fishCatch");
                FishManager.Instance._FishList.Remove(other.gameObject);
                _FishingScore++;
                _Fishing = false;
                other.gameObject.GetComponent<Fish>().DoDestructionFeedback();
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {

        if (_Fishing)
        {
            Debug.Log(other.gameObject.name + " stay");

            if (other.CompareTag("Fish"))
            {
                FishManager.Instance._FishList.Remove(other.gameObject);
                _FishingScore++;
                _Fishing = false;
                other.gameObject.GetComponent<Fish>().DoDestructionFeedback();
            }
        }
    }
    public void SaveStats()
    {
        JsonDataService FishSaveData = new JsonDataService();
        MedalType FishMedal;
        if (_FishingScore >= 12)
        {
            FishMedal = MedalType.Gold;
        }
        else if (_FishingScore >= 8)
        {
            FishMedal = MedalType.Silver;
        }
        else if (_FishingScore >= 4)
        {
            FishMedal = MedalType.Bronze;
        }
        else
        {
            FishMedal = MedalType.None;
        }
        FishManager.Instance._StandResults = new StandResults(FishMedal, _FishingScore);
        FishSaveData.SaveData("FishSaveFile", FishManager.Instance._StandResults);
    }

    private void Update()
    {
        _FishingTimer += Time.deltaTime;
        _FishingScoreText.text = _FishingScore.ToString();
        _FishingTimerText.text = "Time : " + Mathf.RoundToInt(FishManager.Instance._MinigameDuration - _FishingTimer);

        
    }
    public void Fishing(InputAction.CallbackContext context)
    {
        if (context.started && !_Fishing)
        {
            if (Time.time - _LastFishTime > _FishingCooldown)
            {
                _LastFishTime = Time.time;
                StartCoroutine(Tofish());
            }
        }
    }
    public void StartDownTime()
    {
        OnHookDown.Invoke();
    }
    public void StartReeling()
    {
        OnHookReel.Invoke();
        AudioManager._Instance.PlaySFX("fishFail");
    }
    IEnumerator Tofish()
    {
        //time for hook to be down
        _Animator.SetTrigger("Reel");
        yield return WaitUntilEvent(OnHookDown);
        _Fishing = true;
        yield return WaitUntilEvent(OnHookReel);
        _Fishing = false;
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
