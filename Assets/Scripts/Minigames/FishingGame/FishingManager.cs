using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class FishingManager : MonoBehaviour
{
    public static FishingManager _FishInstance;
    [SerializeField] private GameObject _Fish;
    [SerializeField] private SplineContainer[] _Splines;
    public List<GameObject> _FishList = new List<GameObject>();
    private void Start()
    {
        if (!_FishInstance)
        {
            _FishInstance = this; 
        }
        StartCoroutine(FishSpawn());
    }

    IEnumerator FishSpawn()
    {
        while(true)
        {
            if (_FishList.Count < 6)
            {
                _FishList.Add(Instantiate(_Fish, new Vector2(5000, 5000), Quaternion.identity));
                _FishList[_FishList.Count - 1].GetComponent<FishBehavior>()._Spline = _Splines;
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }
}
