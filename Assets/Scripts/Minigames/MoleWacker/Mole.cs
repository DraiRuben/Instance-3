using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Mole : MonoBehaviour, IPointerClickHandler
{
    [System.NonSerialized] public float _PersistenceTime;
    [System.NonSerialized] public float _AppearanceDuration;
    [System.NonSerialized] public int _OccupiedHole;
    [SerializeField] private int _ScoreGain;

    private float _MovementLerpAlpha;
    private bool _IsWacked;
    private Animator _Animator;

    [Header("Behaviour")]
    [SerializeField] private float _DisappearanceDuration;
    [SerializeField] private float _DeathStunTime;
    [SerializeField] private float _ShowLocationYOffset = 1.5f;

    [Header("Stun Parameters")]
    [SerializeField, MinMaxSlider(0, 1.5f)] private Vector2 _TimeBetweenStunFramesRange;
    [SerializeField, MinMaxSlider(-1f, 1f)] private Vector2 _StunDistanceXFrameOffsetRange;
    [SerializeField, MinMaxSlider(-1f, 1f)] private Vector2 _StunDistanceYFrameOffsetRange;

    private static Texture2D _MoleEasterEggTexture;
    private static Sprite _MoleEasterEggSprite;
    private Vector3 _MoleHiddenLocation;
    private Vector3 _MoleShownLocation;
    private bool _IsDisappearancePaused;
    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _MoleHiddenLocation = transform.position;
        _MoleShownLocation = transform.position + new Vector3(0.0f, _ShowLocationYOffset);
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
        if (!_IsWacked)
        {
            MakeMoleDisappear(true);
        }
    }
    public void SetLayer(int layer)
    {
        if (layer >= 0 && layer < 3)
            GetComponent<SortingGroup>().sortingLayerName = $"Layer{layer + 1}Moles";
    }
    private void MakeMoleDisappear(bool _KillMole = false)
    {
        _IsWacked = _KillMole;
        StopAllCoroutines();
        if (_IsWacked)
        {
            AudioManager._Instance.PlaySFX("moleHit", true);
            _Animator.SetTrigger("Dead");
            MoleWacker.Instance._Points += _ScoreGain;
            MoleWacker.Instance._ScoreText.SetText(MoleWacker.Instance._Points.ToString());
            MoleWacker.Instance.OnMoleWacked.Invoke();
            _IsDisappearancePaused = true;
            StartCoroutine(StunMole());
            this.Invoke(() => _IsDisappearancePaused = false, _DeathStunTime);
        }
        else
        {
            _Animator.SetTrigger("Gone");
            MoleWacker.Instance.OnMoleLost.Invoke();
        }
        StartCoroutine(MoleDisappearanceRoutine());
    }
    private IEnumerator StunMole()
    {
        float lastOffsetGeneration = 0;
        float currentOffsetFrameDuration = Random.Range(_TimeBetweenStunFramesRange.x, _TimeBetweenStunFramesRange.y);
        float OffsetX = 0;
        float OffsetY = 0;
        Vector3 nonOffsetPos = transform.position;
        while (_IsDisappearancePaused)
        {
            if (Time.time - lastOffsetGeneration > currentOffsetFrameDuration)
            {
                lastOffsetGeneration = Time.time;
                currentOffsetFrameDuration = Random.Range(_TimeBetweenStunFramesRange.x, _TimeBetweenStunFramesRange.y);
                OffsetX = Random.Range(_StunDistanceXFrameOffsetRange.x, _StunDistanceXFrameOffsetRange.y);
                OffsetY = Random.Range(_StunDistanceYFrameOffsetRange.x, _StunDistanceYFrameOffsetRange.y);
                transform.position = nonOffsetPos + new Vector3(OffsetX, OffsetY);
            }
            yield return null;
        }
    }

    private IEnumerator MoleDisappearanceRoutine()
    {
        float currentDisappearanceLerpAlpha = Mathf.InverseLerp(_MoleShownLocation.y, _MoleHiddenLocation.y, transform.position.y);
        while (currentDisappearanceLerpAlpha < 1)
        {
            if (_IsDisappearancePaused)
            {
                yield return null;
                continue;
            }
            transform.position = Vector3.Lerp(_MoleShownLocation, _MoleHiddenLocation, currentDisappearanceLerpAlpha);
            currentDisappearanceLerpAlpha += Time.deltaTime / _DisappearanceDuration;
            yield return null;
        }
        //add hole back to usable holes after the mole finished its disappearance routine
        MoleWacker.Instance._HolesTenants.Add(_OccupiedHole);
        MoleWacker.Instance._Moles.Remove(gameObject);
        Destroy(gameObject);
    }
    private IEnumerator MoleSpawnMovementRoutine()
    {
        //TODO: play mole appearance anim
        while (_MovementLerpAlpha < 1)
        {
            if (_IsWacked)
            {
                yield break;
            }
            transform.position = Vector3.Lerp(_MoleHiddenLocation, _MoleShownLocation, _MovementLerpAlpha);
            _MovementLerpAlpha += Time.deltaTime / _AppearanceDuration;
            yield return null;
        }
        yield return new WaitForSeconds(_PersistenceTime);
        if (!_IsWacked)
        {
            MakeMoleDisappear();
        }
    }
}
