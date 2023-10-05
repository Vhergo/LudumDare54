using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    [SerializeField] private float indicatorEdgeOffset = 100f;
    [SerializeField] private Vector2 screenPosition;
    private Vector2 indicatorDirection;
    private Image indicatorImage;
    private RectTransform indicatorRectTransform;
    private Transform target;
    private Camera cam;

    private void Start() {
        indicatorImage = GetComponent<Image>();
        indicatorRectTransform = GetComponent<RectTransform>();
        cam = Camera.main;
    }
    private void Update() {
        if (target == null) {
            HideIndicator();
            return;
        }

        if (TargetIsOffScreen()) {
            ShowIndicator();
            HandleIndicatorOffScreen();
        }else {
            HideIndicator();
        }

    }

    private void HandleIndicatorOffScreen() {
        RotateIndicator();
        ClampToScreenEdge();
        indicatorRectTransform.position = screenPosition;
    }

    private void ClampToScreenEdge() {
        Vector2 screenPos = cam.WorldToScreenPoint(target.position);
        screenPosition.x = Mathf.Clamp(screenPos.x, indicatorEdgeOffset, Screen.width - indicatorEdgeOffset);
        screenPosition.y = Mathf.Clamp(screenPos.y, indicatorEdgeOffset, Screen.height - indicatorEdgeOffset);
    }

    private void RotateIndicator() {
        indicatorDirection = (target.position - transform.position).normalized;
        float angle = Mathf.Atan2(indicatorDirection.y, indicatorDirection.x) * Mathf.Rad2Deg;
        indicatorRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private bool TargetIsOffScreen() {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.position);
        return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
    }

    private void HideIndicator() {
        indicatorImage.enabled = false;
    }

    private void ShowIndicator() {
        indicatorImage.enabled = true;
    }

    public void SetTarget(Transform chargerTransform) {
        Debug.Log("Target Position Set: " + chargerTransform.position);
        target = chargerTransform;
    }
}