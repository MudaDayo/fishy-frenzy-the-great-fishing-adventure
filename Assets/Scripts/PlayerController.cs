using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    private bool hasCaughtFish = false;
    [SerializeField] private float fishingTime = 6f;
    private float currentFishingTime = 0f;
    public int lastAction = 0;
    private ScoreManager scoreManager;

    //skillcheck
    public Image skillCheckBar;
    public Image successZone;
    public Image Indicator;

    public float skillCheckSpeed = 200f;

    private bool isSkillCheckActive = false;
    private float skillCheckBarStartPosition;
    private bool skillCheckSuccess = false;


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();
        startPosition = transform.position;
        boostTimer = boostDuration;
        respawnTimer = respawnDuration;
        hitBox.SetActive(false);
        scoreManager = FindObjectOfType<ScoreManager>();
        //skillcheck
        skillCheckBarStartPosition = skillCheckBar.transform.localPosition.x;
        skillCheckBar.gameObject.SetActive(false);
        successZone.gameObject.SetActive(false);
        Indicator.gameObject.SetActive(false);
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

                fishing = false;
                currentFishingTime = 0f; // Reset the fishing timer
            }
        }
        else
        {
            // Reset the fishing timer if the player is not fishing
            currentFishingTime = 0f;
        }

        //skillcheck
        if (isSkillCheckActive)
        {
            UpdateSkillCheck();
        }
        
    }

    public void ResetPosition()
    {
        
        transform.position = startPosition;
    }

    public void ResetBoost()
    {
        
        canBoost = true;
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

        // Check if the player is within the fishing zone
        if (other.gameObject.CompareTag("FishingZone"))
        {
            StartSkillCheck();

            // Set the visibility of the UI elements to true
            skillCheckBar.gameObject.SetActive(true);
            successZone.gameObject.SetActive(true);
            Indicator.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerBase)
        {
            isInBase = false;
        }

        if (other.gameObject.CompareTag("FishingZone"))
        {
            StopFishing();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("FishingZone"))
        {
            // Check if the player is standing still
            if (controller.velocity.magnitude < 0.1f)
            {
                // Start fishing
                
                // Activate the skill check when starting fishing
                StartSkillCheck();

                // Set the visibility of the UI elements to true
                //skillCheckBar.gameObject.SetActive(true);
                //successZone.gameObject.SetActive(true);
                //Indicator.gameObject.SetActive(true);
            }
            else
            {
                // Stop fishing if the player moves
                StopFishing();
            }
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

    private void StartFishing()
    {
        
        if (skillCheckSuccess)
        {
            fishing = true;
            Debug.Log("Started fishing");
        }
    }

    private void StopFishing()
    {
        fishing = false;
        // If the player stops fishing, hide the skill check elements
        if (isSkillCheckActive)
        {
            EndSkillCheck();
        }
    }

    private void UpdateSkillCheck()
    {
        // Calculate the maximum and minimum positions for the indicator
        float maxPosY = skillCheckBar.transform.localPosition.y + (skillCheckBar.rectTransform.rect.height / 2);
        float minPosY = skillCheckBar.transform.localPosition.y - (skillCheckBar.rectTransform.rect.height / 2);

        // Move the indicator vertically
        Indicator.transform.localPosition += Vector3.up * skillCheckSpeed * Time.deltaTime;

        // Check if the indicator reaches the top or bottom
        if (Indicator.transform.localPosition.y >= maxPosY)
        {
            // If the indicator reaches the top, move it back down
            Indicator.transform.localPosition = new Vector3(Indicator.transform.localPosition.x, maxPosY, Indicator.transform.localPosition.z);
            skillCheckSpeed *= -1; // Reverse the direction
        }
        else if (Indicator.transform.localPosition.y <= minPosY)
        {
            // If the indicator reaches the bottom, move it back up
            Indicator.transform.localPosition = new Vector3(Indicator.transform.localPosition.x, minPosY, Indicator.transform.localPosition.z);
            skillCheckSpeed *= -1; // Reverse the direction
        }

        // Check for success condition
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSkillCheckResult();
        }
    }

    private void CheckSkillCheckResult()
    {
        float barPosition = skillCheckBar.transform.localPosition.x;

        // Check if the skill check bar is within the success zone
        if (barPosition <= successZone.transform.localPosition.x + (successZone.rectTransform.rect.width / 2))
        {
            skillCheckSuccess = true;
            skillCheckBar.gameObject.SetActive(false);
            successZone.gameObject.SetActive(false);
            Indicator.gameObject.SetActive(false);
        }
        else
        {
            skillCheckSuccess = false;
        }

        // End the skill check
        EndSkillCheck(); // Call EndSkillCheck method to hide the skill check
    }

    private void FailSkillCheck()
    {
        // Handle skill check failure
        skillCheckSuccess = false;
        skillCheckBar.gameObject.SetActive(false);
        successZone.gameObject.SetActive(false);
        Indicator.gameObject.SetActive(false);
        EndSkillCheck(); // Call EndSkillCheck method to hide the skill check
    }

    private void EndSkillCheck()
    {
        // Disable skill check UI elements
        skillCheckBar.gameObject.SetActive(false);
        successZone.gameObject.SetActive(false);
        Indicator.gameObject.SetActive(false);
        // Set skill check flag to inactive
        isSkillCheckActive = false;

        // Perform actions based on skill check result
        if (skillCheckSuccess)
        {
            // Success actions
            StartFishing();
        }
        else
        {
            // Failure actions
        }
    }

    // Method to start the skill check
    private void StartSkillCheck()
    {
        // Set skill check UI elements active
        skillCheckBar.gameObject.SetActive(true);
        successZone.gameObject.SetActive(true);

        // Reset skill check bar position
        skillCheckBar.transform.localPosition = new Vector3(skillCheckBarStartPosition, skillCheckBar.transform.localPosition.y, skillCheckBar.transform.localPosition.z);

        // Set skill check flag to active
        isSkillCheckActive = true;
    }

}