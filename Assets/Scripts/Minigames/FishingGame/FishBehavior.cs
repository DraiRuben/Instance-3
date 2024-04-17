using UnityEngine;
using UnityEngine.Splines;

public class FishBehavior : MonoBehaviour
{
    public SplineContainer[] _Spline;
    [SerializeField] private float _Speed;
    [SerializeField] private bool _IsBugged;
    private float _DistPercent = 0;
    private float _SplineLength;
    private int _BugValue;
    

    private void Start()
    {
        if (_IsBugged)
        {
            _BugValue = 1;
        }
        else
        {
            _BugValue = 0;
        }
        _SplineLength = _Spline[_BugValue].CalculateLength();
    }

    private void Update()
    {

        _DistPercent += _Speed * Time.deltaTime / _SplineLength;

        Vector3 currentPosition = _Spline[_BugValue].EvaluatePosition(_DistPercent);
        transform.position = currentPosition;

        if (_DistPercent > 1f)
        {
            _DistPercent = 0f;
        }
    }
}
