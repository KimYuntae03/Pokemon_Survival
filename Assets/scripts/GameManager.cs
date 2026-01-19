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
    public int exp;
    public int[] nextExp = { 10, 30, 60, 100, 150, 210, 280, 360, 450, 600 }; 

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
        exp++;

        // 레벨업 체크 (현재 레벨이 최대 레벨 배열을 넘지 않게 처리)
        if (level < nextExp.Length && exp == nextExp[level]) {
            level++;
            exp = 0;
            UpdateLevelUI();
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
        // 다음 레벨 경험치가 있다면 슬라이더 갱신
        if (level < nextExp.Length) {
            expSlider.value = (float)exp / nextExp[level];
        }
    }

    void UpdateKillUI()
    {
        killText.text = string.Format("{0:n0}", kill); // 세 자리마다 콤마
    }

    void UpdateLevelUI()
    {
        levelText.text = string.Format("Lv.{0:D2}", level);
    }
}
