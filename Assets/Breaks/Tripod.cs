using UnityEngine;

public class Tripod : MonoBehaviour
{
    float targetRotation = 0;
    float currentRotation = 0;
    float velocityRotation = 0;
    void Update()
    {
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
    }
}
