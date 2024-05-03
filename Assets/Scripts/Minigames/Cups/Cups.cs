using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Cups : Minigame
{
    public static Cups Instance;
    [System.NonSerialized] public UnityEvent OnWin = new();
    [System.NonSerialized] public UnityEvent OnLose = new();
    [System.NonSerialized] public UnityEvent OnCupChosen = new();

    [Header("Gameplay Stats")]
    public int _WinCount = 0;
    public int _LoseCount = 0;

    [Header("Refs")]
    [SerializeField] private GameObject _Ball;
    [SerializeField] private GameObject _BugMessagePrefab;
    [SerializeField] private TextMeshProUGUI _ScoreText;
    [SerializeField] private GameObject _CupPrefab;

    [Header("Curves")]
    [SerializeField] private AnimationCurve _CupMovementEvolution;
    [SerializeField] private AnimationCurve _CupDisplacementEvolution;
    [SerializeField] private AnimationCurve _ShuffleDuration;
    [SerializeField] private AnimationCurve _WaitTimeBetweenShuffles;
    [SerializeField] private AnimationCurve _CupCountEvolution;


    [Header("ShuffleParameters")]
    [SerializeField] private AnimationCurve _SwitchCountEvolution;

    [Header("Other")]
    [SerializeField, MinValue(0.0f)] private float _DisplacementScalar;
    [SerializeField] private float _BallLeftCupOffsetX;
    [SerializeField] private float _BallRightCupOffsetX;
    [SerializeField] private float _CupSpacingX;
    [SerializeField] private float _BallCupOffsetY;

    private List<Vector2> _CupPositions;
    private List<GameObject> _Cups;
    private GameObject _CupGagné;
    private GameObject _CupPerdu;
    private bool _HasSelectedCup;
    private bool _CanSelectCup;
    private bool _IsBugResolved;
    private int _BallCurrentIndex;
    private int _CurrentShuffleCount = 0;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        Random.InitState(System.DateTime.Now.Millisecond);
    }
    void Start()
    {
        MakeFakeGameFiles();
        _IsBugResolved = IsBugged();
        _CupPositions = new List<Vector2>();
        _Cups = new List<GameObject>();
        if (File.Exists(Application.persistentDataPath + "/CupsSaveFile.json"))
        {
            JsonDataService dataService = new JsonDataService();
            _StandResults = dataService.LoadData<StandResults>("CupsSaveFile");
        }
        gameObject.SetActive(false);
    }
    protected override void MakeFakeGameFiles()
    {
        base.MakeFakeGameFiles();
        if (!Directory.Exists("Game/Minigames/Cups")) Directory.CreateDirectory("Game/Minigames/Cups");
    }
    protected override bool IsBugged()
    {
        return File.Exists("Game/Minigames/Cups/Ball.txt");
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
        dataService.SaveData("CupsSaveFile", _StandResults);
    }
    private Vector2 GetBallOffset(int cupIndex)
    {
        Vector2 returnValue = new Vector2(0, _BallCupOffsetY);
        if (cupIndex == 0) returnValue.x = _BallLeftCupOffsetX;
        else if (cupIndex == _Cups.Count - 1) returnValue.x = _BallRightCupOffsetX;
        return returnValue;
    }
    private IEnumerator ShuffleCupsRoutine()
    {
        _CanSelectCup = false;
        if (_CurrentShuffleCount < (int)_MinigameDuration)
        {
            ManageCupsCount();
            _BallCurrentIndex = Random.Range(0, _Cups.Count);
            _Ball.GetComponent<RectTransform>().anchoredPosition = _CupPositions[_BallCurrentIndex] + GetBallOffset(_BallCurrentIndex);
            //TODO: start a coroutine that shows the player where the ball is at the start
            yield return ShowBallRoutine();

            //Generate Random cup switch amount
            int chosenSwitchCount = (int)_SwitchCountEvolution.Evaluate(_CurrentShuffleCount);
            int currentSwitchCount = 0;
            while (currentSwitchCount < chosenSwitchCount)
            {
                (int index1, int index2) cupSwitchTuple = GetRandomCupSwitchTuple();
                //launches switch routine and only goes further in the execution when it's finished
                yield return CupSwitchRoutine(cupSwitchTuple.index1, cupSwitchTuple.index2);
                //switch ball location if it was under any of the cups
                if (_BallCurrentIndex == cupSwitchTuple.index1) _BallCurrentIndex = cupSwitchTuple.index2;
                else if (_BallCurrentIndex == cupSwitchTuple.index2) _BallCurrentIndex = cupSwitchTuple.index1;
                _Cups.Swap(cupSwitchTuple.index1, cupSwitchTuple.index2);
                currentSwitchCount++;
            }
            _HasSelectedCup = false;
            _CanSelectCup = true;
            _CurrentShuffleCount++;
            _Ball.GetComponent<RectTransform>().anchoredPosition = _CupPositions[_BallCurrentIndex] + GetBallOffset(_BallCurrentIndex);
            SetCupsInteractable(true);
            yield return new WaitUntil(() => _HasSelectedCup);
            //Shuffles again after cup has been selected, this effectively works like a recursive function, but with async execution
            StartCoroutine(ShuffleCupsRoutine());
        }
        else
        {
            //save and exit stand
            TriggerMinigameEnd();
        }
    }
    private void ManageCupsCount()
    {
        int cupCount = (int)_CupCountEvolution.Evaluate(_CurrentShuffleCount);

        if (_Cups.Count != cupCount)
        {
            _CupPositions.Clear();
            if (_Cups.Count > cupCount)
            {
                for (int i = cupCount - 1; i < _Cups.Count; i++)
                {
                    Destroy(_Cups[i]);
                    _Cups.RemoveAt(i);
                }
            }
            else if (cupCount > _Cups.Count)
            {
                for (int i = _Cups.Count; i < cupCount; i++)
                {
                    GameObject cup = Instantiate(_CupPrefab, transform.GetChild(1));
                    (cup.transform as RectTransform).anchoredPosition = new Vector2(0, -205);
                    cup.GetComponent<Button>().onClick.AddListener(() => SelectCup(cup));
                    _Cups.Add(cup);
                }
            }
            float originMult = -(_Cups.Count - 1f) / 2f;
            for (int i = 0; i < _Cups.Count; i++)
            {
                RectTransform cupTransform = _Cups[i].transform as RectTransform;
                cupTransform.anchoredPosition = new Vector3(_CupSpacingX * (originMult + i) / _Cups.Count, cupTransform.anchoredPosition.y);
                _CupPositions.Add(cupTransform.anchoredPosition);
            }
        }
    }
    private IEnumerator ShowBallRoutine()
    {
        //index not good
        int animIndex = _BallCurrentIndex == 0 ? 1 : _BallCurrentIndex == _Cups.Count - 1 ? 3 : 2;
        _Cups[_BallCurrentIndex].GetComponent<Animator>().SetTrigger($"ShowBall{animIndex}");
        yield return new WaitForSeconds(0.3f);
        _Ball.SetActive(true);
        yield return new WaitForSeconds(2.7f);
        _Ball.SetActive(false);
    }
    private IEnumerator ShowCupRoutine(int cupIndex)
    {
        //index not good
        int animIndex = cupIndex == 0 ? 1 : cupIndex == _Cups.Count - 1 ? 3 : 2;
        _Cups[cupIndex].GetComponent<Animator>().SetTrigger($"ShowBall{animIndex}");
        yield return new WaitForSeconds(3f);
    }
    private IEnumerator ChooseCupAnimation(bool win, int chosenCupIndex)
    {
        //shows ball if win, shows chosen cup if not, then the other 2
        if (win)
        {
            yield return ShowBallRoutine();
        }
        else
        {
            //shows chosen cup first, then the remaining ones
            yield return ShowCupRoutine(chosenCupIndex);

            List<int> cups = new List<int>();
            for (int i = 0; i < _Cups.Count; i++)
            {
                cups.Add(i);
            }
            cups.Remove(chosenCupIndex);
            cups.Remove(_BallCurrentIndex);
            foreach (int cup in cups)
            {
                StartCoroutine(ShowCupRoutine(cup));
            }
            yield return ShowBallRoutine();
        }
    }
    private void SetCupsInteractable(bool interactable)
    {
        foreach (GameObject cup in _Cups)
        {
            cup.GetComponent<Button>().interactable = interactable;
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
        float switchDuration = _ShuffleDuration.Evaluate((float)_CurrentShuffleCount / ((int)_MinigameDuration));
        Vector3 displacementVector;
        AudioManager._Instance.PlaySFX("cupSlide");
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
        yield return new WaitForSeconds(_WaitTimeBetweenShuffles.Evaluate((float)_CurrentShuffleCount / ((int)_MinigameDuration)));
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
    public void SelectCup(GameObject SelectedCup)
    {
        if (_CanSelectCup)
        {
            SetCupsInteractable(false);
            if (_IsBugResolved)
            {
                _CanSelectCup = false;
                int selectedIndex = _CupPositions.FindIndex(0, x => (int)x.x == (int)(SelectedCup.transform as RectTransform).anchoredPosition.x);
                if (selectedIndex == _BallCurrentIndex)
                {
                    //win feedback
                    _Cups[selectedIndex].gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    _Cups[selectedIndex].gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>().SetTrigger("Gagné");

                    _WinCount++;
                    _ScoreText.SetText(_WinCount.ToString());
                    OnWin.Invoke();
                    StartCoroutine(ChooseCupAnimation(true, selectedIndex));
                    this.Invoke(() => _HasSelectedCup = true, 3f);
                    Debug.Log("Win");
                    AudioManager._Instance.PlaySFX("cupWin");
                }
                else
                {
                    //lose feedback
                    _Cups[selectedIndex].gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    _Cups[selectedIndex].gameObject.transform.GetChild(1).gameObject.GetComponent<Animator>().SetTrigger("Perdu");
                    //_CupPerdu.SetActive(true);
                    //_CupPerdu.GetComponent<Animator>().SetTrigger("Perdu");
                    _LoseCount++;
                    OnLose.Invoke();
                    StartCoroutine(ChooseCupAnimation(false, selectedIndex));
                    this.Invoke(() => _HasSelectedCup = true, 7f);

                    Debug.Log("Lose");
                }
            }
            else
            {
                if (File.Exists("crashdump.txt"))
                {
                    File.Delete("crashdump.txt");
                }
                //make dump and close game
                FileStream file = File.Create("crashdump.txt");
                file.Close();
                StreamWriter writer = new StreamWriter("crashdump.txt");
                writer.Write("CODE_401_AUTH_FAILED");
                writer.Close();
                Time.timeScale = 0;
                PauseMenu.instance._IsPauseBlocked = true;
                this.Invoke(() =>
                {
                    GameObject _BugMsg = Instantiate(_BugMessagePrefab);
                    _BugMsg.transform.GetChild(3).GetComponent<TextMeshProUGUI>().SetText("Le sprite de la balle est introuvable, un fichier de rapport d'erreur a été enregistré dans le dossier du jeu. Si le problème persiste, veuillez vous référer à notre serveur discord.");
                }, 1.0f, true);
                this.Invoke(() => Application.Quit(), 10f, true);
            }

        }
    }
    public override void TriggerMinigameEnd(bool ClosePreEmptively = false)
    {
        base.TriggerMinigameEnd(ClosePreEmptively);
        _CurrentShuffleCount = 0;
    }
    [Button]
    public override void Interact()
    {
        if (CanInteract())
        {
            PlayerControls.Instance.SetVisibility(false,0.0f);
            gameObject.SetActive(true);
            RequiredMedalsDisplay.Instance.DisplayRequiredMedals(_MedalRequirements, _PointsImage);
            StartCoroutine(ShuffleCupsRoutine());
        }
    }
}
