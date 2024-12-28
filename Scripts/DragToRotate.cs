using UnityEngine;

public class DragToRotate : MonoBehaviour
{
     [SerializeField] private float rotationSpeed; // D�nd�rme h�z�

    void Update()
    {
        if (Input.touchCount > 0) // Dokunma alg�land�
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = touch.position;

                if (touch.phase == TouchPhase.Moved) // Dokunma hareket ediyor
                {
                    float deltaX = touch.deltaPosition.x;

                    // Nesneyi d�nd�r
                    transform.Rotate(Vector3.up, -deltaX * rotationSpeed * Time.deltaTime);
                }       

        }
    }
}
