using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMenu : MonoBehaviour
{
    public static AudioMenu instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        gameObject.SetActive(false);
    }
}
