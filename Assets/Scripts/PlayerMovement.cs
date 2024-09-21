using UnityEngine;

public interface IMovable
{
    void Move(Vector3 direction);
}

public class PlayerMovement : MonoBehaviour, IMovable
{
    public float speed = 5f;
    [SerializeField] private Rigidbody rigidbodyPlayer;
    private Vector3 movementInput;
    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        movementInput = new Vector3(moveX, 0f, moveZ);
    }
    private void FixedUpdate()
    {
        if (movementInput.magnitude > 0)
        {
            Move(movementInput);
        }
    }

    public void Move(Vector3 direction)
    {
        Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
        rigidbodyPlayer.MovePosition(transform.position + move);
    }
}
