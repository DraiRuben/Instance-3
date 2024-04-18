using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleWacker : MonoBehaviour
{
    public static MoleWacker Instance;
    [SerializeField] private float _MinigameDuration;
    [SerializeField] private AnimationCurve _SpawnCooldownEvolution;
    [SerializeField] private float _SpawnBaseCooldown;
    [SerializeField] private AnimationCurve _MoleStayTimeEvolution;
    [SerializeField] private float _MoleStayTimeBase;
    [SerializeField] private AnimationCurve _MoleMovementSpeedEvolution;
    [SerializeField] private float _MoleMovementSpeedBase;
    [SerializeField] private GameObject _MolePrefab;
    private List<int> _HolesTenants;
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;

        _HolesTenants = new List<int>(9);
        for(int i = 0;i <9; i++)
        {
            _HolesTenants.Add(i);
        }
    }
    private void Start()
    {
        StartCoroutine(MoleSpawnRoutine());
    }
    private IEnumerator MoleSpawnRoutine()
    {
        float currentTimer = 0;
        float currentSpawnCooldown = 0.0f;
        float currentSpawnTimer = 0.0f;
        while(currentTimer < _MinigameDuration)
        {
            //spawn mole
            if (currentSpawnTimer < currentSpawnCooldown)
            {
                int chosenHole = _HolesTenants[Random.Range(0, _HolesTenants.Count)];
                _HolesTenants.Remove(chosenHole);
                var mole = Instantiate(_MolePrefab);
                mole.GetComponent<Mole>()._OccupiedHole = chosenHole;
                mole.GetComponent<Mole>()._PersistenceTime = _MoleStayTimeEvolution.Evaluate(currentTimer / _MinigameDuration)*_MoleStayTimeBase;
                mole.GetComponent<Mole>()._MovementSpeed = _MoleMovementSpeedEvolution.Evaluate(currentTimer / _MinigameDuration) * _MoleMovementSpeedBase;
                currentSpawnTimer = 0;
            }
            currentSpawnCooldown = _SpawnCooldownEvolution.Evaluate(currentTimer / _MinigameDuration)*_SpawnBaseCooldown;
            currentSpawnTimer += Time.deltaTime;
            currentTimer += Time.deltaTime;
            yield return null;
        }
    }
}
