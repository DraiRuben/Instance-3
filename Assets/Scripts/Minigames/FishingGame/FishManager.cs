using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;

public class FishManager : MonoBehaviour, IInteractable
{
    public static FishManager Instance;
    [SerializeField] private GameObject _Fish;
    [SerializeField] private PoleManager _PoleManager;
    [SerializeField] private SplineContainer[] _Splines;
    [SerializeField] private AnimationCurve _SpeedCurve;
    [System.NonSerialized] public float _SpeedMult = 1;
    public List<GameObject> _FishList = new List<GameObject>();
    public int _BugValue;
    private float _ElapsedTime;
    public float _MinigameDuration;
    public StandResults _StandResults;

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
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Minigames")) Directory.CreateDirectory("Game/Minigames");
        if (!Directory.Exists("Game/Minigames/FishingGame")) Directory.CreateDirectory("Game/Minigames/FishingGame");
        StreamReader reader = new StreamReader("Game/Minigames/FishingGame/FishBehavior.txt");

        _BugValue = reader.ReadLine() == "Enabled = true;" ? 0 : 1;
        gameObject.SetActive(false);

    }
    private IEnumerator FishSpawn()
    {
        float spawnTimer = 0f;
        while (_ElapsedTime < _MinigameDuration)
        {
            if (_FishList.Count < 6 && spawnTimer>=2)
            {
                _FishList.Add(Instantiate(_Fish, _Splines[_BugValue].EvaluatePosition(0), Quaternion.identity,transform));
                _FishList[_FishList.Count - 1].GetComponent<Fish>()._Spline = _Splines;
                spawnTimer = 0f;
            }
            _SpeedMult = _SpeedCurve.Evaluate(_ElapsedTime / _MinigameDuration);
            spawnTimer += Time.deltaTime;
            _ElapsedTime += Time.deltaTime;
            yield return null;
        }
        TriggerMinigameEnd();

    }
    private void TriggerMinigameEnd()
    {
        Cursor.visible = true;
        StopAllCoroutines();
        _PoleManager.StopAllCoroutines();
        _PoleManager.SaveStats();
        gameObject.SetActive(false);
        StandInteractableTrigger.Map.SetActive(true);
        PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = true;
        PlayerControls.Instance._PlayerInput.SwitchCurrentActionMap("Player");


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
            transform.position = Utility.GetWorldScreenCenterPos() + _InitialOffset;

            PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.SetActive(true);
            StartCoroutine(FishSpawn());
        }
    }
}
