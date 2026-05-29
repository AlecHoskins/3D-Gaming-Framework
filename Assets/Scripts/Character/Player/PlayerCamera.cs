using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;

    public Camera cameraObject;
    public PlayerManager player;
    [SerializeField] Transform cameraPivotTransform;

    #region CHANGEABLE SETTINGS TO CHANGE CAMERA BEHAVIOR

    [Header("Camera Settings")]
    private float cameraSmoothSpeed; //higher value == slower camera
    [SerializeField] float horizontalRotationSpeed;
    [SerializeField] float verticalRotationSpeed;
    [SerializeField] float minPivot;
    [SerializeField] float maxPivot;

    #endregion

    #region  JUST DISPLAYS CAMERA VALUES

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    [SerializeField] float horizontalLookAngle; //lowest point you can look down 
    [SerializeField] float verticalLookAngle; //highest point you can look up 

    #endregion

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            cameraVelocity = Vector3.zero;
            cameraSmoothSpeed = 1;
            verticalRotationSpeed = 220;
            horizontalRotationSpeed = 220;
            minPivot = -30;
            maxPivot = 60;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void HandleAllCameraActions()
    {
        if(player != null)
        {
            //follow player
            HandleFollowTarget();

            //rotate around player
            HandleRotations();

            //collide with objects
        }


    }

    private void HandleFollowTarget()
    {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(
            transform.position, 
            player.transform.position, 
            ref cameraVelocity, 
            cameraSmoothSpeed * Time.deltaTime
        );
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        // if locked on, force rotation towards target

        //normal rotations
        horizontalLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * horizontalRotationSpeed) * Time.deltaTime;
        verticalLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * verticalRotationSpeed) * Time.deltaTime;

        //enforce min/max
        verticalLookAngle = Mathf.Clamp(verticalLookAngle, minPivot, maxPivot);

        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        //IN TERMS OF CAMERA Y IS ACTUALLY HORIZONTAL, X IS VERTICAL

        //horizontal
        cameraRotation.y = horizontalLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation); 
        transform.rotation = targetRotation;

        //vertical
        cameraRotation = Vector3.zero;
        cameraRotation.x = verticalLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }
}
