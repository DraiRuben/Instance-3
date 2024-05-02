using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu instance;

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
