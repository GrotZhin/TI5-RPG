using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UiButtonManager : MonoBehaviour
{
    [Header("Arrow")]
    [SerializeField] private RectTransform Pointer; 
    [SerializeField] private float arrowMoveDuration = 0.2f;
    [SerializeField] private float arrowOffsetX; 
    
    [Header("Buttons")]
    [SerializeField] private float SelectScale = 1.15f;  
    [SerializeField] private float UnScale = 0.9f;  

    private GameObject LastSelect;
    private List<Button> AllButtons = new List<Button>();
    private bool isActive;

    void Start()
    {
        if (Pointer != null) Pointer.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isActive) return;
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected != LastSelect)
        {
            HandleSelectionChanged(currentSelected);
            LastSelect = currentSelected;
        }
    }

    public void InitializeManager(GameObject rootCanvas, GameObject firstSelected)
    {
        isActive = true;
        
        if (Pointer != null) Pointer.gameObject.SetActive(true);

        AllButtons.Clear();
        AllButtons.AddRange(rootCanvas.GetComponentsInChildren<Button>(true));

        ResetAllButtonsToUnselected();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);

        LastSelect = firstSelected;
        HandleSelectionChanged(firstSelected);
    }

    public void DeactivateManager()
    {
        isActive = false;
        if (Pointer != null) Pointer.gameObject.SetActive(false);
        ResetAllButtonsToNormal();
        LastSelect = null;
    }

    private void HandleSelectionChanged(GameObject selected)
    {
        if (selected == null) return;

        Button targetButton = selected.GetComponent<Button>();
        
        if (targetButton != null && AllButtons.Contains(targetButton))
        {
            RectTransform buttonRect = selected.GetComponent<RectTransform>();
            
            MoveArrowTo(buttonRect);

            selected.transform.DOKill();
            selected.transform.DOScale(Vector3.one * SelectScale, 0.2f).SetUpdate(true).SetEase(Ease.OutQuad);

            foreach (Button btn in AllButtons)
            {
                if (btn != null && btn.gameObject != selected)
                {
                    btn.transform.DOKill();
                    btn.transform.DOScale(Vector3.one * UnScale, 0.2f).SetUpdate(true).SetEase(Ease.OutQuad);
                }
            }
        }
    }

    private void MoveArrowTo(RectTransform targetButton)
    {
        if (Pointer == null || targetButton == null) return;
        Pointer.DOKill();
        
        Vector3 localOffset = new Vector3(arrowOffsetX, 0, 0);
        Vector3 targetWorldPosition = targetButton.TransformPoint(localOffset);

        Pointer.DOMove(targetWorldPosition, arrowMoveDuration).SetUpdate(true).SetEase(Ease.OutCubic);

        Pointer.DORotateQuaternion(targetButton.rotation, arrowMoveDuration).SetUpdate(true).SetEase(Ease.OutCubic);
    }

    private void ResetAllButtonsToUnselected()
    {
        foreach (Button btn in AllButtons)
        {
            if (btn == null) continue;
            btn.transform.DOKill();
            btn.transform.localScale = Vector3.one * UnScale;
        }
    }

    private void ResetAllButtonsToNormal()
    {
        foreach (Button btn in AllButtons)
        {
            if (btn == null) continue;
            btn.transform.DOKill();
            btn.transform.localScale = Vector3.one;
        }
    }
}