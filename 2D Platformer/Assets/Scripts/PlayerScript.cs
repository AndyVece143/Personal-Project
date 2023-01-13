using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Transactions;
using UnityEditor.TextCore.Text;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public CharacterController controller;
    [SerializeField] private float speed;
    public Vector3 direction;
    public float jumpForce;
    public float gravity;
    public float stopForce;
    public bool isGrounded;
    public bool isWalled;
    public bool hittingCeiling;
    public float dashForce;
    public bool dashing;
    public bool facingRight = true;
    private bool wallBreak = false;

    private bool dashLimit = false;

    public float maxSpeed;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform ceilingCheck;
    public LayerMask groundLayer;
    public Camera mainCamera;

    private void Awake()
    {
        //mainCamera = gameObject.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        
        if (hInput == 0)
        {
            maxSpeed = 5f;
            if (direction.x < 0)
            {
                direction.x += stopForce * Time.deltaTime;
                if (direction.x > 0)
                {
                    direction.x = 0;
                }    
            }
            if (direction.x > 0)
            {
                direction.x -= stopForce * Time.deltaTime;
                if (direction.x < 0)
                {
                    direction.x = 0;
                }
            }
        }

        if (hInput != 0 && dashing == false)
        {
            direction.x += hInput * speed;

            if (direction.x > maxSpeed)
            {
                direction.x = maxSpeed;
            }
            if (direction.x < -maxSpeed)
            {
                direction.x = -maxSpeed;
            }

            //Get the direction of the player
            if (direction.x > 0)
            {
                facingRight = true;
            }
            else
            {
                facingRight = false;
            }
        }

        //Collision checks
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);
        isWalled = Physics.CheckSphere(wallCheck.position, 0.1f, groundLayer);
        hittingCeiling = Physics.CheckSphere(ceilingCheck.position, 0.15f, groundLayer);

        if (hittingCeiling)
        {
            direction.y = -3;
        }


        //Wall hugging
        if (isWalled == false)
        {
            direction.y += gravity * Time.deltaTime;
            wallBreak = false;
        }
        else
        {
            if (wallBreak == false)
            {
                direction.y = 0;
                wallBreak = true;
            }
            direction.y += (gravity/3) * Time.deltaTime;
            direction.x = 0;

            //Wall jump
            if (Input.GetButtonDown("Jump"))
            {
                switch (facingRight)
                {
                    case true:
                        direction.x = -jumpForce;
                        direction.y = jumpForce;
                        break;

                    case false:
                        direction.x = jumpForce;
                        direction.y = jumpForce;
                        break;
                }
            }
        }
        

        if (isGrounded)
        {
            dashLimit = false;
            direction.y = 0;
            if (Input.GetButtonDown("Jump"))
            {
                direction.y = jumpForce;
            }
        }

        //Air dash
        if (isGrounded == false && isWalled == false && dashLimit == false)
        {
            if (Input.GetButtonDown("Jump"))
            {
                dashLimit = true;
                StartCoroutine(Dash());
            }
        }
        
        //Rotate the play based on direction
        if (facingRight == true)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (facingRight == false)
        {
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
        }


        controller.Move(direction * Time.deltaTime);

        //The camera will follow the player
        mainCamera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10);

    }

    //The dash method
    private IEnumerator Dash()
    {
        float elapsedTime = 0f;
        float duration = 0.3f;

        while (elapsedTime < duration)
        {
            dashing = true;
            maxSpeed = dashForce;

            if (facingRight == true)
            {
                direction.x = dashForce;
            }
            else
            {
                direction.x = -dashForce;
            }

            direction.y = 0;

            elapsedTime += Time.deltaTime;
            yield return null;
            dashing = false;
        }
    }
}
