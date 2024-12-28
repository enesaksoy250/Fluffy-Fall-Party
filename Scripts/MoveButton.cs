using UnityEngine;

public class MoveButton : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector2 parentStartPosition;

    private FollowButton followButton;
    private Transform highestParent;

    private CircleCollider2D circleCollider;


    private Vector2 startTouchPosition;
    private bool isSwipeDetected = false;

    public static MoveButton instance;

    private void Awake()
    {
        instance = this;
        followButton = FindObjectOfType<FollowButton>();
        circleCollider = GetComponentInParent<CircleCollider2D>();
    }

    private void Start()
    {
        highestParent = GetHighestParent(transform);
        startPosition = transform.position;
        parentStartPosition = highestParent.position;
    }

    private void Update()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

         
            if (touch.position.x < Screen.width / 2 && OnlineGameManager.instance.isPlay)
            {
             

                if (touch.phase == TouchPhase.Began)
                {
                    highestParent.position = touch.position;

                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 newPosition = touch.position;
                    Vector3 constrainedPosition = circleCollider.ClosestPoint(newPosition);
                    transform.position = constrainedPosition;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
             
                    highestParent.position = parentStartPosition;
                    followButton.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                }

            
            }

            if (touch.phase == TouchPhase.Began)
            {

                startTouchPosition = touch.position;
                isSwipeDetected = false;
            }
            else if (touch.phase == TouchPhase.Moved)
            {

                if (!isSwipeDetected && startTouchPosition.x < Screen.width / 2 && touch.position.x > Screen.width / 2)
                {
                    isSwipeDetected = true;
                    highestParent.position = parentStartPosition;
                    followButton.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    Debug.Log("Soldan saða kaydýrma algýlandý!");
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {

                startTouchPosition = Vector2.zero;
                isSwipeDetected = false;
            }

        }

     
    }

    public void ResetButtonPosition()
    {

        highestParent.position = parentStartPosition;
        followButton.gameObject.GetComponent<RectTransform>().localPosition = Vector3.zero;

    }

    Transform GetHighestParent(Transform child)
    {
        Transform currentParent = child.parent;

        while (currentParent != null)
        {
            if (currentParent.parent.name == "BottomPanel")
            {
                return currentParent;
            }
            currentParent = currentParent.parent;
        }

        return null;
    }
}
