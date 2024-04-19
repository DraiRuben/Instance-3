using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Cups : MonoBehaviour, IInteractable
{
    public static Cups Instance;
    [System.NonSerialized] public UnityEvent OnWin = new();
    [System.NonSerialized] public UnityEvent OnLose = new();
    [System.NonSerialized] public UnityEvent OnCupChosen = new();

    [Header("Gameplay Stats")]
    public int _WinCount = 0;
    public int _LoseCount = 0;

    [Header("Refs")]
    [SerializeField] private List<GameObject> _Cups;
    [SerializeField] private GameObject _Ball;
    [SerializeField] private GameObject _BugMessagePrefab;

    [Header("Curves")]
    [SerializeField] private AnimationCurve _CupMovementEvolution;
    [SerializeField] private AnimationCurve _CupDisplacementEvolution;
    [SerializeField] private AnimationCurve _ShuffleDuration;
    [SerializeField] private AnimationCurve _WaitTimeBetweenShuffles;

    [Header("ShuffleParameters")]
    [SerializeField, Range(1, 20)] private int _MinSwitchCount;
    [SerializeField, Range(1, 20), MinValue("_MinSwitchCount")] private int _MaxSwitchCount;
    [SerializeField] private int _ShuffleCount;

    [Header("Other")]
    [SerializeField, MinValue(0.0f)] private float _DisplacementScalar;

    private List<Vector3> _CupPositions;
    private bool _HasSelectedCup;
    private bool _CanSelectCup;
    private bool _IsBugResolved;
    private int _BallCurrentIndex;
    private int _CurrentShuffleCount = 0;
    private StandResults _StandResults;
    public void Interact()
    {
        if(_StandResults._Medal == MedalType.None)
        {
            gameObject.SetActive(true);
            StartCoroutine(ShuffleCupsRoutine());
        }
    }
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;

        Random.InitState(System.DateTime.Now.Millisecond);
    }
    void Start()
    {
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Minigames")) Directory.CreateDirectory("Game/Minigames");
        if (!Directory.Exists("Game/Minigames/Cups")) Directory.CreateDirectory("Game/Minigames/Cups");
        _IsBugResolved = File.Exists("Game/Minigames/Cups/Ball.txt");
        _CupPositions = new List<Vector3>();
        foreach (var cup in _Cups)
        {
            _CupPositions.Add((cup.transform as RectTransform).position);
        }
        if (File.Exists(Application.persistentDataPath + "/CupsSaveFile.json"))
        {
            JsonDataService dataService = new JsonDataService();
            _StandResults = dataService.LoadData<StandResults>("CupsSaveFile");
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
        switch (_WinCount)
        {
            case 0:
                Medal = MedalType.None;
                break;
            case 1:
                Medal = MedalType.Bronze;
                break;
            case 2:
                Medal = MedalType.Silver;
                break;
            case 3:
                Medal = MedalType.Gold;
                break;
            default:
                Medal = MedalType.None;
                break;
        }
        _StandResults = new StandResults(Medal, _WinCount);
        dataService.SaveData("CupsSaveFile", _StandResults);
    }
    private IEnumerator ShuffleCupsRoutine()
    {
        _CanSelectCup = false;
        if (_CurrentShuffleCount < _ShuffleCount)
        {
            _BallCurrentIndex = Random.Range(0, _Cups.Count);
            _Ball.GetComponent<RectTransform>().position = _CupPositions[_BallCurrentIndex];
            _Ball.SetActive(false);
            //TODO: start a coroutine that shows the player where the ball is at the start


            //Generate Random cup switch amount
            int chosenSwitchCount = Random.Range(_MinSwitchCount, _MaxSwitchCount + 1);
            int currentSwitchCount = 0;
            while (currentSwitchCount < chosenSwitchCount)
            {
                var cupSwitchTuple = GetRandomCupSwitchTuple();
                //launches switch routine and only goes further in the execution when it's finished
                yield return CupSwitchRoutine(cupSwitchTuple.index1, cupSwitchTuple.index2);
                //switch ball location if it was under any of the cups
                if (_BallCurrentIndex == cupSwitchTuple.index1) _BallCurrentIndex = cupSwitchTuple.index2;
                else if (_BallCurrentIndex == cupSwitchTuple.index2) _BallCurrentIndex = cupSwitchTuple.index1;
                currentSwitchCount++;
            }
            _HasSelectedCup = false;
            _CanSelectCup = true;
            _CurrentShuffleCount++;
            _Ball.GetComponent<RectTransform>().position = _CupPositions[_BallCurrentIndex];
            _Ball.SetActive(true);
            yield return new WaitUntil(() => _HasSelectedCup);
            //Shuffles again after cup has been selected, this effectively works like a recursive function, but with async execution
            StartCoroutine(ShuffleCupsRoutine());
        }
        else
        {
            //save and exit stand
            SaveStats();
        }
    }
    private IEnumerator CupSwitchRoutine(int cupIndex1, int cupIndex2)
    {
        //fetches both cup refs and their original locations
        RectTransform cup1 = _Cups[cupIndex1].transform as RectTransform;
        RectTransform cup2 = _Cups[cupIndex2].transform as RectTransform;
        Vector3 cup1OriginPosition = cup1.position;
        Vector3 cup2OriginPosition = cup2.position;
        //gets the vector that's perpendicular to the vector going from one location to the other
        //this is to add to the lerp to make the curved trajectory instead of a straight one
        Vector3 cupUpVector = Vector3.Cross((cup1OriginPosition - cup2OriginPosition).normalized, Vector3.forward).normalized;
        float lerpAlpha = 0.0f;
        float timeSinceSwitchStart = 0.0f;
        float switchDuration = _ShuffleDuration.Evaluate((float)_CurrentShuffleCount / _ShuffleCount);
        Vector3 displacementVector;
        while (lerpAlpha < 1.0f)
        {
            displacementVector = cupUpVector * _DisplacementScalar * _CupDisplacementEvolution.Evaluate(lerpAlpha);
            cup1.position = Vector3.Lerp(cup1OriginPosition, cup2OriginPosition, lerpAlpha) + displacementVector;
            cup2.position = Vector3.Lerp(cup2OriginPosition, cup1OriginPosition, lerpAlpha) - displacementVector;
            lerpAlpha += (1.0f / switchDuration) * _CupMovementEvolution.Evaluate(timeSinceSwitchStart / switchDuration) * Time.deltaTime;
            timeSinceSwitchStart += Time.deltaTime;
            yield return null;
        }
        cup1.position = cup2OriginPosition;
        cup2.position = cup1OriginPosition;
        yield return new WaitForSeconds(_WaitTimeBetweenShuffles.Evaluate((float)_CurrentShuffleCount / _ShuffleCount));
    }
    private (int index1, int index2) GetRandomCupSwitchTuple()
    {
        List<int> IndexList = new();
        //Populates index list with 0,1,2,3, etc...
        for (int i = 0; i < _CupPositions.Count; i++)
        {
            IndexList.Add(i);
        }
        int index1 = IndexList[Random.Range(0, IndexList.Count)];
        IndexList.Remove(index1);
        int index2 = IndexList[Random.Range(0, IndexList.Count)];
        return (index1, index2);
    }
    public void SelectCup(int cupIndex)
    {
        if (_CanSelectCup)
        {
            if (_IsBugResolved)
            {
                int selectedIndex = _CupPositions.FindIndex(0, x => x == (_Cups[cupIndex].transform as RectTransform).position);
                if (selectedIndex == _BallCurrentIndex)
                {
                    //win feedback
                    _WinCount++;
                    OnWin.Invoke();
                    Debug.Log("Win");
                }
                else
                {
                    //lose feedback
                    _LoseCount++;
                    OnLose.Invoke();
                    Debug.Log("Lose");
                }
                OnCupChosen.Invoke();
                _HasSelectedCup = true;
            }
            else
            {
                if (File.Exists("crashdump.txt"))
                {
                    File.Delete("crashdump.txt");
                }
                //make dump and close game
                var file = File.Create("crashdump.txt");
                file.Close();
                StreamWriter writer = new StreamWriter("crashdump.txt");
                writer.Write("CODE_401_AUTH_FAILED");
                writer.Close();

                this.Invoke(() => 
                { 
                   var _BugMsg = Instantiate(_BugMessagePrefab);
                    _BugMsg.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Le sprite de la balle est introuvable, un fichier de rapport d'erreur a été enregistré dans le dossier du jeu.");
                }, 1.0f);
                this.Invoke(() => Application.Quit(), 2.5f);
            }

        }
    }
}
