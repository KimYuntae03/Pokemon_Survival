using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{   
    public static GameManager instance;
    public PoolManager pool;
    public Player_Controller player;

    public float gameTime;
    public float maxGameTime = 7 * 60f;

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

    void Awake()
    {
        // 싱글톤 초기화
        instance = this;
    }

    void Start()
    {
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
        gameTime += Time.deltaTime;

        if (gameTime > maxGameTime) {
            gameTime = maxGameTime;
        }
        UpdateTimerUI(); //타이머 UI업데이트
    }

    public void GetExp()
    {
        exp += Mathf.RoundToInt(1 * expMultiplier);
        //필요한 경험치량 증가
        int targetExp = Mathf.RoundToInt(Mathf.Pow(level, 1.4f)) + 9;

        if (exp >= targetExp) {
            level++;
            exp = 0;
            UpdateLevelUI();
            
            if (level == 2) {
                player.Evolve(1); // 파이숭이 진화
            } else if (level == 3) {
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
