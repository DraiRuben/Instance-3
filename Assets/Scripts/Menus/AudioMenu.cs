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
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
}
