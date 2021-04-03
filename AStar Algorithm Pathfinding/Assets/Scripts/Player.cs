using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rb2d;

    private void Start()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        _rb2d.velocity = new Vector2(xInput * StaticConfig.PlayerMovementSpeed * Time.deltaTime, yInput * StaticConfig.PlayerMovementSpeed * Time.deltaTime);
    }
}
