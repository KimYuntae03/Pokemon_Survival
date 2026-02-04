using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //재시작 기능 사용
using System.Collections;

public class GameManager : MonoBehaviour
{   
    public static GameManager instance;
    public PoolManager pool;
    public Player_Controller player;

    public float gameTime;
    public float maxGameTime = 7 * 60f;
    public bool isPlayerLive;//플레이어 생사확인
    public int level;
    public int kill;
    public float exp;
    public float expMultiplier = 1.0f; //경험치 추가배율
    public LevelUp uiLevelUp; //레벨업 시 연결할 UI변수
    public float damageBuff = 1.0f; //데미지 관리 기본 1배
    
    public Slider expSlider;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI killText;
    public TextMeshProUGUI levelText;

    public AudioSource levelUpSfx;//레벨업 시 효과음 
    public AudioSource expGetSfx;//보석획득 효과음
    public AudioSource berrySfx;//자뭉열매 효과음
    public AudioSource berrySfx2;
    public AudioSource PlayerDieSfx;//플레이어 사망 효과음
    public AudioClip magnetSfxClip;//자석획득 효과음

    [Header("BGM SFX")]
    public AudioSource titleBgm;//타이틀 BGM연결 
    public AudioSource mainBgm;//인게임 BGM
    public AudioSource dangerBgm;//20퍼 이하 BGM
    public AudioSource bossBgm; //기라티나 BGM

    [Header("Weapon SFX")]
    public AudioSource scratchSfx;//할퀴기 효과음

    public GameObject uiGameOver;
    public GameObject uiGameClear;
    bool isDanger = false; //BGM체크 변수

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject); // 새로 만들어진 중복 객체를 파괴
            return;
        }
        // 싱글톤 초기화
        instance = this;
        Time.timeScale = 0f;
    }

    public void GameStart()
    {   
        if (titleBgm != null) { // 타이틀BGM종료
            titleBgm.Stop();
        }
        if (mainBgm != null) {//인게임 BGM재생
            mainBgm.Play();
        }

        Time.timeScale = 1f;
        isPlayerLive = true;
        level = 1;
        exp = 0;
        kill = 0;
        gameTime = 0;

        UpdateExpUI();
        UpdateKillUI();
        UpdateLevelUI();
    }

    public void GameOver()
    {
        isPlayerLive = false; // 플레이어 사망
        uiGameOver.SetActive(true); // 종료 UI 활성화
        if (mainBgm != null) {//인게임 BGM종료
            mainBgm.Stop();
        }
        if (dangerBgm != null) {//위험 BGM정지
            dangerBgm.Stop();
        }
        if (bossBgm != null) {//위험 BGM정지
            bossBgm.Stop();
        }
        Time.timeScale = 0f;
    }

    public void GameClear()
    {
        isPlayerLive = false; // 플레이어 상태 업데이트
        uiGameClear.SetActive(true); // 클리어 UI 활성화
        
        // 인게임 BGM 정지
        if (mainBgm != null) mainBgm.Stop();
        if (dangerBgm != null) dangerBgm.Stop();
        
        Time.timeScale = 0f; // 게임 일시 정지
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    void Start()
    {
        if (titleBgm != null) { //게임 실행시 BGM재생
            titleBgm.Play();
        }
        // 초기 UI 업데이트
        level = 1;
        exp = 0;
        kill = 0;
        UpdateExpUI();
        UpdateKillUI();
        UpdateLevelUI();
    }

    void Update()
    {   
        // 타이머 작동
        if(!isPlayerLive) return;
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime) {
            gameTime = maxGameTime;
        }
        UpdateTimerUI(); //타이머 UI업데이트
    }

    public void GetExp()
    {   
        if (expGetSfx != null) {
            expGetSfx.PlayOneShot(expGetSfx.clip); 
        }

        exp += Mathf.RoundToInt(1 * expMultiplier);
        //필요한 경험치량 증가
        int targetExp = Mathf.RoundToInt(Mathf.Pow(level, 1.4f)) + 9;

        if (exp >= targetExp) {
            level++;
            exp = 0;
            if (levelUpSfx != null) {
                levelUpSfx.Play(); 
            }
            UpdateLevelUI();
            
            if (level == 9) {
                player.Evolve(1); // 파이숭이 진화
            } else if (level == 18) {
                player.Evolve(2); // 초염몽 진화
            }

            if (uiLevelUp != null) {
                uiLevelUp.Show();
                }
            }
        UpdateExpUI();
    }

    public void GetKill()
    {
        kill++;
        UpdateKillUI();
    }

    void UpdateTimerUI()
    {
        int min = Mathf.FloorToInt(gameTime / 60);
        int sec = Mathf.FloorToInt(gameTime % 60);
        timerText.text = string.Format("{0:D2}:{1:D2}", min, sec);
    }

    void UpdateExpUI()
    {
        int targetExp = Mathf.RoundToInt(Mathf.Pow(level, 1.4f)) + 9;
        expSlider.value = exp / targetExp; //현재 Exp / 목표 Exp를 경험치 바에 반영
    }

    void UpdateKillUI()
    {
        killText.text = string.Format("{0:n0}", kill); // 세 자리마다 콤마
    }

    void UpdateLevelUI()
    {
        levelText.text = string.Format("Lv.{0:D2}", level);
    }

    public void ApplyDamageBuff(float amount) //데미지 증가 함수
    {
        damageBuff += amount; // 10% 증가 시 1.1f
    }

    public void ChangeBgm(bool danger)
    {
        if (isDanger == danger) return;

        isDanger = danger;

        if (danger) {
            mainBgm.Pause(); // 일반 BGM 일시정지
            dangerBgm.Play(); // 긴급 BGM 재생
        } else {
            dangerBgm.Stop(); // 긴급 BGM 정지
            mainBgm.UnPause(); // 일반 BGM 다시 재생
        }
    }

    public void PlayMagnetSfx()
    {
        if (magnetSfxClip != null)
        {
            // 기존에 있던 expGetSfx나 다른 소스를 빌려 써서 재생합니다.
            // PlayOneShot을 쓰면 기존 소리와 섞여서 잘 들립니다.
            expGetSfx.PlayOneShot(magnetSfxClip); 
        }
    }

    public IEnumerator PlayBerrySfxSequence()
    {
        if (berrySfx != null) {
            berrySfx.PlayOneShot(berrySfx.clip);
        }

        yield return new WaitForSeconds(0.6f); 

        if (berrySfx2 != null) {
            berrySfx2.PlayOneShot(berrySfx2.clip);
        }
    }
    public void PlayBossBgm()
    {
        if (mainBgm != null) mainBgm.Stop();
        if (dangerBgm != null) dangerBgm.Stop();
        
        if (bossBgm != null) {
            bossBgm.Play();
        }
    }
}
