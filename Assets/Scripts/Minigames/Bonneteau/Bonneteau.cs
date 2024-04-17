using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Bonneteau : MonoBehaviour
{
    public static Bonneteau Instance;
    [System.NonSerialized] public UnityEvent OnWin = new();
    [System.NonSerialized] public UnityEvent OnLose = new();
    [System.NonSerialized] public UnityEvent OnCupChosen = new();

    [Header("Gameplay Stats")]
    public int _WinCount = 0;
    public int _LoseCount = 0;

    [Header("Refs")]
    [SerializeField] private List<GameObject> _Cups;
    [SerializeField] private GameObject _Ball;
    [SerializeField] private GameObject _CrashWindow;

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
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Minigames")) Directory.CreateDirectory("Game/Minigames");
        if (!Directory.Exists("Game/Minigames/Bonneteau")) Directory.CreateDirectory("Game/Minigames/Bonneteau");
        _IsBugResolved = File.Exists("Game/Minigames/Bonneteau/Ball.txt");
        _CupPositions = new List<Vector3>();
        foreach(var cup in _Cups)
        {
            _CupPositions.Add((cup.transform as RectTransform).position);
        }
        StartCoroutine(ShuffleCupsRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator ShuffleCupsRoutine()
    {
        _CanSelectCup = false;
        if (_CurrentShuffleCount < _ShuffleCount)
        {
            _BallCurrentIndex = Random.Range(0, _Cups.Count);
            _Ball.SetActive(false);
            //Generate Random cup switch amount
            int chosenSwitchCount = Random.Range(_MinSwitchCount, _MaxSwitchCount + 1);
            int currentSwitchCount = 0;
            while(currentSwitchCount < chosenSwitchCount)
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
    }
    private IEnumerator CupSwitchRoutine(int cupIndex1, int cupIndex2)
    {
        //fetches both cup refs
        RectTransform cup1 = _Cups[cupIndex1].transform as RectTransform;
        RectTransform cup2 = _Cups[cupIndex2].transform as RectTransform;
        Vector3 cup1OriginPosition = cup1.position;
        Vector3 cup2OriginPosition = cup2.position;
        Vector3 cupUpVector = Vector3.Cross((cup1OriginPosition - cup2OriginPosition).normalized,Vector3.forward).normalized;
        float lerpAlpha = 0.0f;
        float timeSinceSwitchStart = 0.0f;
        float switchDuration = _ShuffleDuration.Evaluate((float)_CurrentShuffleCount / _ShuffleCount);
        Vector3 displacementVector;
        while (lerpAlpha < 1.0f)
        {
            displacementVector = cupUpVector * _DisplacementScalar * _CupDisplacementEvolution.Evaluate(lerpAlpha);
            cup1.position = Vector3.Lerp(cup1OriginPosition, cup2OriginPosition,lerpAlpha)+displacementVector;
            cup2.position = Vector3.Lerp(cup2OriginPosition, cup1OriginPosition,lerpAlpha)-displacementVector;
            lerpAlpha += (1.0f/switchDuration) *_CupMovementEvolution.Evaluate(timeSinceSwitchStart/switchDuration)* Time.deltaTime;
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
        for(int i = 0; i < _CupPositions.Count; i++)
        {
            IndexList.Add(i);
        }
        int index1 = IndexList[Random.Range(0,IndexList.Count)];
        IndexList.Remove(index1);
        int index2 = IndexList[Random.Range(0, IndexList.Count)];
        return (index1, index2);
    }
    public void SelectCup(int cupIndex)
    {
        if (_CanSelectCup)
        {
            if(_IsBugResolved)
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
                //make dump and close game
                if (!File.Exists("crashdump.txt"))
                {
                    var file = File.Create("crashdump.txt");
                    file.Close();
                    StreamWriter writer = new StreamWriter("crashdump.txt");
                    writer.Write("CODE_401_AUTH_FAILED");
                    writer.Close();
                }
                this.Invoke(()=> _CrashWindow.SetActive(true), 1.0f);
                this.Invoke(()=> Application.Quit(), 2.5f);
            }
            
        }
    }
}

public static class Utility
{
    public static void Invoke(this MonoBehaviour mb, System.Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }

    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}