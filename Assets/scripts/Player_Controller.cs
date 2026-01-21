using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    public float moveSpeed = 3.5f; //플레이어 속도
    private Rigidbody2D rb;
    public Vector2 inputVec;
    private Animator anim;
    private float lastHorizontal = 0;
    private float lastVertical = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) lastHorizontal = -1;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) lastHorizontal = 1;

        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow)) 
            lastHorizontal = (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) ? 1 : 0;
        if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)) 
            lastHorizontal = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) ? -1 : 0;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) lastVertical = 1;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) lastVertical = -1;

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow)) 
            lastVertical = (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) ? -1 : 0;
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow)) 
            lastVertical = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) ? 1 : 0;

        inputVec.x = lastHorizontal;
        inputVec.y = lastVertical;

        if (inputVec.sqrMagnitude > 0.01f) 
        {
            anim.SetFloat("InputX", inputVec.x);
            anim.SetFloat("InputY", inputVec.y);
        }
       float inputMagnitude = inputVec.magnitude;
        anim.SetFloat("Speed", inputMagnitude);
        inputVec = inputVec.normalized;
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + inputVec * moveSpeed * Time.fixedDeltaTime);
    }
    public void ApplySpeedBoost(float amount)
    {
        moveSpeed += amount;
    }
}
