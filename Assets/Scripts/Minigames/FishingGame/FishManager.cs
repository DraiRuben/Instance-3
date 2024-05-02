using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public sealed class FishManager : Minigame
{
    public static FishManager Instance;
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI _ScoreText;
    [SerializeField] private TextMeshProUGUI _TimerText;
    [SerializeField] private GameObject _Fish;
    [SerializeField] private PoleManager _PoleManager;
    [SerializeField] private SplineContainer[] _Splines;

    [Header("Params")]
    [SerializeField] private AnimationCurve _SpeedCurve;
    [SerializeField] private float _MaxConcurrentFishCount;
    [SerializeField] private float _FishSpawnFrequency;

    [System.NonSerialized] public float _SpeedMult = 1;
    [System.NonSerialized] public List<GameObject> _FishList = new List<GameObject>();
    [System.NonSerialized] public int _BugValue;
    [System.NonSerialized] public int _FishingScore;
    [System.NonSerialized] public float _ElapsedTime;

    private Vector3 _InitialOffset;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        _InitialOffset = transform.position - Camera.main.transform.position;
        _InitialOffset.z = 0;
        if (File.Exists(Application.persistentDataPath + "/FishSaveFile.json"))
        {
            JsonDataService FishSaveData = new JsonDataService();
            _StandResults = FishSaveData.LoadData<StandResults>("FishSaveFile");
        }
    }
    private void Start()
    {
        MakeFakeGameFiles();
        gameObject.SetActive(false);

    }
    protected override void MakeFakeGameFiles()
    {
        base.MakeFakeGameFiles();
        if (!Directory.Exists("Game/Minigames/FishingGame")) Directory.CreateDirectory("Game/Minigames/FishingGame");
    }
    protected override bool IsBugged()
    {
        if (!File.Exists("Game/Minigames/FishingGame/FishBehavior.txt"))
        {
            FileStream file = File.Create("Game/Minigames/FishingGame/FishBehavior.txt");
            StreamWriter writer = new StreamWriter(file);
            writer.Write("Enabled = false;");
            writer.Close();
            _BugValue = 1;
        }
        else
        {
            StreamReader reader = new StreamReader("Game/Minigames/FishingGame/FishBehavior.txt");
            _BugValue = reader.ReadLine() == "Enabled = true;" ? 0 : 1;
            reader.Close();
        }
        return _BugValue == 1;
    }
    private IEnumerator FishSpawn()
    {
        _ElapsedTime = 0f;
        float spawnTimer = 0f;
        while (_ElapsedTime < _MinigameDuration)
        {
            if (_FishList.Count < _MaxConcurrentFishCount && spawnTimer >= _FishSpawnFrequency)
            {
                _FishList.Add(Instantiate(_Fish, _Splines[_BugValue].EvaluatePosition(0), Quaternion.identity, transform));
                _FishList[_FishList.Count - 1].transform.GetChild(0).GetComponent<Fish>()._Spline = _Splines;
                spawnTimer = 0f;
            }
            _SpeedMult = _SpeedCurve.Evaluate(_ElapsedTime / _MinigameDuration);
            spawnTimer += Time.deltaTime;
            _ScoreText.text = _FishingScore.ToString();
            _TimerText.text = "Time : " + Mathf.RoundToInt(_MinigameDuration - _ElapsedTime);
            _ElapsedTime += Time.deltaTime;
            yield return null;
        }
        TriggerMinigameEnd();

    }
    private void OnApplicationQuit()
    {
        SaveStats();
    }
    public override void TriggerMinigameEnd(bool ClosePreEmptively = false)
    {
        base.TriggerMinigameEnd(ClosePreEmptively);
        _PoleManager._CurrentFishingCount = 0;
        foreach (GameObject fish in _FishList)
        {
            Destroy(fish);
        }
        _FishList.Clear();
    }
    protected override void SaveStats()
    {
        JsonDataService FishSaveData = new JsonDataService();

        MedalType Medal = MedalType.None;
        if (_FishingScore >= _MedalRequirements.MinRequiredForMedal[MedalType.Gold])
            Medal = MedalType.Gold;
        else if(_FishingScore >= _MedalRequirements.MinRequiredForMedal[MedalType.Silver])
            Medal = MedalType.Silver;
        else if(_FishingScore >= _MedalRequirements.MinRequiredForMedal[MedalType.Bronze])
            Medal = MedalType.Bronze;

        _StandResults = new StandResults(Medal, _FishingScore);
        FishSaveData.SaveData("FishSaveFile", _StandResults);
    }
    [Button]
    public override void Interact()
    {
        if (CanInteract())
        {
            IsBugged();
            transform.position = Utility.GetWorldScreenCenterPos() + _InitialOffset;

            PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.SetActive(true);
            StartCoroutine(FishSpawn());
        }
    }
}
