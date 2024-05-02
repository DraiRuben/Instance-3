using System.Collections;
using UnityEngine;

public class NPCPathFind : MonoBehaviour
{
    [SerializeField] private GameObject[] _Node;
    [SerializeField] private float _Duration;
    private float _ElapsedTime;
    private int _RandomNode;

    private Vector3 _StartNode;
    private Vector3 _EndNode;

    private void Start()
    {
        _RandomNode = Random.Range(0, _Node.Length);
        _StartNode = _Node[_RandomNode].transform.position;
        transform.position = _StartNode;
        _EndNode = _StartNode;
    }
    private void Update()
    {
        StartCoroutine(CheckDistance());
        LearpTo();
    }

    public IEnumerator CheckDistance()
    {
        if (Vector3.Distance(transform.position, _EndNode) <= 0.001f)
        {
            _ElapsedTime = 0.0f;
            _Duration = Random.Range(2f, 4f);
            _StartNode = _EndNode;

            while (_EndNode == _StartNode)
            {
                _EndNode = _Node[Random.Range(0, _Node.Length)].transform.position;
            }
        }
        yield return new WaitForFixedUpdate();
    }

    private void LearpTo()
    {
        _ElapsedTime += Time.deltaTime;
        float _PercentageComplete = _ElapsedTime / _Duration;
        transform.position = Vector3.Lerp(_StartNode, _EndNode, Mathf.SmoothStep(0, 1, _PercentageComplete));
    }
}
