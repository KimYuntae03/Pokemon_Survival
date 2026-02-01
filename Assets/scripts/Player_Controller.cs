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
    public Transform visualTransform;//플레이어 이미지 저장용 변수

    public RuntimeAnimatorController[] evolutionAnimators;
    //ㄴ>애니메이션 저장용 배열. 불꽃숭이0,파이숭이1,초염몽2
    public RectTransform hpBarRect; //체력바 위치조정
    public Transform shadowTransform;//그림자 위치조정

    void Awake()
    {   
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(); //색 변경 컴포넌트 가져오기
        CurHp = maxHp; //체력 초기화
    }

    void Start()
    {   
        if (hpSlider != null) {
            hpSlider.maxValue = maxHp;
            hpSlider.value = CurHp;
        }

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
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

        if (CurHp > 0 && CurHp / maxHp <= 0.2f) {
            GameManager.instance.ChangeBgm(true);
        }

        StopCoroutine("OnDamage");
        StartCoroutine("OnDamage");

        if (CurHp <= 0) {
            Die();
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
        if (GameManager.instance.PlayerDieSfx != null) {
            GameManager.instance.PlayerDieSfx.PlayOneShot(GameManager.instance.PlayerDieSfx.clip);
        }
        rb.simulated = false;
        ResetInput();
        if (shadowTransform != null) shadowTransform.gameObject.SetActive(false);
        if (hpSlider != null) hpSlider.gameObject.SetActive(false);
        StartCoroutine(DieRoutine());
    }



    public void ResetInput()
    {
        inputVec = Vector2.zero;
        lastHorizontal = 0;
        lastVertical = 0;
        
        // 애니메이션도 정지 상태로 강제 전환
        if (anim != null) {
            anim.SetFloat("Speed", 0);
        }
    }

    public void ApplyMaxHpBoost(float amount) //맥스업 선택시 hp증가함수
    {
        maxHp += amount; 
        CurHp += amount; 
        
        // UI 업데이트
        if (hpSlider != null) {
            hpSlider.maxValue = maxHp;
            hpSlider.value = CurHp;
        }
    }

    public void Evolve(int stage) //플레이어 진화
    {
        if (stage < evolutionAnimators.Length && evolutionAnimators[stage] != null)
        {
            anim.runtimeAnimatorController = evolutionAnimators[stage];
            Debug.Log($"<color=cyan>진화 성공!</color> 단계: {stage}");
            
            Vector3 hpBarPos = new Vector3(0, -25f, 0);
            Vector3 shadowPos = new Vector3(0, -0.6f, 0);
            Vector3 shadowScale = new Vector3(1f, 1f, 1f);

            if(stage == 1)
            {
                shadowPos = new Vector3(0, -0.7f, 0);
            }
            else if (stage == 2) 
            {
                hpBarPos = new Vector3(0, -33f, 0);
                shadowPos = new Vector3(0, -0.95f, 0);
                shadowScale = new Vector3(1.3f, 1.2f, 1f);
            }
            hpBarRect.localPosition = hpBarPos;
            shadowTransform.localPosition = shadowPos;
            shadowTransform.localScale = shadowScale;
        }
    }

    public void Heal(float amount)
    {
        CurHp += amount;
        CurHp = Mathf.Min(CurHp, maxHp);
        
        UpdateHpBar();

        if (CurHp / maxHp > 0.2f) {
            GameManager.instance.ChangeBgm(false);
        }
    }

    public void OnHeal()
    {
        StartCoroutine(HealEffectRoutine());
    }

    IEnumerator HealEffectRoutine()
    {
        Color originalColor = Color.white;
        Color healColor = new Color(0.2f, 1f, 0.2f, 1f);
        
        yield return new WaitForSeconds(0.9f);

        spriteRenderer.color = healColor;

        yield return new WaitForSeconds(1.0f);

        // 다시 원래색으로 복구
        spriteRenderer.color = originalColor;
    }

    IEnumerator DieRoutine()
    {
        float duration = 0.8f; // 연출 시간
        float timer = 0f;
        Vector3 startLocalPos = visualTransform.localPosition;

        if (shadowTransform != null) shadowTransform.gameObject.SetActive(false);//그림자,체력바 끄기
        if (hpSlider != null) hpSlider.gameObject.SetActive(false);

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / duration;
            
            // 아래로 1.5만큼 이동 및 투명도 조절
            visualTransform.localPosition = startLocalPos + Vector3.down * (progress * 2.5f);

            yield return null;
        }
        GameManager.instance.GameOver();
    }
}
