using UnityEngine;

public class DragToRotate : MonoBehaviour
{
     [SerializeField] private float rotationSpeed; // Döndürme hýzý

    void Update()
    {
        if (Input.touchCount > 0) // Dokunma algýlandý
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = touch.position;

                if (touch.phase == TouchPhase.Moved) // Dokunma hareket ediyor
                {
                    float deltaX = touch.deltaPosition.x;

                    // Nesneyi döndür
                    transform.Rotate(Vector3.up, -deltaX * rotationSpeed * Time.deltaTime);
                }       

        }
    }
}
