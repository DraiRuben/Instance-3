using UnityEngine;
using UnityEngine.Splines;

public class Fish : MonoBehaviour
{
    public SplineContainer[] _Spline;
    private Animator _Animator;
    private float _Speed;
    [SerializeField] private float _BaseSpeed;
    [SerializeField] private bool _IsBugged;
    private float _DistPercent = 0;
    private float _SplineLength;
    private bool _IsDying;
    private void Awake()
    {
        _Animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _Speed = _BaseSpeed * FishManager.Instance._SpeedMult;
        _SplineLength = _Spline[FishManager.Instance._BugValue].CalculateLength();
    }
    public void DoDestructionFeedback()
    {
        _IsDying = true;
        //play anim
        _Animator.SetTrigger("Fish");
        this.Invoke(() =>
        {
            FishManager.Instance._FishList.Remove(transform.parent.gameObject);
            Destroy(transform.parent.gameObject);
        }, 1.0f);
        //TODO: play sound
    }
    private void Update()
    {
        if (!_IsDying)
        {
            _DistPercent += _Speed * Time.deltaTime / _SplineLength;
            Vector3 currentPosition = _Spline[FishManager.Instance._BugValue].EvaluatePosition(_DistPercent);
            if (currentPosition.x < transform.parent.position.x)
            {
                transform.parent.rotation = new Quaternion(0, 180, 0, 0);
            }
            else
            {
                transform.parent.rotation = new Quaternion(0, 0, 0, 0);
            }
            transform.parent.position = currentPosition;
            if (_DistPercent > 1f)
            {
                _DistPercent = 0f;
            }
        }

    }
}
