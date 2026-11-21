using UnityEngine;

public class TaylaLeaning : MonoBehaviour
{
    public Transform leanTarget; 
    public CharacterController characterController;

    public float movementLeanIntensity; 
    public float cameraLeanIntensity;    
    public float maxLeanAngle;          
    public float leanSmooth;            
    public float maxSpeed;
    public float weightSmooth; 

    private Vector2 moveInput;
    private Vector2 lookInput;
    private float currentLeanZ;
    private float currentWeight; 

    void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
    }
    void OnEnable()
    {
        InputInfo.OnMoveEvent += HandleMove;
        InputInfo.OnLockEvent += HandleLook;
    }

    void OnDisable()
    {
        InputInfo.OnMoveEvent -= HandleMove;
        InputInfo.OnLockEvent -= HandleLook;
    }

    private void HandleMove(Vector2 input)
    {
        moveInput = input;
    }

    private void HandleLook(Vector2 input)
    {
        lookInput = input;
    }

    void Update()
    {
        float targetLeanZ = 0f;
        float targetWeight = 0f; 

        if (characterController != null)
        {
            Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
            float currentSpeed = horizontalVelocity.magnitude;
            targetWeight = Mathf.Clamp01(currentSpeed / maxSpeed);
        }
        targetLeanZ += moveInput.x * movementLeanIntensity * -1f;
        targetLeanZ += lookInput.x * cameraLeanIntensity * -1f;

        targetLeanZ = Mathf.Clamp(targetLeanZ, -maxLeanAngle, maxLeanAngle);
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * weightSmooth);

        targetLeanZ *= currentWeight;
        currentLeanZ = Mathf.Lerp(currentLeanZ, targetLeanZ, Time.deltaTime * leanSmooth);
        
        if (leanTarget != null)
        {
            leanTarget.localRotation = Quaternion.Euler(0f, 0f, currentLeanZ);
        }
    }
}