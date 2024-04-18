using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private GameObject _Fish;
    [SerializeField] private SplineContainer[] _Splines;
    private int i = 0;
    private void Start()
    {
        StartCoroutine(FishSpawn());
    }

    IEnumerator FishSpawn()
    {
        if (i < 6)
        {
            GameObject Fish = Instantiate(_Fish,new Vector2(5000,5000), Quaternion.identity);
            Fish.GetComponent<FishBehavior>()._Spline = _Splines;
            i++;
            yield return new WaitForSeconds(1);
            StartCoroutine(FishSpawn());
        }
    }
}
