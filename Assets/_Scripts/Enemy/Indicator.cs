using System.Collections;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    private Transform target;
    private Camera cam;

    private void Start() {
        cam = Camera.main;
    }
    private void Update() {
        if (target == null) return;

        if (TargetIsOffScreen()) {
            CalculateIndicatorPosition();
            gameObject.SetActive(true);
        } else {
            gameObject.SetActive(false);
        }
    }

    public void SetTarget(Transform chargerTransform) {
        target = chargerTransform;
    }

    private void CalculateIndicatorPosition() {
        Vector3 screenPoint = cam.WorldToViewportPoint(target.position);

        // Calculate an "edge point" that goes from the middle of the screen to out of the screen edges
        Vector3 edgePoint = new Vector3(screenPoint.x - 0.5f, screenPoint.y - 0.5f, 0) * 2;

        // We'll find the intersection of the edgePoint direction and the screen bounds.
        float absX = Mathf.Abs(edgePoint.x);
        float absY = Mathf.Abs(edgePoint.y);

        if (absX > absY) {
            edgePoint = (edgePoint.x > 0 ? 1 : -1) * edgePoint / absX;
        } else {
            edgePoint = (edgePoint.y > 0 ? 1 : -1) * edgePoint / absY;
        }

        // We now have an edge point. Convert it to screen position and place the indicator there.
        Vector3 screenPos = new Vector3(edgePoint.x, edgePoint.y, 0);
        Vector3 worldPosForIndicator = Camera.main.ViewportToWorldPoint(screenPos);
        worldPosForIndicator.z = 0; // Set to some suitable value. For example, indicators might be on the z=0 plane

        transform.position = worldPosForIndicator;
    }


    private bool TargetIsOffScreen() {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(target.position);
        return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
    }
}