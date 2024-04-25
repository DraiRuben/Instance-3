using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;

public sealed class FishManager : Minigame
{
    public static FishManager Instance;
    [SerializeField] private GameObject _Fish;
    [SerializeField] private PoleManager _PoleManager;
    [SerializeField] private SplineContainer[] _Splines;
    [SerializeField] private AnimationCurve _SpeedCurve;
    [System.NonSerialized] public float _SpeedMult = 1;
    public List<GameObject> _FishList = new List<GameObject>();
    public int _BugValue;

    private Vector3 _InitialOffset;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        _InitialOffset = transform.position - Camera.main.transform.position;
        _InitialOffset.z = 0;
    }
    private void Start()
    {
        MakeFakeGameFiles();
        IsBugged();
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
            var file = File.Create("Game/Minigames/FishingGame/FishBehavior.txt");
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
        float elapsedTime = 0f;
        float spawnTimer = 0f;
        while (elapsedTime < _MinigameDuration)
        {
            if (_FishList.Count < 6 && spawnTimer>=2)
            {
                _FishList.Add(Instantiate(_Fish, _Splines[_BugValue].EvaluatePosition(0), Quaternion.identity,transform));
                _FishList[_FishList.Count - 1].GetComponent<Fish>()._Spline = _Splines;
                spawnTimer = 0f;
            }
            _SpeedMult = _SpeedCurve.Evaluate(elapsedTime / _MinigameDuration);
            spawnTimer += Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        TriggerMinigameEnd();

    }
    protected override void TriggerMinigameEnd()
    {
        base.TriggerMinigameEnd();
        _PoleManager._FishingTimer = 0;
    }
    protected override void SaveStats()
    {
        _PoleManager.SaveStats();
    }
    [Button]
    public override void Interact()
    {
        if (CanInteract())
        {
            transform.position = Utility.GetWorldScreenCenterPos() + _InitialOffset;

            PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.SetActive(true);
            StartCoroutine(FishSpawn());
        }
    }
}
