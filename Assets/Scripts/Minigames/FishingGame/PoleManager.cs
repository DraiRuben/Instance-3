using UnityEngine;
using UnityEngine.InputSystem;

public class Pole : MonoBehaviour
{
    private void Update()
    {
        Vector2 MousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        MousePosition.y -= 1;
        transform.position = MousePosition;
    }
}
