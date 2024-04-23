using Sirenix.OdinInspector;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;


public class RifleMinigame : MonoBehaviour, IInteractable
{
    public static RifleMinigame Instance;
    [Header("sound")]
    [SerializeField] private AudioClip _ReloadSound;
    [SerializeField] private AudioClip _ShootSound;
    [SerializeField] private bool _IsBugged;
    [SerializeField] private float _MinigameDuration;
    [SerializeField] private TextMeshProUGUI _TimerText;
    [SerializeField] private TextMeshProUGUI _ScoreText;
    private int _Points;
    private float _ReloadTime;
    public StandResults _StandResults;
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
    }
    [Button]
    public void Interact()
    {
        gameObject.SetActive(true);
        StartCoroutine(RunMinigame());
    }
    private void Start()
    {
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Animations")) Directory.CreateDirectory("Game/Animations");
        if (!Directory.Exists("Game/Animations/Rifle")) Directory.CreateDirectory("Game/Animations/Rifle");
        _IsBugged = !File.Exists("Game/Animations/Rifle/Target.anim");
        if (File.Exists(Application.persistentDataPath + "/RifleSaveFile.json"))
        {
            JsonDataService dataService = new JsonDataService();
            _StandResults = dataService.LoadData<StandResults>("RifleSaveFile");
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
        if (_Points >= 6)
        {
            Medal = MedalType.Gold;
        }
        else if (_Points >= 4)
        {
            Medal = MedalType.Silver;
        }
        else if (_Points >= 2)
        {
            Medal = MedalType.Bronze;
        }
        else
        {
            Medal = MedalType.None;
        }
        _StandResults = new StandResults(Medal, _Points);
        dataService.SaveData("RifleSaveFile", _StandResults);
    }
    public void HandleTargetHit(GameObject target)
    {
        if (_ReloadTime <= 0)
        {
            if (!_IsBugged)
            {
                Destroy(target);
                _Points++;
                _ScoreText.SetText($"Score : {_Points}");
                this.Invoke(() => {
                    if (transform.GetChild(0).childCount <= 0) //targets child
                    {
                        TriggerMinigameEnd();
                    }
                },0.1f);
                
            }
        }
    }
    private IEnumerator RunMinigame()
    {
        float _elapsedTime = 0f;
        Cursor.visible = false;
        while (true)
        {
            ReloadTimer();
            if (Input.GetMouseButtonDown(0) && _ReloadTime <= 0)
            {
                StartCoroutine(Shoot());
            }
            _elapsedTime += Time.deltaTime;
            _TimerText.SetText($"Time : {Mathf.RoundToInt(_MinigameDuration-_elapsedTime)}");
            if (_elapsedTime >= _MinigameDuration)
            {
                //end minigame here
                TriggerMinigameEnd();
                break;
            }
            yield return null;
        }
       
    }
    private void TriggerMinigameEnd()
    {
        Cursor.visible = true;
        StopAllCoroutines();
    }
    private IEnumerator Shoot()
    {
        AudioManager.Instance.PlaySound("shoot");
        yield return null;
        _ReloadTime = _ReloadSound.length + _ShootSound.length;
        yield return new WaitForSeconds(_ShootSound.length);
        AudioManager.Instance.PlaySound("reload");
    }

    private void ReloadTimer()
    {
        if (_ReloadTime > 0)
        {
            _ReloadTime -= Time.deltaTime;
        }
    }

}