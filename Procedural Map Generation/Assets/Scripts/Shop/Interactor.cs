using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;


public class Interactor : MonoBehaviour
{
    public float interactRange = 3f;
    public LayerMask interactableLayer = ~0;
    public TMP_Text promptText;
    [Tooltip("Liga logs no Console para descobrir por que o E não interage.")]
    public bool debugLog = false;

    private IInteractable currentInteractable;
    private int lastInteractFrame = -1;

    void Start()
    {
        InputInfo.OnInteractEvent += OnInteractPressed;
    }

    void OnDestroy()
    {
        InputInfo.OnInteractEvent -= OnInteractPressed;
    }

    void Update()
    {
        if (GameUIState.IsUIOpen)
        {
            if (promptText != null) promptText.gameObject.SetActive(false);
            return;
        }

        DetectInteractable();

        Keyboard kb = Keyboard.current;
        if (kb != null && kb.eKey.wasPressedThisFrame) OnInteractPressed();
    }

    void OnInteractPressed()
    {
        if (GameUIState.IsUIOpen) return;
        if (lastInteractFrame == Time.frameCount) return;

        if (currentInteractable == null)
        {
            if (debugLog) Debug.Log("[Interactor] E pressionado, mas nenhum interagível dentro do alcance.", this);
            return;
        }

        lastInteractFrame = Time.frameCount;
        if (debugLog) Debug.Log($"[Interactor] Interagindo com: {currentInteractable}", this);
        currentInteractable.Interact(gameObject);
    }

    void DetectInteractable()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
        IInteractable closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable == null) continue;

            float dist = Vector3.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        currentInteractable = closest;

        if (promptText != null)
        {
            if (currentInteractable != null)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = currentInteractable.GetInteractionPrompt();
            }
            else
            {
                promptText.gameObject.SetActive(false);
            }
        }
    }
}