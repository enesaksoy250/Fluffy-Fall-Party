using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    public float rotationSpeed; 
    public string rotationAxis; 

    void Update()
    {
    
        Vector3 axis = GetRotationAxis();
     
        transform.Rotate(axis, rotationSpeed * Time.deltaTime);
    }

    private Vector3 GetRotationAxis()
    {
        
        switch (rotationAxis.ToLower())
        {
            case "y":
                return Vector3.up; 
            case "z":
                return Vector3.forward; 
            default:
                return Vector3.up; 
        }
    }
}
