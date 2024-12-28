using UnityEngine;

public class ArrowSlide : MonoBehaviour
{
    public string direction; 
    public float slideSpeed; 

    private void OnTriggerStay(Collider other)
    {
       
        if (other.CompareTag("Player")) 
        {
            Vector3 slideDirection = GetDirectionVector();
            other.transform.Translate(slideDirection * slideSpeed * Time.deltaTime, Space.World);
        }
    }

    private Vector3 GetDirectionVector()
    {
        
        switch (direction.ToLower())
        {
            case "z":
                return Vector3.forward; 
            case "x":
                return Vector3.right;   
            case "-x":
                return Vector3.left;   
            default:
                Debug.LogWarning("Ge�ersiz y�n! Varsay�lan olarak Z ekseni kullan�lacak.");
                return Vector3.forward; 
        }
    }
}
