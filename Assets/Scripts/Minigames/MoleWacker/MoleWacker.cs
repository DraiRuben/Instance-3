using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MoleWacker : MonoBehaviour, IInteractable
{
    public static MoleWacker Instance;
    [System.NonSerialized] public UnityEvent OnMoleWacked = new();
    [System.NonSerialized] public UnityEvent OnMoleLost = new();
    [Header("Gameplay Stats")]
    public int _WinCount = 0;
    public int _LoseCount = 0;

    [Header("Refs")]
    [SerializeField] private GameObject _MolePrefab;
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
    [SerializeField] private float _MinigameDuration;

    [System.NonSerialized] public List<int> _HolesTenants;
    private List<Vector3> _HolesPositions;
    public StandResults _StandResults;
    private bool _IsBugResolved;

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
    }
    private void Start()
    {
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Minigames")) Directory.CreateDirectory("Game/Minigames");
        if (!Directory.Exists("Game/Minigames/MoleWacker")) Directory.CreateDirectory("Game/Minigames/MoleWacker");
        _IsBugResolved = File.Exists("Game/Minigames/MoleWacker/Mole.png");
        if (File.Exists(Application.persistentDataPath + "/MoleSaveFile.json"))
        {
            JsonDataService dataService = new JsonDataService();
            _StandResults = dataService.LoadData<StandResults>("MoleSaveFile");
        }
        gameObject.SetActive(false);
    }
    private void OnApplicationQuit()
    {
        SaveStats();
    }

    private void SaveStats()
    {
        JsonDataService dataService = new JsonDataService();
        MedalType Medal;
        if (_WinCount >= 17)
        {
            Medal = MedalType.Gold;
        }
        else if (_WinCount >= 11)
        {
            Medal = MedalType.Silver;
        }
        else if (_WinCount >= 6)
        {
            Medal = MedalType.Bronze;
        }
        else
        {
            Medal = MedalType.None;
        }
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
                _HolesTenants.Remove(chosenHole);
                GameObject mole = Instantiate(_MolePrefab, _HolesPositions[chosenHole] - new Vector3(0, _MoleSpawnYOffset), Quaternion.identity);
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
    private void TriggerMinigameEnd()
    {
        Cursor.visible = true;
        StopAllCoroutines();
        SaveStats();
        gameObject.SetActive(false);
        StandInteractableTrigger.Map.SetActive(true);
        PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = true;
        PlayerControls.Instance._PlayerInput.SwitchCurrentActionMap("Player");

        //TODO: Maybe change how minigame end is done so that we have a fade in and out of minigame instead of instant deactivation
    }
    public bool CanInteract()
    {
        return _StandResults._Medal == MedalType.None;
    }
    [Button]
    public void Interact()
    {
        if (CanInteract())
        {
            PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.SetActive(true);
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

                this.Invoke(() =>
                {
                    GameObject _BugMsg = Instantiate(_BugMessagePrefab);
                    _BugMsg.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Le sprite de la taupe est introuvable, un fichier de rapport d'erreur a été enregistré dans le dossier du jeu.");
                }, 1.0f);
                this.Invoke(() => Application.Quit(), 2.5f);
            }
        }
    }
}
