using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement; //재시작 기능 사용

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

    public AudioSource titleBgm;//타이틀 BGM연결 
    public AudioSource levelUpSfx;//레벨업 시 효과음 
    public AudioSource expGetSfx;//보석획득 효과음
    public GameObject uiGameOver;

    void Awake()
    {
        // 싱글톤 초기화
        instance = this;
        Time.timeScale = 0f;
    }

    public void GameStart()
    {   
        if (titleBgm != null) { //게임 시작 누르면 타이틀BGM종료
            titleBgm.Stop();
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
        float remainTime = gameTime;
        int min = Mathf.FloorToInt(remainTime / 60);
        int sec = Mathf.FloorToInt(remainTime % 60);
        timerText.text = string.Format("{0:D2}:{1:D2}", min, sec);
    }

    void UpdateExpUI()
    {
        int currentTargetExp = Mathf.RoundToInt(Mathf.Pow(level, 1.4f)) + 9;
        expSlider.value = (float)exp / currentTargetExp; //현재 Exp / 목표 Exp를 경험치 바에 반영
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
}
