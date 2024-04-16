using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float PlayerDir;
    [SerializeField] private Rigidbody2D Rigidbody2D;

    private void movement(InputAction.CallbackContext context)
    {

    }
}
