using UnityEngine;
using UnityEngine.InputSystem; 

public class TaylaLeaning : MonoBehaviour
{

    public Transform leanTarget; 
    public CharacterController characterController;
    public PlayerInput playerInput; 

    public float movementLeanIntensity; 
    public float cameraLeanIntensity;    
    public float maxLeanAngle;          
    public float leanSmooth;            
    public float maxSpeed;
    public float weightSmooth; 
    private InputAction moveAction;
    private InputAction lookAction;
    private float currentLeanZ;
    private float currentWeight; 

    void Awake()
    {
        if (playerInput != null)
        {
            moveAction = playerInput.actions.FindAction("Move");
            lookAction = playerInput.actions.FindAction("Look");
        }

        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }
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
        if (moveAction != null)
        {
            float horizontalInput = moveAction.ReadValue<Vector2>().x;
         
            targetLeanZ += horizontalInput * movementLeanIntensity*-1;
        }
        if (lookAction != null)
        {
            float cameraXInput = lookAction.ReadValue<Vector2>().x;
          
            targetLeanZ += cameraXInput * cameraLeanIntensity*-1;
        }
        targetLeanZ = Mathf.Clamp(targetLeanZ, -maxLeanAngle, maxLeanAngle);
        currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * weightSmooth);

        targetLeanZ *= currentWeight;
        currentLeanZ = Mathf.Lerp(currentLeanZ, targetLeanZ, Time.deltaTime * leanSmooth);
        leanTarget.localRotation = Quaternion.Euler(0f, 0f, currentLeanZ);
    }
}