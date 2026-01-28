using UnityEngine;
using UnityEngine.UI; //체력바 UI를 위해 추가
using System.Collections;//플레이어 피격시 데미지 표현(코루틴 사용)

public class Player_Controller : MonoBehaviour
{
    public float moveSpeed = 3.5f; //플레이어 속도
    private Rigidbody2D rb;
    public Vector2 inputVec;
    private Animator anim;
    private float lastHorizontal = 0;
    private float lastVertical = 0;
    public ItemData defaultWeaponData;
    public float maxHp = 100f;
    public float CurHp;
    public Slider hpSlider; //인스펙터에서 연결할 슬라이더
    SpriteRenderer spriteRenderer; //캐릭터 색 바꿀 컴포넌트
    public Vector2 lastVec; //마지막 입력 방향 저장용 변수

    void Awake()
    {   
        spriteRenderer = GetComponent<SpriteRenderer>(); //색 변경 컴포넌트 가져오기
        CurHp = maxHp; //체력 초기화
    }

    void Start()
    {   
        if (hpSlider != null) {
            hpSlider.maxValue = maxHp;
            hpSlider.value = CurHp;
        }

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // 기본무기 생성
        if (defaultWeaponData != null) {
            GameObject obj = Instantiate(defaultWeaponData.itemPrefab, transform);
            obj.transform.localPosition = Vector3.zero;

            Weapon weapon = obj.GetComponent<Weapon>();
            weapon.Init(defaultWeaponData);
            weapon.level++; 
        }
    }
    

    void Update()
    {   
        if (Time.timeScale == 0) 
        {
            inputVec = Vector2.zero; // 이동 벡터 초기화
            anim.SetFloat("Speed", 0); // 애니메이션도 정지 상태로 고정
            return;
        }
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

        if (inputVec.magnitude != 0) {
            lastVec = inputVec.normalized;
        }
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position + inputVec * moveSpeed * Time.fixedDeltaTime);
    }
    public void ApplySpeedBoost(float amount)
    {
        moveSpeed += amount;
    }

    void UpdateHpBar()
    {
        if (hpSlider != null) {
            hpSlider.value = CurHp; // 슬라이더 값을 현재 체력으로 업데이트
        }
    }
    public void TakeDamage(float damage)
    {
        CurHp -= damage;
        CurHp = Mathf.Max(CurHp, 0);
        UpdateHpBar();

        StopCoroutine("OnDamage");
        StartCoroutine("OnDamage");

        if (CurHp <= 0) {
            // 사망 로직 추가 예정
        }
    }

    IEnumerator OnDamage() //색상을 변경했다가 되돌리는 코루틴
    {
        // 캐릭터 색상을 빨간색으로 변경
        spriteRenderer.color = Color.red;

        // 0.3초 동안 대기
        yield return new WaitForSeconds(0.1f);

        // 다시 원래 색상(하얀색)으로 복구
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        Debug.Log("플레이어 사망!");
        //추후에 게임오버 UI나 재시작 화면 구현
        Time.timeScale = 0f; 
    }
}
