using UnityEngine;
using UnityEngine.Splines;

public class FishBehavior : MonoBehaviour
{
    public SplineContainer[] _Spline;
    [SerializeField] private float _Speed;
    [SerializeField] private bool _IsBugged;
    private float _DistPercent = 0;
    private float _SplineLength;
    

    private void Start()
    {
        _SplineLength = _Spline[FishingManager._FishInstance._BugValue].CalculateLength();
    }

    private void Update()
    {

        _DistPercent += _Speed * Time.deltaTime / _SplineLength;

        Vector3 currentPosition = _Spline[FishingManager._FishInstance._BugValue].EvaluatePosition(_DistPercent);
        transform.position = currentPosition;

        if (_DistPercent > 1f)
        {
            _DistPercent = 0f;
        }
    }
}
