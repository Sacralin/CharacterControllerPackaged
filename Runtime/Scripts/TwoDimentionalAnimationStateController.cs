using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Code Source: https://www.youtube.com/watch?v=_J8RPIaO2Lc

public class TwoDimentionalAnimationStateController : MonoBehaviour
{
    Animator animator;
    FirstAndThirdPersonCharacterInputs inputActions;
    public float velocityZ = 0f;
    float velocityX = 0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;
    
    //increase performance
    int velocityZHash;
    int velocityXHash;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        inputActions = new FirstAndThirdPersonCharacterInputs();
        inputActions.CharacterControls.Enable();

        //increase Performance
        velocityZHash = Animator.StringToHash("Velocity Z");
        velocityXHash = Animator.StringToHash("Velocity X");
    }

    public void ChangeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backwardPressed, bool runPressed, float currentMaxVelocity)
    {
        //if player pressed forward, increase velocity in Z direction
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        //increase velocity in the backward direction
        if (backwardPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }

        //increase velocity in left direction
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        //increase velocity in right direction
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        //decrease velocityZ
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }

        //increase velocityZ
        if(!backwardPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

        //increase velocityX if left is not pressed and velocityX < 0
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }

        //decrease velocityX if right is not pressed and velocityX > 0
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    void LockOrResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool backwardPressed, bool runPressed, float currentMaxVelocity)
    {
        //reset velocityZ
        //if (!forwardPressed && velocityZ < 0.0f)
        //{
        //    velocityZ = 0.0f;
        //}
        if(!forwardPressed && !backwardPressed && velocityZ != 0.0f &&(velocityZ > -0.05f && velocityZ < 0.05f))
        {
            velocityZ = 0.0f;
        }

        //reset velocityX 
        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0.0f;
        }

        //lock forward -- sets velocity to max velocity 
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        //decelerate to the maximum walk velocity
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        //round to the currentMaxVelocity if within offset
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }

        //lock Backward -- sets velocity to max velocity 
        if (backwardPressed && runPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        //decelerate to the maximum walk velocity
        else if (backwardPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityZ < -currentMaxVelocity && velocityZ > (-currentMaxVelocity - 0.05f))
            {
                velocityZ = -currentMaxVelocity;
            }
        }
        //round to the currentMaxVelocity if within offset
        else if (backwardPressed && velocityZ > -currentMaxVelocity && velocityZ < (-currentMaxVelocity + 0.05f))
        {
            velocityZ = -currentMaxVelocity;
        }

        //lock Left -- sets velocity to max velocity 
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        //decelerate to the maximum walk velocity
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        //round to the currentMaxVelocity if within offset
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }

        //lock Right-- sets velocity to max velocity 
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        //decelerate to the maximum walk velocity
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            //round to the currentMaxVelocity if within offset
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        //round to the currentMaxVelocity if within offset
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movementInput = inputActions.CharacterControls.Walk.ReadValue<Vector2>();
        bool forwardPressed = movementInput.y > 0f;
        bool backwardPressed = movementInput.y < 0f;
        bool rightPressed = movementInput.x < 0f;
        bool leftPressed = movementInput.x > 0f;
        bool runPressed = inputActions.CharacterControls.Run.ReadValue<float>() > 0;

        //set current maxVelocity
        float currentMaxVelocity;
        if (animator.GetBool("isCrouched")) { currentMaxVelocity = maximumWalkVelocity; }
        else { currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity; }

        ChangeVelocity(forwardPressed, leftPressed, rightPressed, backwardPressed, runPressed, currentMaxVelocity);
        LockOrResetVelocity(forwardPressed, leftPressed, rightPressed, backwardPressed, runPressed, currentMaxVelocity);

        animator.SetFloat(velocityZHash, velocityZ);
        animator.SetFloat(velocityXHash, velocityX);
    }
}
