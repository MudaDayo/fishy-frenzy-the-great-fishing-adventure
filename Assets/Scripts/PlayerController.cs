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
    private string enemyTag;

    [SerializeField]
    private float boostDuration, boostSteering, respawnDuration;

    private float boostTimer, respawnTimer;
    [SerializeField]
    private Vector3 startPosition;

    [SerializeField]
    private GameObject smoke, fireSmoke, playerBase, hitBox, brokenSmoke;
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

        startPosition = transform.position;

        boostTimer = boostDuration;
        respawnTimer = respawnDuration;

        hitBox.SetActive(false);
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

        if(respawnTimer < respawnDuration)
        {
            respawnTimer += Time.deltaTime;
            return;
        }
        else if(brokenSmoke.activeSelf)
        {
            brokenSmoke.SetActive(false);
        }

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
            hitBox.SetActive(true);
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
            hitBox.SetActive(false);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
        if (other.gameObject == playerBase)
        {
            canBoost = true;
        }
        if (other.gameObject.tag == enemyTag)
        {
            controller.enabled = false;
            transform.position = startPosition;
            canBoost = true;
            controller.enabled = true;
            respawnTimer = 0f;
            brokenSmoke.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "FishingZone")
        {
            if (boosting && !(boostTimer < boostDuration) && move == Vector3.zero)
            {
                //add fishing code here 
            }
        }
    }
}
