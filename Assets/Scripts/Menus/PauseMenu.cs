using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public bool _IsPauseBlocked;
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        gameObject.SetActive(false);
    }
    private void Update()
    {
        Cursor.visible = true;
    }
}
