using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Splines;

public class Mole : MonoBehaviour, IPointerClickHandler
{
    public float _PersistenceTime;
    public float _AppearanceDuration;
    public int _OccupiedHole;

    private float _MovementLerpAlpha;
    private bool _IsDisappearing;
    private bool _IsWacked;
    private Animator _Animator;
    [SerializeField] private float _DisappearanceDuration;
    [SerializeField] private float _DeathStunTime;
    [SerializeField] private float _ShowLocationYOffset = 1.5f;

    private static Texture2D _MoleEasterEggTexture;
    private static Sprite _MoleEasterEggSprite;
    private Vector3 _MoleHiddenLocation;
    private Vector3 _MoleShownLocation;
    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _MoleHiddenLocation = transform.position;
        _MoleShownLocation = transform.position + new Vector3(0.0f, _ShowLocationYOffset, 0.0f);
        StartCoroutine(MoleSpawnMovementRoutine());

        //easter egg that loads image from folder
        /*if (!_MoleEasterEggSprite)
        {
            byte[] fileData = File.ReadAllBytes("Game/Minigames/MoleWacker/Mole.png");
            _MoleEasterEggTexture = new Texture2D(2, 2);
            _MoleEasterEggTexture.LoadImage(fileData);
            _MoleEasterEggSprite = Sprite.Create(_MoleEasterEggTexture, new Rect(0, 0, _MoleEasterEggTexture.width, _MoleEasterEggTexture.height), new Vector2(0, 0), 100, 0, SpriteMeshType.Tight);
        }
        GetComponent<SpriteRenderer>().sprite = _MoleEasterEggSprite;*/
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        MakeMoleDisappear(true);
    }
    public void SetLayer(int layer)
    {
        if (layer >= 0 && layer < 3)
            GetComponent<SortingGroup>().sortingLayerName = $"Layer{layer + 1}Moles";
    }
    private void MakeMoleDisappear(bool _KillMole = false)
    {
        if (true/*_MovementLerpAlpha > 0.5f*/)
        {
            if (!_IsDisappearing ||(_KillMole && !_IsWacked))
            {
                StopAllCoroutines();
                _IsWacked = _KillMole;
                if (_IsWacked)
                {
                    _Animator.SetTrigger("Dead");
                    MoleWacker.Instance._ScoreText.SetText($"Score : {++MoleWacker.Instance._WinCount}");
                    MoleWacker.Instance.OnMoleWacked.Invoke();
                    this.Invoke(() => StartCoroutine(MoleDisappearanceRoutine()), _DeathStunTime);
                }
                else
                {
                    _Animator.SetTrigger("Gone");
                    MoleWacker.Instance._LoseCount++;
                    MoleWacker.Instance.OnMoleLost.Invoke();
                    StartCoroutine(MoleDisappearanceRoutine());
                }
                _IsDisappearing = true;
            }
        }
    }
    private IEnumerator MoleDisappearanceRoutine()
    {
        float currentDisappearanceLerpAlpha = 0.0f;
        while (currentDisappearanceLerpAlpha < 1)
        {
            transform.position = Vector3.Lerp(_MoleShownLocation, _MoleHiddenLocation, currentDisappearanceLerpAlpha);
            currentDisappearanceLerpAlpha += Time.deltaTime / _DisappearanceDuration;
            yield return null;
        }
        //add hole back to usable holes after the mole finished its disappearance routine
        MoleWacker.Instance._HolesTenants.Add(_OccupiedHole);
        Destroy(gameObject);
    }
    private IEnumerator MoleSpawnMovementRoutine()
    {
        //TODO: play mole appearance anim
        while (_MovementLerpAlpha < 1)
        {
            transform.position = Vector3.Lerp(_MoleHiddenLocation, _MoleShownLocation, _MovementLerpAlpha);
            _MovementLerpAlpha += Time.deltaTime / _AppearanceDuration;
            yield return null;
        }
        yield return new WaitForSeconds(_PersistenceTime);
        if (!_IsWacked)
            MakeMoleDisappear();
    }
}
