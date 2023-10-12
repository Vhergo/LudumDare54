using System.Collections;
using UnityEngine;

public class RotateToMouse : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 10f;
    private Vector2 mousePos;

    private void Update() {
        HandleRotation();
    }

    private void HandleRotation() {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

    }
}