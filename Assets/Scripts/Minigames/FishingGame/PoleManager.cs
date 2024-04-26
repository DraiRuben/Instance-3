using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _FishingScoreText;
    [SerializeField] private TextMeshProUGUI _FishingTimerText;

    private bool _Fishing;
    public int _FishingScore;
    [System.NonSerialized] public float _FishingTimer;
    private Animator _Animator;
    private float _LastFishTime;


    private void Awake()
    {
        if (File.Exists(Application.persistentDataPath + "/FishSaveFile.json"))
        {
            JsonDataService FishSaveData = new JsonDataService();
            FishManager.Instance._StandResults = FishSaveData.LoadData<StandResults>("FishSaveFile");
        }
        _Animator = GetComponent<Animator>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_Fishing)
        {
            if (other.CompareTag("Fish"))
            {
                FishManager.Instance._FishList.Remove(other.gameObject);
                _FishingScore++;
                _Fishing = false;
                Destroy(other.gameObject);
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
            if (Time.time - _LastFishTime > 1f)
            {
                _LastFishTime = Time.time;
                StartCoroutine(Tofish());
            }
        }
    }

    IEnumerator Tofish()
    {
        _Fishing = true;
        _Animator.SetTrigger("Reel");
        yield return new WaitForSeconds(1f);
        _Fishing = false;
    }
}
