using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Time.timeScale = 1;
    }

    [Header("--- 과일/스폰 ---")]
    public GameObject[] fruitPrefabs;
    public Sprite[] fruitSprites;
    public float spawnY = 4.0f;
    public float xLimit = 2.4f;

    [Header("--- 시작 과일 ---")]
    [Tooltip("시작 시 등장할 하위 과일 종류 수 (예: 5 = 체리~감)")]
    [SerializeField] private int startFruitRange = 5;

    [Header("--- UI 참조 ---")]
    public TMP_Text scoreText;
    public TMP_Text bestScoreText;
    public GameObject gameOverPanel;
    public Image nextFruitImage;

    [Header("--- 옵션/볼륨 UI ---")]
    public GameObject optionPanel;
    public Slider volumeSlider;

    [Header("--- 코인/스킬 UI ---")]
    public TMP_Text coinText;          // 코인 표시 텍스트
    public TMP_Text shakeCountText;    // 흔들기 스킬 횟수 표시 텍스트
    public GameObject shopPanel;       // 상점 화면 패널
    public Button skillButton;         // 스킬 사용 버튼
    public TMP_Text messageText;       // 안내 메시지 (구매 실패/스킬 없음 등)

    [Header("--- 메시지 ---")]
    [Tooltip("안내 메시지가 화면에 유지되는 시간(초)")]
    [SerializeField] private float messageDuration = 1.5f;

    [Header("--- 경제 밸런스 (인스펙터 조절) ---")]
    [Tooltip("스킬 1회 구매 가격 (코인)")]
    [SerializeField] private int shakePrice = 20;
    [Tooltip("게임 시작 시 기본 스킬 횟수")]
    [SerializeField] private int startShakeCount = 1;

    [Header("--- 흔들기 스킬 밸런스 (인스펙터 조절) ---")]
    [Tooltip("아래로 눌러 담는 세기 (대략 속도, 질량과 무관하게 적용)")]
    [SerializeField] private float shakeForce = 30f;
    [Tooltip("이 Y보다 위(데드존)에 걸친 과일은 흔들기 대상에서 제외")]
    [SerializeField] private float deadZoneY = 2.5f;

    // 게임 상태
    private GameObject currentFruitObj;
    private int nextFruitIndex;
    private bool isReady = false;

    private int currentScore = 0;

    // 코인/스킬 상태 (매 판 초기화 — PlayerPrefs 저장/로드 안 함)
    private int currentCoins = 0;
    private int shakeCount = 0;

    void Start()
    {
        // 1. 최고 점수만 PlayerPrefs로 계속 유지
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        UpdateBestScoreUI(bestScore);

        // 2. 코인과 스킬 횟수는 매 판 초기화 (코인 0, 스킬은 기본값)
        currentCoins = 0;
        shakeCount = startShakeCount;

        UpdateScoreUI();
        UpdateCoinUI();
        UpdateShakeCountUI();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (optionPanel != null) optionPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (messageText != null) messageText.gameObject.SetActive(false);

        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        nextFruitIndex = Random.Range(0, startFruitRange);
        SpawnNextFruit();
    }

    void Update()
    {
        if (Time.timeScale == 0 || !isReady) return;

        if (currentFruitObj != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            float clampX = Mathf.Clamp(worldPos.x, -xLimit, xLimit);
            currentFruitObj.transform.position = new Vector3(clampX, spawnY, 0);
        }

        // UI 버튼(옵션/상점/흔들기 등) 위를 클릭한 경우엔 과일을 떨어뜨리지 않음
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            DropFruit();
        }
    }

    // 마우스 포인터가 UI 요소 위에 있는지 검사
    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    void SpawnNextFruit()
    {
        Vector3 spawnPos = new Vector3(0, spawnY, 0);
        currentFruitObj = Instantiate(fruitPrefabs[nextFruitIndex], spawnPos, Quaternion.identity);

        Rigidbody2D rb = currentFruitObj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.isKinematic = true;

        nextFruitIndex = Random.Range(0, startFruitRange);

        if (nextFruitImage != null && nextFruitIndex < fruitSprites.Length)
        {
            nextFruitImage.sprite = fruitSprites[nextFruitIndex];
        }

        isReady = true;
    }

    void DropFruit()
    {
        if (currentFruitObj == null) return;

        Rigidbody2D rb = currentFruitObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            SoundManager.Instance.PlayDropSound();
            rb.AddForce(Vector2.down * 1f, ForceMode2D.Impulse);
        }

        // 투하하는 순간부터 합체 판정 허용 (대기 중엔 합쳐지지 않음)
        Fruit fruitComp = currentFruitObj.GetComponent<Fruit>();
        if (fruitComp != null) fruitComp.isDropped = true;

        currentFruitObj = null;
        isReady = false;

        Invoke("SpawnNextFruit", 1.0f);
    }

    // --- 점수 획득 + 코인 적립 (점수의 1/10, 버림) ---
    public void AddScore(int amount)
    {
        currentScore += amount;

        // 코인은 점수의 1/10 (정수 나눗셈 = 버림). 판 단위라 저장하지 않음.
        currentCoins += (amount / 10);

        UpdateScoreUI();
        UpdateCoinUI();

        int currentBest = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScore > currentBest)
        {
            PlayerPrefs.SetInt("BestScore", currentScore);
            PlayerPrefs.Save();
            UpdateBestScoreUI(currentScore);
        }
    }

    void UpdateScoreUI() { if (scoreText != null) scoreText.text = "Score: " + currentScore; }
    void UpdateBestScoreUI(int score) { if (bestScoreText != null) bestScoreText.text = "Best: " + score; }

    void UpdateCoinUI() { if (coinText != null) coinText.text = "coin : " + currentCoins; }
    void UpdateShakeCountUI() { if (shakeCountText != null) shakeCountText.text = "count : " + shakeCount; }

    public void GameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- 옵션 관련 ---
    public void OpenOption() { if (optionPanel != null) optionPanel.SetActive(true); Time.timeScale = 0; }
    public void CloseOption() { if (optionPanel != null) optionPanel.SetActive(false); Time.timeScale = 1; }
    public void GoToMainMenu() { Time.timeScale = 1; SceneManager.LoadScene("TitleScene"); }
    public void SetVolume(float volume) { AudioListener.volume = volume; }

    // --- 상점 관련 ---
    public void OpenShop() { if (shopPanel != null) shopPanel.SetActive(true); Time.timeScale = 0; }
    public void CloseShop() { if (shopPanel != null) shopPanel.SetActive(false); Time.timeScale = 1; }

    public void BuyShakeSkill()
    {
        if (currentCoins >= shakePrice)
        {
            currentCoins -= shakePrice;
            shakeCount++;

            // 코인/스킬 횟수는 판 단위 — PlayerPrefs 저장하지 않음
            UpdateCoinUI();
            UpdateShakeCountUI();
        }
        else
        {
            // 코인 부족 안내
            ShowMessage("Not enough coins!");
        }
    }

    // --- 안내 메시지: 잠깐 띄웠다가 자동으로 사라짐 (timeScale 0에서도 동작) ---
    private Coroutine messageRoutine;

    public void ShowMessage(string msg)
    {
        if (messageText == null) return;

        messageText.text = msg;
        messageText.gameObject.SetActive(true);

        if (messageRoutine != null) StopCoroutine(messageRoutine);
        messageRoutine = StartCoroutine(HideMessageAfter(messageDuration));
    }

    private IEnumerator HideMessageAfter(float seconds)
    {
        // 상점/옵션은 Time.timeScale = 0 상태라 Realtime으로 대기해야 함
        yield return new WaitForSecondsRealtime(seconds);
        if (messageText != null) messageText.gameObject.SetActive(false);
        messageRoutine = null;
    }

    // --- 흔들기 스킬: 데드존 아래 과일만 아래+중앙으로 눌러 담기 (위로 안 튐) ---
    public void OnClickShakeSkill()
    {
        if (shakeCount <= 0)
        {
            // 남은 스킬이 없으면 안내 후 종료
            ShowMessage("Out of Shakes!");
            return;
        }

        // 1. 횟수 차감 (판 단위 — 저장 없음)
        shakeCount--;
        UpdateShakeCountUI();

        // 2. 화면 흔들기
        CameraShake.Instance.Shake(0.5f, 0.5f);

        // 3. 데드존 아래 과일만 "아래 + 중앙"으로 눌러 담음 (절대 위로 안 튀게)
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Fruit");
        foreach (GameObject fruit in fruits)
        {
            // (a) 데드존에 걸친 위험한 과일은 제외
            if (fruit.transform.position.y >= deadZoneY) continue;

            Rigidbody2D rb = fruit.GetComponent<Rigidbody2D>();
            if (rb == null) continue;

            // (b) 수평은 화면 중앙(x=0)으로, 수직은 항상 아래로 (위쪽 성분 없음)
            float toCenterX = Mathf.Clamp(-fruit.transform.position.x, -1f, 1f);
            Vector2 dir = new Vector2(toCenterX + Random.Range(-0.25f, 0.25f), -1f).normalized;

            // (c) 질량과 무관하게 같은 속도로만 눌러 담기 (가벼운 과일 폭주 방지)
            rb.AddForce(dir * shakeForce * rb.mass, ForceMode2D.Impulse);
            rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
        }
    }
}
