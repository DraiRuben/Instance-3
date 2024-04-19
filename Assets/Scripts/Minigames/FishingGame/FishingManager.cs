using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Splines;

public class FishingManager : MonoBehaviour
{
    public static FishingManager _FishInstance;
    [SerializeField] private GameObject _Fish;
    [SerializeField] private SplineContainer[] _Splines;
    [SerializeField] private TextAsset _BehaviorFile;
    public List<GameObject> _FishList = new List<GameObject>();
    public int _BugValue;
    private void Start()
    {
        if (!Directory.Exists("Game")) Directory.CreateDirectory("Game");
        if (!Directory.Exists("Game/Minigames")) Directory.CreateDirectory("Game/Minigames");
        if (!Directory.Exists("Game/Minigames/FishingGame")) Directory.CreateDirectory("Game/Minigames/FishingGame");
        if (_BehaviorFile.text == "Enabled = true;")
        {
            _BugValue = 0;
        }
        else
        {
            _BugValue = 1;
        }
        if (!_FishInstance)
        {
            _FishInstance = this; 
        }

        StartCoroutine(FishSpawn());
    }

    IEnumerator FishSpawn()
    {
        while(true)
        {
            if (_FishList.Count < 6)
            {
                _FishList.Add(Instantiate(_Fish, new Vector2(0, 1), Quaternion.identity));
                _FishList[_FishList.Count - 1].GetComponent<FishBehavior>()._Spline = _Splines;
                yield return new WaitForSeconds(2);
            }
            yield return null;
        }
    }
}
