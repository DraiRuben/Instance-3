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
        gameObject.SetActive(false);
    }
}
