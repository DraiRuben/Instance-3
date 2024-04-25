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
    private int _Points;
    private float _ReloadTime;

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
        _IsBugged = IsBugged();
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
            var targetAnim = File.Create("Game/Animations/Whack-a-mole/Target.anim");
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
                _ScoreText.SetText(_Points.ToString());
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
        
        PlayerControls.Instance?.OnSelect.AddListener(() =>
        {
            if (_ReloadTime <= 0 && Time.timeScale==1)
            {
                StartCoroutine(Shoot());
            }
        });

        while (true)
        {
            ReloadTimer();
            if(Time.timeScale == 1)
                Cursor.visible = false;
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
        PlayerControls.Instance?.OnSelect.RemoveAllListeners();
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

    [Button]
    public override void Interact()
    {
        if (CanInteract())
        {
            transform.position = Utility.GetWorldScreenCenterPos() + _InitialOffset;

            PlayerControls.Instance.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.SetActive(true);
            StartCoroutine(RunMinigame());
        }
    }

}