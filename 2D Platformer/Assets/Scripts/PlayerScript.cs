using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public CharacterController controller;
    [SerializeField] private float speed;
    private Vector3 direction;
    public float jumpForce;
    public float gravity;

    public Transform groundCheck;
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
        direction.x = hInput * speed;

        bool isGrounded = Physics.CheckSphere(groundCheck.position, 0.15f, groundLayer);

        direction.y += gravity * Time.deltaTime;

        if (isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                direction.y = jumpForce;
            }
        }
        

        controller.Move(direction * Time.deltaTime);

        mainCamera.transform.position = new Vector3(this.transform.position.x, 0, -10);
    }
}
