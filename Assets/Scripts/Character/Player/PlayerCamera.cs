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
    [SerializeField] float cameraCollisionRadius; //highest point you can look up 

    #region CHANGEABLE SETTINGS TO CHANGE CAMERA BEHAVIOR

    [Header("Camera Settings")]
    private float cameraSmoothSpeed; //higher value == slower camera
    [SerializeField] float horizontalRotationSpeed;
    [SerializeField] float verticalRotationSpeed;
    [SerializeField] float minPivot;
    [SerializeField] float maxPivot;
    [SerializeField] LayerMask collideWithLayers; 

    #endregion

    #region  JUST DISPLAYS CAMERA VALUES

    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    private Vector3 cameraObjectPosition; //camera moved to this position on collision
    [SerializeField] float horizontalLookAngle; //lowest point you can look down 
    [SerializeField] float verticalLookAngle; //highest point you can look up 
    //used for camera collision
    private float cameraZPosition; 
    private float targetCameraZPosition;

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
            cameraCollisionRadius = 0.2f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    public void HandleAllCameraActions()
    {
        if(player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollision();
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

    private void HandleCollision()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        if(
            Physics.SphereCast(
                cameraPivotTransform.position, 
                cameraCollisionRadius, 
                direction, 
                out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers
            )
        ){
            //This prevents the camera from changing position due to collisions with characters.
            //TODO: Will need to do the same for game enemies too, normal sized ones at least. 
            //Will need to experiment with giant boss enemies to see what feels right
            if(hit.collider.gameObject.name == "Player(Clone)")
            {
                return;
            }

            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        if(Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, 0.2f);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}
