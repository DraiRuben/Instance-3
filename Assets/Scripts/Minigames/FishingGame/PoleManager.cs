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
    private int _FishingScore;
    private float _FishingTimer;


    private void Start()
    {
        if (File.Exists(Application.persistentDataPath + "/FishSaveFile.json"))
        {
            JsonDataService FishSaveData = new JsonDataService();
            FishManager.Instance._FishResults = FishSaveData.LoadData<StandResults>("FishSaveFile");
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

    private void FishSaving()
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
        FishManager.Instance._FishResults = new StandResults(FishMedal, _FishingScore);
        FishSaveData.SaveData("FishSaveFile", FishManager.Instance._FishResults);
    }

    private void Update()
    {
        _FishingTimer += Time.deltaTime;
        _FishingScoreText.text = "score : " + _FishingScore;
        _FishingTimerText.text = "time : " + Mathf.RoundToInt(30 - _FishingTimer);
        if (_FishingTimer > 30)
        {
            FishSaving();
            Debug.Log("game end");
        }
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