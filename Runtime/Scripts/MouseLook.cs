using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    private FirstAndThirdPersonCharacterInputs inputActions;
    public float mouseSensitivity = 80f;
    public Transform playerBody;
    private float xRotation = 0f;
    public float lookUpMaxAngle = 90f;
    public float lookDownMaxAngle = 90f;

    private void Awake()
    {
        inputActions = new FirstAndThirdPersonCharacterInputs();
        inputActions.CharacterControls.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked; //lock the cursor to the center of the screen
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseInput = inputActions.CharacterControls.MouseMovement.ReadValue<Vector2>(); //read mouse input
        float mouseX = mouseInput.x * mouseSensitivity * Time.deltaTime; // store mouse input, apply sensitivity and smooth with delta time
        float mouseY = mouseInput.y * mouseSensitivity * Time.deltaTime;
       
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -lookUpMaxAngle, lookDownMaxAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
