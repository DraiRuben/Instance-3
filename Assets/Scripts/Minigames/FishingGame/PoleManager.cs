using System.Collections;
using System.IO;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PoleManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _FishingScoreText;
    [SerializeField] private TextMeshProUGUI _FishingTimerText;
    [SerializeField] private AnimationCurve _FishingAnim;

    private bool _Fishing;
    private int _FishingScore;
    private float _FishingTimer;
    private Vector3 HookStartPos;


    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/FishSaveFile.json"))
        {
            JsonDataService FishSaveData = new JsonDataService();
            FishManager.Instance._StandResults = FishSaveData.LoadData<StandResults>("FishSaveFile");
            HookStartPos = transform.position;
        }
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
        _FishingScoreText.text = "Score : " + _FishingScore;
        _FishingTimerText.text = "Time : " + Mathf.RoundToInt(FishManager.Instance._MinigameDuration - _FishingTimer);

        
    }
    public void Fishing(InputAction.CallbackContext context)
    {
        if (context.started && !_Fishing)
        {
            StartCoroutine(Tofish());
        }
    }

    IEnumerator Tofish()
    {
        _Fishing = true;
        yield return new WaitForSeconds(0.5f);
        _Fishing = false;
    }
}
