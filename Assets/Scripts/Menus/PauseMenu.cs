using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        gameObject.SetActive(false);
    }
}
