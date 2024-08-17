using UnityEngine;
using Photon.Pun;

public class touchcontroller : MonoBehaviour
{
    private bool isRotating;
    private Vector2 prevTouchPos;
    public float rotationSpeed = 2f;
    public float cameraRotationLimitX = 90f; // Adjust this limit for the camera
    public RectTransform touchPanel; // Reference to the panel in which touch input is allowed
    public Transform playerCamera;
    public Transform playerBody;
    public float updown = 1f;
    float xRotation = 0f;
    PhotonView PV;
    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Canvas>().gameObject);
        }
    }


    void Update()
    {
        if (PV.IsMine)
        {
            touchcon();
        }
        else
        {
            return;
        }
    }
    void touchcon()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the touch position is within the panel
            if (IsTouchInPanel(touch.position))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    isRotating = true;
                    prevTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved && isRotating)
                {
                    Vector2 deltaTouch = touch.position - prevTouchPos;

                    float rotateX = deltaTouch.y * rotationSpeed;
                    float rotateY = deltaTouch.x * rotationSpeed;

                    // Adjust the camera rotation around the X-axis
                    xRotation -= rotateX;
                    xRotation = Mathf.Clamp(xRotation, -cameraRotationLimitX, cameraRotationLimitX);
                    playerCamera.localRotation = Quaternion.Euler(xRotation * updown, 0f, 0f);

                    // Rotate the player body around the Y-axis
                    playerBody.Rotate(Vector3.up * rotateY);

                    prevTouchPos = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isRotating = false;
                }
            }
        }
    }
    // Check if the touch position is within the panel
    private bool IsTouchInPanel(Vector2 touchPosition)
    {
        if (touchPanel == null)
        {
            // If no panel is assigned, allow touch input everywhere
            return true;
        }

        // Convert touch position to local coordinates of the panel
        Vector2 localTouchPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(touchPanel, touchPosition, null, out localTouchPos);

        return touchPanel.rect.Contains(localTouchPos);
    }
}

