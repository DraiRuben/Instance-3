using UnityEngine;
using UnityEngine.InputSystem;

public class Pole : MonoBehaviour
{
    private bool _fishing;
    private void Update()
    {
        Vector2 MousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        MousePosition.y -= 1;
        if (!_fishing)
        {
            transform.position = MousePosition;
        }
    }

    public void Fishing(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _fishing = true;
            transform.position = new Vector2(transform.position.x, transform.position.y-0.5f);
        }
        if (context.canceled)
        {
            _fishing = false;
        }
    }
}
