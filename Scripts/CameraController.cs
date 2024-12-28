using Photon.Pun;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 0.2f; // Kamera döndürme hýzý
    private Vector2 lastTouchPosition;
    private bool isSwiping;

    private PhotonView photonView;

    private Transform playerTransform;

    private void Awake()
    {

        photonView = GetComponent<PhotonView>();

        if (transform.parent != null)
        {
            playerTransform = transform.parent;
        }

    }

    private void Start()
    {

        gameObject.transform.localPosition = new Vector3(0, 4.39f, -4.64f);
        gameObject.transform.rotation = Quaternion.Euler(19.66f, 0, 0);

        if (!photonView.IsMine)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (photonView.IsMine && OnlineGameManager.instance.isPlay)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);


                if (touch.position.x >= Screen.width / 2)
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        lastTouchPosition = touch.position;
                        isSwiping = true;
                    }
                    else if (touch.phase == TouchPhase.Moved && isSwiping)
                    {
                        Vector2 currentTouchPosition = touch.position;
                        Vector2 swipeDelta = currentTouchPosition - lastTouchPosition;

                        float rotationAmount = swipeDelta.x * rotationSpeed;

                        playerTransform.Rotate(0, rotationAmount, 0);

                        lastTouchPosition = currentTouchPosition;

                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        isSwiping = false;
                    }
                }
            }
        }
    }
}
