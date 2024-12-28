using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float moveSpeed; 
    public float minY; 
    public float maxY; 
    public float minX; 
    public float maxX;
    public bool moveHorizontally; 
    public bool startGoingUpOrRight;

    private bool movingPositiveDirection; 

    void Start()
    {
        
        movingPositiveDirection = startGoingUpOrRight;
    }

    void Update()
    {
        float step = moveSpeed * Time.deltaTime; 

        if (moveHorizontally)
        {
           
            if (movingPositiveDirection)
            {
                transform.localPosition += new Vector3(step, 0, 0);
                if (transform.localPosition.x >= maxX)
                {
                    transform.localPosition = new Vector3(maxX, transform.localPosition.y, transform.localPosition.z);
                    movingPositiveDirection = false; 
                }
            }
            else
            {
                transform.localPosition -= new Vector3(step, 0, 0);
                if (transform.localPosition.x <= minX)
                {
                    transform.localPosition = new Vector3(minX, transform.localPosition.y, transform.localPosition.z);
                    movingPositiveDirection = true;
                }
            }
        }
        else
        {
            
            if (movingPositiveDirection)
            {
                transform.localPosition += new Vector3(0, step, 0);
                if (transform.localPosition.y >= maxY)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, maxY, transform.localPosition.z);
                    movingPositiveDirection = false; 
                }
            }
            else
            {
                transform.localPosition -= new Vector3(0, step, 0);
                if (transform.localPosition.y <= minY)
                {
                    transform.localPosition = new Vector3(transform.localPosition.x, minY, transform.localPosition.z);
                    movingPositiveDirection = true; 
                }
            }
        }
    }
}
