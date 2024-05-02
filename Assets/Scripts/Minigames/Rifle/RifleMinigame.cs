using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;


public class RifleMinigame : Minigame
{
    public static RifleMinigame Instance;
    [Header("sound")]
    [SerializeField] private AudioClip _ReloadSound;
    [SerializeField] private AudioClip _ShootSound;
    [SerializeField] private bool _IsBugged;
    [SerializeField] private TextMeshProUGUI _TimerText;
    [SerializeField] private TextMeshProUGUI _ScoreText;
    [SerializeField] private CameraShakeProfile _ShootShakeProfile;
    [SerializeField] private GameObject _ImpactGameObject;
    private CinemachineImpulseSource _ShootShakeSource;
    private int _Points;
    private float _ReloadTime;

    private Vector2 _ScreenPos;
    private Vector2 _WorldPos;

    private Vector3 _InitialOffset;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        _InitialOffset = transform.position - Camera.main.transform.position;
        _InitialOffset.z = 0;
        _ShootShakeSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Start()
    {
        MakeFakeGameFiles();
        if (File.Exists(Application.persistentDataPath + "/RifleSaveFile.json"))
        {
            JsonDataService dataService = new JsonDataService();
            _StandResults = dataService.LoadData<StandResults>("RifleSaveFile");
        }
        gameObject.SetActive(false);
    }
    protected override bool IsBugged()
    {
        if (File.Exists("Game/Animations/Rifle/Target.anim")
           && File.GetCreationTime("Game/Animations/Rifle/Target.anim") == new System.DateTime(2002, 12, 14))
            return false;

        return true;
    }
    protected override void MakeFakeGameFiles()
    {
        base.MakeFakeGameFiles();
        if (!Directory.Exists("Game/Animations")) Directory.CreateDirectory("Game/Animations");
        //false rifle anims
        if (!Directory.Exists("Game/Animations/Rifle")) Directory.CreateDirectory("Game/Animations/Rifle");
        if (!File.Exists("Game/Animations/Rifle/Bullet.anim")) File.Create("Game/Animations/Rifle/Bullet.anim");
        if (!File.Exists("Game/Animations/Rifle/Rifle.anim")) File.Create("Game/Animations/Rifle/Rifle.anim");
        //false cup anims
        if (!Directory.Exists("Game/Animations/Cup")) Directory.CreateDirectory("Game/Animations/Cup");
        if (!File.Exists("Game/Animations/Cup/Cup.anim")) File.Create("Game/Animations/Cup/Cup.anim");
        if (!File.Exists("Game/Animations/Cup/Shuffling.anim")) File.Create("Game/Animations/Cup/Shuffling.anim");
        //false character anims
        if (!Directory.Exists("Game/Animations/Character")) Directory.CreateDirectory("Game/Animations/Character");
        if (!File.Exists("Game/Animations/Character/IdleCharacter.anim")) File.Create("Game/Animations/Character/IdleCharacter.anim");
        if (!File.Exists("Game/Animations/Character/StandInteract.anim")) File.Create("Game/Animations/Character/StandInteract.anim");
        if (!File.Exists("Game/Animations/Character/WalkingCharacter.anim")) File.Create("Game/Animations/Character/WalkingCharacter.anim");

        //false whackamole anims
        if (!Directory.Exists("Game/Animations/Whack-a-mole"))
        {
            Directory.CreateDirectory("Game/Animations/Whack-a-mole");
            FileStream targetAnim = File.Create("Game/Animations/Whack-a-mole/Target.anim");
            targetAnim.Close();
            File.SetCreationTime("Game/Animations/Whack-a-mole/Target.anim", new System.DateTime(2002, 12, 14));
        }
        if (!File.Exists("Game/Animations/Whack-a-mole/Hammer.anim")) File.Create("Game/Animations/Whack-a-mole/Hammer.anim");
        if (!File.Exists("Game/Animations/Whack-a-mole/Mole.anim")) File.Create("Game/Animations/Whack-a-mole/Mole.anim");
        if (!File.Exists("Game/Animations/Whack-a-mole/MoleWacked.anim")) File.Create("Game/Animations/Whack-a-mole/MoleWacked.anim");

        //false Prize counter anims
        if (!Directory.Exists("Game/Animations/PrizeCounter")) Directory.CreateDirectory("Game/Animations/PrizeCounter");
        if (!File.Exists("Game/Animations/PrizeCounter/PrizeObtain.anim")) File.Create("Game/Animations/PrizeCounter/PrizeObtain.anim");

        //false fishing anims
        if (!Directory.Exists("Game/Animations/Fishing")) Directory.CreateDirectory("Game/Animations/Fishing");
        if (!File.Exists("Game/Animations/Fishing/Fish.anim")) File.Create("Game/Animations/Fishing/Fish.anim");
        if (!File.Exists("Game/Animations/Fishing/FishingRod.anim")) File.Create("Game/Animations/Fishing/FishingRod.anim");


    }
    private void OnApplicationQuit()
    {
        SaveStats();
    }
    protected override void SaveStats()
    {
        JsonDataService dataService = new JsonDataService();
        MedalType Medal = MedalType.None;
        if (_Points >= _MedalRequirements.MinRequiredForMedal[MedalType.Gold])
            Medal = MedalType.Gold;
        else if (_Points >= _MedalRequirements.MinRequiredForMedal[MedalType.Silver])
            Medal = MedalType.Silver;
        else if (_Points >= _MedalRequirements.MinRequiredForMedal[MedalType.Bronze])
            Medal = MedalType.Bronze;

        _StandResults = new StandResults(Medal, _Points);
        dataService.SaveData("RifleSaveFile", _StandResults);
    }
    public void HandleTargetHit(Target target)
    {
        if (_ReloadTime <= 0)
        {
            if (!_IsBugged)
            {
                _Points++;
                target.DoDestructionFeedback();
                _ScoreText.SetText(_Points.ToString());
                AudioManager._Instance.PlaySFX("targetHit", true);
                this.Invoke(() =>
                {
                    if (transform.GetChild(0).childCount <= 0) //targets child
                    {
                        TriggerMinigameEnd();
                    }
                }, 0.2f);
            }
        }
    }
    private IEnumerator RunMinigame()
    {
        float _elapsedTime = 0f;

        PlayerControls.Instance?.OnSelect.AddListener(() =>
        {
            if (_ReloadTime <= 0 && Time.timeScale == 1)
            {
                StartCoroutine(Shoot());
            }
        });

        while (true)
        {
            ReloadTimer();

            _elapsedTime += Time.deltaTime;
            _TimerText.SetText($"Time : {Mathf.RoundToInt(_MinigameDuration - _elapsedTime)}");
            if (_elapsedTime >= _MinigameDuration)
            {
                //end minigame here
                TriggerMinigameEnd();
                break;
            }
            yield return null;
        }
        PlayerControls.Instance?.OnSelect.RemoveAllListeners();
    }

    private IEnumerator Shoot()
    {
        AudioManager._Instance.PlaySFX("shoot", true);
        BulletImpact();
        CameraShakeManager.Instance.ShakeCamera(_ShootShakeSource, _ShootShakeProfile);
        yield return null;
        _ReloadTime = _ReloadSound.length + _ShootSound.length;
        yield return new WaitForSeconds(_ShootSound.length);
        AudioManager._Instance.PlaySFX("reload", true);
    }

    private void ReloadTimer()
    {
        if (_ReloadTime > 0)
        {
            _ReloadTime -= Time.deltaTime;
        }
    }

    [Button]
    public override void Interact()
    {
        if (CanInteract())
        {
            _IsBugged = IsBugged();
            transform.position = Utility.GetWorldScreenCenterPos() + _InitialOffset;

            PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.SetActive(true);
            StartCoroutine(RunMinigame());
        }
    }

    private void BulletImpact()
    {
        _ScreenPos = Input.mousePosition;
        _WorldPos = Camera.main.ScreenToWorldPoint(_ScreenPos);
        GameObject impact = Instantiate(_ImpactGameObject, _WorldPos, Quaternion.identity);
        Destroy(impact, 1f);
    }

}