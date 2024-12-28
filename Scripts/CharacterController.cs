using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
public class CharacterController : MonoBehaviour
{
    public static CharacterController instance;
  
    private Animator animator; 
    private Rigidbody rb;  
    private Camera cam;
    private PhotonView pw;
    private groundCollider groundCollider;
    private PlayerMovement playerMovement;
    private AudioSource jumpSound;

    public float moveSpeed;     
    public float jumpForce;   
     

   [HideInInspector] public bool isGameStart, countdownEnd,countdownStart=false;

    private Vector3[] startPositions =
    {
            new Vector3(-3.23f, 1.77f, 6.6f),
            new Vector3(-0.64f, 1.77f, 6.6f),
            new Vector3(-1.66f, 1.77f, 6.6f),
            new Vector3(1.34f, 1.77f, 6.6f),
          
        };

    private List<Vector3> checkpointPositions = new List<Vector3>();

    private int time = 6;

    public bool isRunning,isJumping,waitingArea = false;

    private float jumpClipLength,dieClipLength;
    public bool isReadyForJump=true;
    private bool isReadyForRun=true;

    private TextMeshProUGUI timerText, waitTitleText;
    private TextMeshProUGUI usernameText;

    private double startTime;
    public float slideForce;

    public bool removeAdControl { get; private set; }

    private void Awake()
    {
        waitingArea = false;        
        instance = this;
        FindComponent();
      
    }

    void Start()
    {

        Invoke(nameof(SetStartPosition),1);
    

        if (pw.IsMine)
        {
           
            if (PhotonNetwork.IsMasterClient)
            {
                OnlineGameManager.instance.borders.SetActive(true);
                waitingArea = true;
            }
            SetPlayerMovementScript();
            removeAdControl = UserFirebaseInformation.instance.removeAd;
            string username = UserFirebaseInformation.instance.UserName;
            pw.Owner.NickName = username;
            pw.RPC("SetUsername", RpcTarget.AllBuffered, pw.ViewID, username);

        }

        else
        {
            GetComponent<CharacterController>().enabled = false;
        }


        GetAnimationDuration();
        slideForce = 250;

    }

    
   

    [PunRPC]
    private void SetUsername(int viewID, string username)
    {
        
        PhotonView targetView = PhotonView.Find(viewID);

        if (targetView != null && targetView.gameObject != null)
        {
           
            TextMeshProUGUI targetUsernameText = targetView.GetComponentInChildren<TextMeshProUGUI>();
            if (targetUsernameText != null)
            {
                targetUsernameText.text = username;
            }
        }
    }


    public void SetStartPosition()
    {
        if (pw.IsMine)
        {
        
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            transform.position = startPositions[actorNumber - 1];
         

        }
                       
    }

    

    private void SetPlayerMovementScript()
    {

        playerMovement = FindObjectOfType<PlayerMovement>();
        playerMovement.SetFindControllerScript(this);

    }

    void Update()
    {


        Control();
        
     
        if (pw.IsMine && (countdownEnd || waitingArea) && OnlineGameManager.instance.isPlay)
        {
            Move();
        }
  
    
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2 && !countdownStart && PhotonNetwork.IsMasterClient)
        {

            waitingArea = false;
            OnlineGameManager.instance.borders.SetActive(false);
            animator.SetBool("IsRunning", false);
            SetStartPosition();
            StartCountdown();

        }

    }

    private void Move()
    {

        if (isRunning && isReadyForRun)
        {

            Vector3 screenDirection = FollowButton.instance.position.normalized;

            Vector3 cameraForward = cam.transform.forward;
            Vector3 cameraRight = cam.transform.right;

            Vector3 moveDirection = (cameraForward * screenDirection.y + cameraRight * screenDirection.x).normalized;

            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            animator.SetBool("IsRunning", true);


        }

        else
        {

            animator.SetBool("IsRunning", false);

        }



    }

    public void Jump()
    {


        if (groundCollider.isGrounded && isReadyForJump && pw.IsMine && (countdownEnd || waitingArea)  && OnlineGameManager.instance.isPlay)
        {

            StartCoroutine(SetReadyForJump(jumpClipLength));
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("Jump");
            jumpSound.Play();

        }


    }

    void StartCountdown()
    {
        if (!countdownStart)
        {
            countdownStart = true;
            startTime = PhotonNetwork.Time + 6;
            pw.RPC("SetStartTime", RpcTarget.AllBuffered, startTime);
        }
    }

    [PunRPC]
    void SetStartTime(double time)
    {
        startTime = time;
        StartCoroutine(SynchronizedCountdown());
    }

    IEnumerator SynchronizedCountdown()
    {
        while (true)
        {
            double timeLeft = startTime - PhotonNetwork.Time;

            if (timeLeft > 0)
            {
                UIElements.instance.timerText.text = "THE GAME BEGINS";
            }
            else
            {
                break;
            }

            yield return null;
        }

     
        for (int i = 3; i > 0; i--)
        {
            UIElements.instance.timerText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

      
        UIElements.instance.timerText.text = "START";
        yield return new WaitForSeconds(1f);
        UIElements.instance.timerText.text = "";
        countdownEnd = true;
   
        CharacterController [] characterController = FindObjectsOfType<CharacterController>();
     
        foreach(CharacterController character in characterController)
        {
            character.countdownEnd = true;
        }     

        if(!removeAdControl) { AdManager.instance.LoadInterstitialAd(); print("reklam yüklendi"); }
          

    }


    [PunRPC]
    public void Control()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2 && !isGameStart) // burasý 1 deðil 5 ti
        {
            isGameStart = true;
        }
    }

    public IEnumerator SetReadyForJump(float duration)
    {
        isReadyForJump = false;
        yield return new WaitForSeconds(duration);
        isReadyForJump = true;

    }
   

    public void AddCheckpoint(Vector3 checkpointPosition)
    {
        if (!checkpointPositions.Contains(checkpointPosition))
        {
            checkpointPositions.Add(checkpointPosition);
            Debug.Log("Checkpoint added: " + checkpointPosition);
        }
    }

    public void RespawnAtLastCheckpoint()
    {
        if (checkpointPositions.Count > 0)
        {
            Vector3 lastCheckpoint = checkpointPositions[checkpointPositions.Count - 1];
            transform.position = lastCheckpoint;
            Debug.Log("Respawned at last checkpoint: " + lastCheckpoint);
        }
        else
        {
            transform.position = startPositions[PhotonNetwork.LocalPlayer.ActorNumber - 1];
            Debug.Log("No checkpoints reached yet, respawning at start position.");
        }
    }

    public void Die()
    {
       
        isReadyForRun = false;
        isReadyForJump = false;
        animator.SetTrigger("Die");
        StartCoroutine(SetPositionAfterDie());

    }


    private void OnTriggerEnter(Collider other)
    {
       

        if (other.gameObject.CompareTag("OutOfMap"))
        {

            RespawnAtLastCheckpoint();

        }
     
    }

   

    IEnumerator SetPositionAfterDie()
    {

        yield return new WaitForSeconds(dieClipLength * 2);
        animator.SetTrigger("Idle");
        RespawnAtLastCheckpoint();
        yield return new WaitForSeconds(dieClipLength / 2);
        isReadyForJump = true;
        isReadyForRun = true;   

    }


    private void GetAnimationDuration()
    {

        RuntimeAnimatorController controller = animator.runtimeAnimatorController;

        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == "jumpNew")
            {
                jumpClipLength = clip.length;
            }

            if(clip.name == "Die01")
            {
                dieClipLength = clip.length;
            }
        }

    }

    public void ResetCheckpoints()
    {
        checkpointPositions.Clear(); // Checkpoint dizisini temizler
    }

    public void LoadAd()
    {        
       AdManager.instance.LoadInterstitialAd();         
    }

    public void UpdateLeaderboard(string userName,int number)
    {

        DataBaseManager.instance.UpdateLeaderboard(userName,number);

    }

 
    private void FindComponent()
    {

        usernameText = GetComponentInChildren<TextMeshProUGUI>();
        cam = GetComponentInChildren<Camera>();
        groundCollider = GetComponentInChildren<groundCollider>();
        jumpSound = GetComponentInChildren<AudioSource>();
        pw = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();


    }


}
