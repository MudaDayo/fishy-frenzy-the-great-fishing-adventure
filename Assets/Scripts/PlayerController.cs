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
    private float boostDuration, boostSteering, respawnDuration, afkSlowdown;

    private float boostTimer, respawnTimer;
    [SerializeField]
    private Vector3 startPosition;

    [SerializeField]
    private GameObject smoke, fireSmoke, playerBase, hitBox, brokenSmoke, otherShip, smallSmoke;
    [SerializeField]
    private float gravityValue = -9.81f;

    [SerializeField]
    private float speedBoostModifier = 1.5f;

    private Vector3 move, lastMove;

    private Vector2 movementInput = Vector2.zero;
    private bool boosting = false;
    private bool switching = false;
    private bool canBoost = true;
    private bool lastFrameSwitching;

    //tried to use enum, too lazy... 0 for nothing, 1 to move, 2 to fish
    public int lastAction = 0;

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

    public void OnSwitch(InputAction.CallbackContext context)
    {
        switching = context.action.triggered;
    }

    void Update()
    {
        Respawn();

        if(lastAction == 1)
        {
            controller.Move(lastMove * Time.deltaTime * playerSpeed * afkSlowdown);
            return;
        }

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        move = new Vector3(movementInput.x, 0, movementInput.y).normalized;
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

        if (boostTimer < boostDuration)
        {
            if (boostTimer == 0f)
            {
                boostDirection = new Vector3(movementInput.x, 0, movementInput.y).normalized;
            }
            boostTimer += Time.deltaTime;

            Vector3 boostManeuver = new Vector3(movementInput.x, 0, movementInput.y).normalized * boostSteering;
            boostDirection = new Vector3(boostDirection.x + boostManeuver.x, 0f, boostDirection.z + boostManeuver.z).normalized;

            fireSmoke.SetActive(true);
            smoke.SetActive(false);
            smallSmoke.SetActive(false);
            controller.Move(boostDirection * Time.deltaTime * playerSpeed * speedBoostModifier);

            gameObject.transform.forward = boostDirection * Time.deltaTime * playerSpeed * speedBoostModifier;
        }
        else
        {
            if(canBoost)
            {
                smoke.SetActive(true);
                smallSmoke.SetActive(false);
            }
            else
            {
                smallSmoke.SetActive(true);
                smoke.SetActive(false);
            }
            

            fireSmoke.SetActive(false);
            hitBox.SetActive(false);
        }

        lastFrameSwitching = switching;

        SwitchBoat();

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (gameObject.GetComponent<PlayerInput>().enabled)
        {
            lastAction = 0;
        }
    }

    private void Respawn()
    {
        if (respawnTimer < respawnDuration)
        {
            respawnTimer += Time.deltaTime;
            return;
        }
        else if (brokenSmoke.activeSelf)
        {
            brokenSmoke.SetActive(false);
        }
    }

    private void SwitchBoat()
    {
        if (switching)
        {
            gameObject.GetComponent<PlayerInput>().enabled = false;

            otherShip.GetComponent<PlayerInput>().enabled = false;
            otherShip.GetComponent<PlayerInput>().enabled = true;

            otherShip.GetComponent<PlayerController>().lastAction = 0;

            switching = false;

            lastMove = move;
            if (lastMove != Vector3.zero && !(boostTimer < boostDuration))
            {
                lastAction = 1;
            }
            else
            {
                lastAction = 0;
            }
        }

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
            if ((boosting && !(boostTimer < boostDuration) && move == Vector3.zero) || lastAction == 2)
            {
                //add fishing code here 
                Debug.Log("Fishing");
                
                //this is for passive fishing
              lastAction = 2;
            }
        }
    }
}
