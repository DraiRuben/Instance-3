using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mole : MonoBehaviour,IPointerClickHandler
{
    public float _PersistenceTime;
    public float _MovementSpeed;
    public int _OccupiedHole;
    private float _MovementLerpAlpha;
    private void Start()
    {
        StartCoroutine(MoleSpawnMovementRoutine());
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    private IEnumerator MoleSpawnMovementRoutine()
    {
        yield break;
    }
}
