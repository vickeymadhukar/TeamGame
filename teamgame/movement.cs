using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class movement : MonoBehaviour
{
    public float speed = 10f;
    public CharacterController controller;
    public float gravity = -9.8f;
    Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask ground;
    bool isGrounded;
    public float jumpHeight = 3f;
    public AudioSource walksound;
    public FixedJoystick joystick;

    public Transform playerCamera;
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;

    private float defaultPosY;
    private float timer = 0f;

    public GameObject playercanvas;

    PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (!PV.IsMine && GetComponent<movement>() != null)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            playercanvas.SetActive(false);
            Destroy(GetComponent<movement>());
        }
    }

    void Start()
    {
       
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Debug.Log("CANVAS DESTROYED");
        }
        else
        {
            playercanvas.SetActive(true);
            
            PV.TransferOwnership(PhotonNetwork.LocalPlayer);
        }


        walksound = GetComponent<AudioSource>();

        // Assuming the main camera is the player's camera
        if (playerCamera == null)
        {
            playerCamera = Camera.main.transform;
        }

        defaultPosY = playerCamera.localPosition.y;
    }

    void Update()
    {
        if (!PV.IsMine)
        {
            controller.enabled = false;
            return;
        }
        else
        {
            controller.enabled = true;
            playermovement();
        }

    }



    void playermovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, ground);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * gravity * -2f);
        }

        float moveX = joystick.Horizontal; // Get horizontal joystick input
        float moveZ = joystick.Vertical; // Get vertical joystick input

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (move.magnitude > 0.1f)
        {
            if (!walksound.isPlaying)
            {
                walksound.Play();
            }
        }
        else
        {
            walksound.Stop();
        }

        if (Mathf.Abs(joystick.Horizontal) > 0.1f || Mathf.Abs(joystick.Vertical) > 0.1f)
        {
            HeadBobbing();
        }
        else
        {
            timer = 0f;
            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, Mathf.Lerp(playerCamera.localPosition.y, defaultPosY, Time.deltaTime * bobbingSpeed), playerCamera.localPosition.z);
        }
    }
    void HeadBobbing()
    {
        float waveSlice = 0.0f;
        float horizontal = Mathf.Abs(joystick.Horizontal);
        float vertical = Mathf.Abs(joystick.Vertical);

        if (horizontal == 0 && vertical == 0)
        {
            timer = 0.0f;
        }
        else
        {
            waveSlice = Mathf.Sin(timer);
            timer += bobbingSpeed;

            if (timer > Mathf.PI * 2)
            {
                timer -= Mathf.PI * 2;
            }
        }

        if (waveSlice != 0)
        {
            float translateChange = waveSlice * bobbingAmount;
            float totalAxes = Mathf.Clamp(horizontal + vertical, 0.0f, 1.0f);
            float translateValue = totalAxes * translateChange;

            playerCamera.localPosition = new Vector3(playerCamera.localPosition.x, defaultPosY + translateValue, playerCamera.localPosition.z);
        }
      
    }

}
