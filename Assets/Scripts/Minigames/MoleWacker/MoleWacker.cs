using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MoleWacker : Minigame
{
    public static MoleWacker Instance;
    [System.NonSerialized] public UnityEvent OnMoleWacked = new();
    [System.NonSerialized] public UnityEvent OnMoleLost = new();
    [Header("Gameplay Stats")]
    public int _WinCount = 0;
    public int _LoseCount = 0;

    [Header("Refs")]
    [SerializeField] private GameObject _MolePrefab;
    [SerializeField] private GameObject _GoldenMolePrefab;
    [SerializeField] private GameObject _BugMessagePrefab;

    [Header("Spawn Interval")]
    [SerializeField] private AnimationCurve _SpawnCooldownEvolution;
    [SerializeField] private float _SpawnBaseCooldown;

    [Header("Mole Persistence time")]
    [SerializeField] private AnimationCurve _MoleStayTimeEvolution;
    [SerializeField] private float _MoleStayTimeBase;

    [Header("Mole Speed")]
    [SerializeField] private AnimationCurve _MoleMovementSpeedEvolution;
    [SerializeField] private float _MoleMovementSpeedBase;

    [Header("Display")]
    [SerializeField] private TextMeshProUGUI _TimerText;
    public TextMeshProUGUI _ScoreText;

    [Header("Other")]
    [SerializeField] private float _MoleSpawnYOffset;
    [SerializeField, Range(0.0f, 1.0f)] private float _GoldenMoleSpawnChance;

    [System.NonSerialized] public List<int> _HolesTenants;
    private List<Vector3> _HolesPositions;
    private bool _IsBugResolved;

    private Vector3 _InitialOffset;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        _HolesPositions = new();
        _HolesTenants = new List<int>(9);
        for (int i = 0; i < 9; i++)
        {
            _HolesTenants.Add(i);
        }
        _InitialOffset = transform.position - Camera.main.transform.position;
        _InitialOffset.z = 0;

    }
    private void Start()
    {
        MakeFakeGameFiles();
        _IsBugResolved = IsBugged();
        if (File.Exists(Application.persistentDataPath + "/MoleSaveFile.json"))
        {
            JsonDataService dataService = new JsonDataService();
            _StandResults = dataService.LoadData<StandResults>("MoleSaveFile");
        }

        gameObject.SetActive(false);
    }

    protected override void MakeFakeGameFiles()
    {
        base.MakeFakeGameFiles();
        if (!Directory.Exists("Game/Minigames/MoleWacker")) Directory.CreateDirectory("Game/Minigames/MoleWacker");
    }
    protected override bool IsBugged()
    {
        return File.Exists("Game/Minigames/MoleWacker/Mole.png");
    }
    private void OnApplicationQuit()
    {
        SaveStats();
    }
    protected override void SaveStats()
    {
        JsonDataService dataService = new JsonDataService();
        MedalType Medal = MedalType.None;
        if (_WinCount >= _MedalRequirements.MinRequiredForMedal[MedalType.Gold])
            Medal = MedalType.Gold;
        else if (_WinCount >= _MedalRequirements.MinRequiredForMedal[MedalType.Silver])
            Medal = MedalType.Silver;
        else if (_WinCount >= _MedalRequirements.MinRequiredForMedal[MedalType.Bronze])
            Medal = MedalType.Bronze;

        _StandResults = new StandResults(Medal, _WinCount);
        dataService.SaveData("MoleSaveFile", _StandResults);
    }

    private IEnumerator MoleSpawnRoutine()
    {
        float currentTimer = 0;
        float currentSpawnCooldown = 0.0f;
        float currentSpawnTimer = 0.0f;
        while (currentTimer < _MinigameDuration)
        {
            //spawn mole
            if (currentSpawnTimer > currentSpawnCooldown && _HolesTenants.Count > 0)
            {
                int chosenHole = _HolesTenants[Random.Range(0, _HolesTenants.Count)];
                bool goldenMole = Random.Range(0.0f, 1.0f) <= _GoldenMoleSpawnChance;
                _HolesTenants.Remove(chosenHole);
                GameObject mole = Instantiate(goldenMole ? _GoldenMolePrefab : _MolePrefab, _HolesPositions[chosenHole] - new Vector3(0, _MoleSpawnYOffset), Quaternion.identity, transform);
                mole.GetComponent<Mole>()._OccupiedHole = chosenHole;
                mole.GetComponent<Mole>()._PersistenceTime = _MoleStayTimeEvolution.Evaluate(currentTimer / _MinigameDuration) * _MoleStayTimeBase;
                mole.GetComponent<Mole>()._AppearanceDuration = _MoleMovementSpeedEvolution.Evaluate(currentTimer / _MinigameDuration) * _MoleMovementSpeedBase;
                mole.GetComponent<Mole>().SetLayer(chosenHole / 3);
                currentSpawnTimer = 0;
            }
            _TimerText.SetText($"Time : {Mathf.RoundToInt(_MinigameDuration - currentTimer)}");
            currentSpawnCooldown = _SpawnCooldownEvolution.Evaluate(currentTimer / _MinigameDuration) * _SpawnBaseCooldown;
            currentSpawnTimer += Time.deltaTime;
            currentTimer += Time.deltaTime;
            yield return null;
        }
        TriggerMinigameEnd();
    }
    [Button]
    public override void Interact()
    {
        if (CanInteract())
        {
            transform.position = Utility.GetWorldScreenCenterPos() + _InitialOffset;
            PlayerControls.Instance.SetVisibility(false,0.0f);
            gameObject.SetActive(true);
            RequiredMedalsDisplay.Instance.DisplayRequiredMedals(_MedalRequirements, _PointsImage);
            if (_IsBugResolved)
            {
                Transform Holes = transform.GetChild(0);
                foreach (Transform child in Holes)
                {
                    _HolesPositions.Add(child.position);
                }
                StartCoroutine(MoleSpawnRoutine());
            }
            else
            {
                //make dump and close game
                if (File.Exists("crashdump.txt"))
                {
                    File.Delete("crashdump.txt");
                }
                FileStream file = File.Create("crashdump.txt");
                file.Close();
                StreamWriter writer = new StreamWriter("crashdump.txt");
                writer.Write("CODE_500_ERR");
                writer.Close();
                Time.timeScale = 0;
                PauseMenu.instance._IsPauseBlocked = true;
                this.Invoke(() =>
                {
                    GameObject _BugMsg = Instantiate(_BugMessagePrefab);
                    _BugMsg.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Le sprite de la taupe est introuvable, un fichier de rapport d'erreur a été enregistré dans le dossier du jeu. Si le problème persiste, veuillez vous référer à notre serveur discord.");
                }, 1.0f, true);
                this.Invoke(() => Application.Quit(), 10f, true);
            }
        }
    }
}
