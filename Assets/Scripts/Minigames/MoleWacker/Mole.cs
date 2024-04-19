using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class Mole : MonoBehaviour,IPointerClickHandler
{
    public float _PersistenceTime;
    public float _AppearanceDuration;
    public int _OccupiedHole;

    private float _MovementLerpAlpha;
    private bool _IsDisappearing;
    private bool _IsWacked;

    [SerializeField] private float _DisappearanceDuration;
    [SerializeField] private float _ShowLocationYOffset = 1.5f;


    private Vector3 _MoleHiddenLocation;
    private Vector3 _MoleShownLocation;
    private void Start()
    {
        _MoleHiddenLocation = transform.position;
        _MoleShownLocation = transform.position + new Vector3(0.0f, _ShowLocationYOffset, 0.0f);
        StartCoroutine(MoleSpawnMovementRoutine());
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        MakeMoleDisappear(true);
    }
    public void SetLayer(int layer)
    {
        if (layer >= 0 && layer < 3)
            GetComponent<SortingGroup>().sortingLayerName = $"Layer{layer+1}Moles";
    }
    private void MakeMoleDisappear(bool _KillMole = false)
    {
        if (_MovementLerpAlpha > 0.5f)
        {
            if (!_IsDisappearing)
            {
                StartCoroutine(MoleDisappearanceRoutine());
                _IsWacked = _KillMole;
                _IsDisappearing = true;
                if (_IsWacked)
                {
                    MoleWacker.Instance._WinCount++;
                    MoleWacker.Instance.OnMoleWacked.Invoke();
                }
                else
                {
                    MoleWacker.Instance._LoseCount++;
                    MoleWacker.Instance.OnMoleLost.Invoke();
                }
                    
                //TODO: play either wacked anim or fuck you anim
            }
        }
    }
    private IEnumerator MoleDisappearanceRoutine()
    {
        float currentDisappearanceLerpAlpha = 0.0f;
        while (currentDisappearanceLerpAlpha < 1)
        {
            transform.position = Vector3.Lerp(_MoleShownLocation,_MoleHiddenLocation,currentDisappearanceLerpAlpha);
            currentDisappearanceLerpAlpha += Time.deltaTime/_DisappearanceDuration;
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
