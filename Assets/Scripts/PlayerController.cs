using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;

    private Vector3 playerVelocity, boostDirection;

    private bool groundedPlayer;

    [SerializeField] private float playerSpeed = 2.0f;

    [SerializeField] private string enemyTag;

    [SerializeField] private float boostDuration, boostSteering, respawnDuration, afkSlowdown;

    private float boostTimer, respawnTimer;

    [SerializeField] private Vector3 startPosition;
   

    [SerializeField] private GameObject smoke, fireSmoke, playerBase, hitBox, brokenSmoke, otherShip, smallSmoke, boatWithFish, boatNoFish, indicatorFish;

    [SerializeField] private float gravityValue = -9.81f;

    [SerializeField] private float speedBoostModifier = 1.5f;

    private Vector3 move, lastMove;

    private bool isInBase = false;

    private Vector2 movementInput = Vector2.zero;

    private bool boosting = false;

    private bool switching = false;

    private bool canBoost = true;

    private bool lastFrameSwitching;

    private bool fishing = false;

    private bool hasCaughtFish = false; // Flag to track if the player has already caught a fish

    [SerializeField] private float fishingTime = 6f; // Time required to catch a fish

    private float currentFishingTime = 0f; // Time spent fishing

    //tried to use enum, too lazy... 0 for nothing, 1 to move, 2 to fish

    public int lastAction = 0;

    private ScoreManager scoreManager;

    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();

        startPosition = transform.position;
        

        boostTimer = boostDuration;
        respawnTimer = respawnDuration;

        hitBox.SetActive(false);
        scoreManager = FindObjectOfType<ScoreManager>();
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
        ReturnFishToBase();

        if (respawnTimer < respawnDuration)
        {
            respawnTimer += Time.deltaTime;

            if (gameObject.GetComponent<PlayerInput>().enabled)
            {
                SwitchBoat();
            }

            return;
        }
        else if (brokenSmoke.activeSelf)
        {
            brokenSmoke.SetActive(false);
        }

        if (lastAction == 1)
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
            if (canBoost)
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

        if (switching)
        {
            SwitchBoat();
        }



        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        if (gameObject.GetComponent<PlayerInput>().enabled)
        {
            lastAction = 0;
        }

        // Fishing mechanics
        if (fishing && !hasCaughtFish)
        {
            // Increment the fishing timer
            currentFishingTime += Time.deltaTime;

            // Check if enough time has passed to catch a fish
            if (currentFishingTime >= fishingTime)
            {
                
                Debug.Log("Caught a fish!");
                hasCaughtFish = true; // Set the flag to true since the player has caught a fish

                boatNoFish.SetActive(false);
                boatWithFish.SetActive(true);

                //StopFishing();
                fishing = false;
                currentFishingTime = 0f; // Reset the fishing timer
                
            }
        }
        else
        {
            // Reset the fishing timer if the player is not fishing
            currentFishingTime = 0f;
        }
    }

    private void SwitchBoat()
    {
        gameObject.GetComponent<PlayerInput>().enabled = false;

        otherShip.GetComponent<PlayerInput>().enabled = false;
        otherShip.GetComponent<PlayerInput>().enabled = true;

        otherShip.GetComponent<PlayerController>().lastAction = 0;

        indicatorFish.SetActive(false);
        otherShip.GetComponent<PlayerController>().indicatorFish.SetActive(true);

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerBase)
        {
            canBoost = true;
            isInBase = true;
        }
        if (other.gameObject.tag == enemyTag)
        {
            lastAction = 0;
            fishing = false;
            hitBox.SetActive(false);
            boostTimer = boostDuration;
            boatWithFish.SetActive(false);
            boatNoFish.SetActive(true);

            hasCaughtFish = false;

            controller.enabled = false;
            transform.position = startPosition;
            canBoost = true;
            controller.enabled = true;
            respawnTimer = 0f;
            brokenSmoke.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerBase)
        {
            isInBase = false;
        }

        if (other.gameObject.tag == "FishingZone")
        {
            currentFishingTime = 0f;
            fishing = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "FishingZone")
        {
            // Check if the player is standing still
            if (controller.velocity.magnitude < 0.1f)
            {
                // Start fishing
                StartFishing();
            }
/*            else
            {
                // Stop fishing if the player moves
                StopFishing();
            }*/
        }
    }
    public void ResetPosition()
    {
        controller.enabled = false;

        hitBox.SetActive(false);

        transform.position = startPosition;

        

        playerVelocity = Vector3.zero;
        
        hasCaughtFish = false;

        currentFishingTime = 0f;

        isInBase = false;

        boatWithFish.SetActive(false);

        boatNoFish.SetActive(true);

        controller.enabled = true;

        lastAction = 0;

        respawnTimer = respawnDuration;
    }
    public void ResetBoost()
    {
        canBoost = true; // Reset boost state to active
    }
    private void StartFishing()
    {
        if (!fishing && !hasCaughtFish) // Check if the player is not already fishing and hasn't caught a fish yet
        {
            fishing = true;
            Debug.Log("Started fishing");
        }
    }

    private void ReturnFishToBase()
    {
            if (hasCaughtFish && isInBase)
            {
                // Assuming ScoreManager is a singleton or is available in the scene
                ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
                if (scoreManager != null)
                {
                    scoreManager.IncrementScore(gameObject.tag);
                }

                boatWithFish.SetActive(false);
                boatNoFish.SetActive(true);

                hasCaughtFish = false; // Reset the flag when the player is back to base
            }
    }
    private bool IsInFishingZone()
    {
        // Get all colliders overlapping with the player's position
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.1f);

        // Check if any of the colliders have the tag "FishingZone"
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("FishingZone"))
            {
                return true; // Player is within a collider with the FishingZone tag
            }
        }

        return false; // Player is not within any collider with the FishingZone tag
    }
}
