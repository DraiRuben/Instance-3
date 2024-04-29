using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    public static ControlsMenu Instance;
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        gameObject.SetActive(false);
    }
}
