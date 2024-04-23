using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;

public class FishManager : MonoBehaviour, IInteractable
{
    public static FishManager Instance;
    [SerializeField] private GameObject _Fish;
    [SerializeField] private SplineContainer[] _Splines;
    [SerializeField] private AnimationCurve _SpeedCurve;
    [System.NonSerialized] public float _SpeedMult = 1;
    public List<GameObject> _FishList = new List<GameObject>();
    public int _BugValue;
    private float _ElapsedTime;
    public float _MinigameDuration;
    public StandResults _StandResults;
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
    }
    private void Start()
    {
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Minigames")) Directory.CreateDirectory("Game/Minigames");
        if (!Directory.Exists("Game/Minigames/FishingGame")) Directory.CreateDirectory("Game/Minigames/FishingGame");
        StreamReader reader = new StreamReader("Game/Minigames/FishingGame/FishBehavior.txt");

        _BugValue = reader.ReadLine() == "Enabled = true;" ? 0 : 1;
    }

    private void Update()
    {
        _SpeedMult = _SpeedCurve.Evaluate(_ElapsedTime / _MinigameDuration);
        _ElapsedTime += Time.deltaTime;
    }

    IEnumerator FishSpawn()
    {
        while (true)
        {
            if (_FishList.Count < 6)
            {
                _FishList.Add(Instantiate(_Fish, new Vector2(0, 1), Quaternion.identity));
                _FishList[_FishList.Count - 1].GetComponent<Fish>()._Spline = _Splines;
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }
    public bool CanInteract()
    {
        return _StandResults._Medal == MedalType.None;
    }

    public void Interact()
    {
        if (CanInteract())
        {
            StartCoroutine(FishSpawn());
        }
    }
}
