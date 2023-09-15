using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 5f;
    public float gravity = -9.81f;
    public float pickupRange = 2f;

    private CharacterController characterController;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private Vector3 playerVelocity;
    private bool isGrounded;

    private List<GameObject> inventory = new List<GameObject>();
    private GameObject currentWeapon;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Initialize the current weapon
        currentWeapon = null;

        // Initialize inventory and current weapon
        inventory.Clear();
        currentWeapon = null;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();

        // Jumping
        HandleJump();

        // Try to pick up weapons
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }

        // Handle weapon firing
        if (Input.GetButtonDown("Fire1") && currentWeapon != null)
        {
            // ... (code for firing the current weapon)
        }
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;
        movement.y = 0f;

        characterController.Move(movement * movementSpeed * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleJump()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && playerVelocity.y < 0f)
        {
            playerVelocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = jumpForce;
        }

        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, pickupRange))
        {
            GameObject weaponObject = hit.collider.gameObject; // Get the weapon object
            if (weaponObject.CompareTag("weaponPickup")) // Check the tag
            {
                PickUpWeapon(weaponObject);
            }
        }
    }

    public void PickUpWeapon(GameObject weaponObject)
    {
        if (!inventory.Contains(weaponObject))
        {
            // Add the weapon to the player's inventory
            inventory.Add(weaponObject);

            // Set the weapon object as a child of the player
            weaponObject.transform.SetParent(transform);

            // Disable the weapon object initially
            weaponObject.SetActive(false);

            // Set the current weapon to the newly picked-up weapon
            currentWeapon = weaponObject;

            // ... (other code for handling weapon switching)
        }
    }

    // ... (other methods for switching weapons, firing, etc.)
}
