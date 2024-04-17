using UnityEngine;
using UnityEngine.Splines;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private GameObject _Fish;
    [SerializeField] private SplineContainer[] _Splines;
    private int i = 0;
    private void Update()
    {
        while (i< 6)
        {
            GameObject Fish = Instantiate(_Fish, new Vector2(3,0), Quaternion.identity);
            Fish.GetComponent<FishBehavior>()._Spline = _Splines;
            i++;
        }
    }
}
