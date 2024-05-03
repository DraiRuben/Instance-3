using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class NPCPathFind : MonoBehaviour
{
    [SerializeField] private GameObject[] _Node;
    private float _MovementSpeed;
    private float _ElapsedTime;
    private int _RandomNode;
    private float _PathLength;
    private Vector3 _StartNode;
    private Vector3 _EndNode;
    [SerializeField, MinMaxSlider(0, 10)] private Vector2 _WaitTime;
    [SerializeField, MinMaxSlider(0, 10)] private Vector2 _Speed;

    private void Start()
    {
        _RandomNode = Random.Range(0, _Node.Length);
        _StartNode = _Node[_RandomNode].transform.position;
        transform.position = _StartNode;
        _EndNode = _StartNode;
    }
    private void OnEnable()
    {
        StartCoroutine(CheckDistance());

    }
    private void Update()
    {
        LerpTo();
    }

    public IEnumerator CheckDistance()
    {
        while (true)
        {
            if (Vector3.Distance(transform.position, _EndNode) <= 0.001f)
            {
                _ElapsedTime = 0.0f;
                _MovementSpeed = Random.Range(_Speed.x, _Speed.y);
                _StartNode = _EndNode;
                while (_EndNode == _StartNode)
                {
                    _EndNode = _Node[Random.Range(0, _Node.Length)].transform.position;
                }
                _PathLength = Vector3.Distance(transform.position, _EndNode);
                yield return new WaitForSeconds(Random.Range(_WaitTime.x, _WaitTime.y));
            }
            yield return new WaitForFixedUpdate();
        }

    }

    private void LerpTo()
    {
        _ElapsedTime += Time.deltaTime * _MovementSpeed / _PathLength;
        transform.position = Vector3.Lerp(_StartNode, _EndNode, Mathf.SmoothStep(0, 1, _ElapsedTime));
    }
}
