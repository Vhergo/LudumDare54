using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSelectAnimation : MonoBehaviour
{
    public static CursorSelectAnimation Instance { get; private set; }

    [SerializeField] private float shrinkSpeed = 0.5f;
    private Vector2 cursorPosition;
    private bool isShrinking;
    private float originalSizeX;
    private RectTransform rectTransform;

    private void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalSizeX = rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        isShrinking = Input.GetMouseButton(0) ? true : false;
        ScaleSafeZone();
    }

    private void ScaleSafeZone()
    {
        if (isShrinking) {
            if (rectTransform.sizeDelta.x > 0f)
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - shrinkSpeed * 100f * Time.deltaTime, rectTransform.sizeDelta.y - shrinkSpeed * 100f * Time.deltaTime);

        } else {
            if (rectTransform.sizeDelta.x < originalSizeX)
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x + shrinkSpeed * 200f * Time.deltaTime, rectTransform.sizeDelta.y + shrinkSpeed * 200f * Time.deltaTime);
        }
    }

    private void FollowMousePosition()
    {
        cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = cursorPosition;
    }
}
