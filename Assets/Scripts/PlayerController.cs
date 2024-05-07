using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField]
    private float playerSpeed = 2.0f;

    [SerializeField]
    private GameObject smoke, fireSmoke;
/*    [SerializeField]
    private float gravityValue = -9.81f;*/

    [SerializeField]
    private float speedBoostModifier = 1.5f;

    private Vector2 movementInput = Vector2.zero;
    private bool boosting = false;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext context)
    {
        boosting = context.action.triggered;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 move = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        controller.Move(move * Time.deltaTime * playerSpeed);

        if (move != Vector3.zero)
        {
            gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (boosting)
        {
            fireSmoke.SetActive(true);
            smoke.SetActive(false);
            Vector3 boostMove = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            controller.Move(boostMove * Time.deltaTime * playerSpeed * speedBoostModifier);
        }
        else
        {
            smoke.SetActive(true);
            fireSmoke.SetActive(false);
        }

/*        playerVelocity.y += gravityValue * Time.deltaTime;*/
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
