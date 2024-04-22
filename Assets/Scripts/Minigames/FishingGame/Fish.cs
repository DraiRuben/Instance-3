using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    public SplineContainer[] _Spline;

    private float _Speed;
    [SerializeField] private float _BaseSpeed;
    [SerializeField] private bool _IsBugged;
    private float _DistPercent = 0;
    private float _SplineLength;



    private void Start()
    {
        _Speed = _BaseSpeed * FishManager.Instance._SpeedMult;
        _SplineLength = _Spline[FishManager.Instance._BugValue].CalculateLength();
    }

    private void Update()
    {
        _DistPercent += _Speed * Time.deltaTime / _SplineLength;
        Vector3 currentPosition = _Spline[FishManager.Instance._BugValue].EvaluatePosition(_DistPercent);
        transform.position = currentPosition;

        if (_DistPercent > 1f)
        {
            _DistPercent = 0f;
        }
    }
}
