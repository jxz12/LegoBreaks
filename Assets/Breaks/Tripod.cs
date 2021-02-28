using System.Collections.Generic;

using UnityEngine;

public class Tripod : MonoBehaviour
{
    float targetRotation;
    float currentRotation;
    float velocityRotation;
    Vector3 offsetPosition;
    Vector3 targetPosition;
    Vector3 currentPosition;
    Vector3 velocityPosition;
    float targetField;
    float currentField;
    float velocityField;
    void Start() {
        targetRotation = currentRotation = transform.rotation.eulerAngles.y;
        targetPosition = currentPosition = transform.position;
        targetField = currentField = Camera.main.fieldOfView;
    }
    void LateUpdate()
    {
        // Tween rotation
        if (Input.GetMouseButtonDown(1)) {
            targetRotation += 90;
        }
        currentRotation = Mathf.SmoothDamp(
            currentRotation,
            targetRotation,
            ref velocityRotation,
            .2f
        );
        transform.rotation = Quaternion.Euler(0, currentRotation, 0);

        // position and FOV to view bricks (credit: Brackeys on youtube)
        if (followedBricks != null) {
            var bounds = new Bounds();
            foreach (Brick b in followedBricks) {
                bounds.Encapsulate(b.transform.position);
            }
            targetPosition = bounds.center + offsetPosition;
            targetField = Mathf.Lerp(25, 75, Mathf.Max(bounds.size.x, bounds.size.z) / 40);
        } else {
            targetPosition = Vector3.zero;
            targetField = 60;
        }
        currentPosition = Vector3.SmoothDamp(
            currentPosition,
            targetPosition,
            ref velocityPosition,
            .5f
        );
        currentField = Mathf.SmoothDamp(
            currentField,
            targetField,
            ref velocityField,
            .5f
        );
        transform.position = currentPosition;
        Camera.main.fieldOfView = currentField;
    }
    IEnumerable<Brick> followedBricks;
    public void Follow(IEnumerable<Brick> bricks, Vector3 offset) {
        followedBricks = bricks;
        offsetPosition = offset;
    }
}
