using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class NPCPathFind : MonoBehaviour
{
    [SerializeField] private GameObject[] _Node;
    private float _MovementSpeed;
    private float _ElapsedTime;
    private float _PathLength;
    private Vector3 _StartNode;
    private Vector3 _EndNode;
    [SerializeField, MinMaxSlider(0, 10)] private Vector2 _WaitTime;
    [SerializeField, MinMaxSlider(0, 10)] private Vector2 _Speed;

    private void Start()
    {
        _StartNode = _Node[Random.Range(0, _Node.Length)].transform.position;
        transform.position = _StartNode;
        _EndNode = _Node[Random.Range(0, _Node.Length)].transform.position;
        _PathLength = Vector3.Distance(transform.position, _EndNode);
        _MovementSpeed = Random.Range(_Speed.x, _Speed.y);
    }
    private void OnEnable()
    {
        StartCoroutine(CheckDistance());
    }
    public IEnumerator CheckDistance()
    {
        yield return null;
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
            _ElapsedTime += Time.deltaTime * _MovementSpeed / _PathLength;
            transform.position = Vector3.Lerp(_StartNode, _EndNode, Mathf.SmoothStep(0, 1, _ElapsedTime));
            yield return new WaitForFixedUpdate();
        }

    }
}
