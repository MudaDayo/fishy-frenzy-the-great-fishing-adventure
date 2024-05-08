using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity, boostDirection;
    private bool groundedPlayer;
    [SerializeField]
    private float playerSpeed = 2.0f;

    [SerializeField]
    private float boostDuration, boostSteering;

    private float boostTimer;

    [SerializeField]
    private GameObject smoke, fireSmoke, playerBase;
    [SerializeField]
    private float gravityValue = -9.81f;

    [SerializeField]
    private float speedBoostModifier = 1.5f;

    private Vector2 movementInput = Vector2.zero;
    private bool boosting = false;
    private bool canBoost = true;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        boostTimer = boostDuration;
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
        if (!(boostTimer < boostDuration))
        {
            controller.Move(move * Time.deltaTime * playerSpeed);
            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
            }
        }

        

        if (boosting && !(boostTimer < boostDuration) && move != Vector3.zero && canBoost)
        {
            boostTimer = 0f;
            canBoost = false;
        }

        // Changes the height position of the player..
        if (boostTimer < boostDuration)
        {
            if(boostTimer == 0f)
            {
                boostDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            }
            boostTimer += Time.deltaTime;

            Vector3 boostManeuver = new Vector3(movementInput.x, 0, movementInput.y).normalized * boostSteering;
            boostDirection = new Vector3(boostDirection.x + boostManeuver.x , 0f, boostDirection.z + boostManeuver.z).normalized;

            fireSmoke.SetActive(true);
            smoke.SetActive(false);
            controller.Move(boostDirection * Time.deltaTime * playerSpeed * speedBoostModifier);

            gameObject.transform.forward = boostDirection * Time.deltaTime * playerSpeed * speedBoostModifier;
        }
        else
        {
            smoke.SetActive(true);
            fireSmoke.SetActive(false);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerBase)
        {
            canBoost = true;
        }
    }
}
