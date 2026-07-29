using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeadZone : MonoBehaviour
{
    [Header("Settings")]
    public string targetTag = "Fruit";  // 감지할 태그 이름
    public float timeLimit = 7.0f;      // 게임오버까지 총 시간 (7초, 넉넉하게)
    [Tooltip("남은 시간이 이 값 이하가 되면 카운트다운 표시 (그 전엔 조용히 누적)")]
    public float countdownStart = 5.0f; // 남은 5초부터 카운트다운 시작

    [Header("Countdown UI (데드존 카운트다운)")]
    [Tooltip("카운트다운 전체를 켜고 끄는 루트 오브젝트")]
    public GameObject countdownRoot;
    [Tooltip("시간에 따라 줄어드는 원형 링 (Image Type=Filled, Radial360)")]
    public Image countdownRing;
    [Tooltip("남은 초를 표시하는 텍스트")]
    public TMP_Text countdownText;
    [Tooltip("숫자 펄스(커졌다 작아짐) 강도")]
    public float pulseAmount = 0.15f;
    [Tooltip("숫자 펄스 속도")]
    public float pulseSpeed = 8f;

    [Header("Debug Info")]
    [SerializeField] private float timer = 0f; // 현재 누적 시간 확인용

    // 현재 데드존 안에 머무는 과일들을 개별 추적
    private readonly HashSet<GameObject> fruitsInside = new HashSet<GameObject>();

    private void Start()
    {
        // 시작 시 카운트다운은 숨김
        if (countdownRoot != null) countdownRoot.SetActive(false);
    }

    // 과일이 데드존에 들어오면 등록
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            fruitsInside.Add(collision.gameObject);
        }
    }

    // 과일이 데드존을 빠져나가면 해당 과일만 제거
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            fruitsInside.Remove(collision.gameObject);
        }
    }

    private void Update()
    {
        // 파괴된(합체/소멸된) 과일은 자동 정리
        fruitsInside.RemoveWhere(f => f == null);

        // 데드존에 과일이 하나라도 있으면 시간 누적, 하나도 없으면 리셋
        if (fruitsInside.Count > 0)
        {
            timer += Time.deltaTime;

            float remaining = Mathf.Max(0f, timeLimit - timer);

            // 남은 시간이 countdownStart 이하일 때만 카운트다운 표시
            if (remaining <= countdownStart) ShowCountdown(remaining);
            else HideCountdown();

            if (timer >= timeLimit)
            {
                Debug.Log("게임 오버!");
                HideCountdown();
                GameManager.Instance.GameOver();
            }
        }
        else
        {
            timer = 0f;
            HideCountdown();
        }
    }

    private void ShowCountdown(float remaining)
    {
        if (countdownRoot != null && !countdownRoot.activeSelf)
            countdownRoot.SetActive(true);

        // 링: 카운트다운 구간(countdownStart) 기준으로 채워짐 (5초에 꽉 참 -> 0에서 빔)
        if (countdownRing != null)
            countdownRing.fillAmount = countdownStart > 0f ? Mathf.Clamp01(remaining / countdownStart) : 0f;

        // 숫자: 남은 초(올림) + 펄스
        if (countdownText != null)
        {
            countdownText.text = Mathf.CeilToInt(remaining).ToString();
            float pulse = 1f + pulseAmount * Mathf.Abs(Mathf.Sin(Time.unscaledTime * pulseSpeed));
            countdownText.transform.localScale = Vector3.one * pulse;
        }
    }

    private void HideCountdown()
    {
        if (countdownRoot != null && countdownRoot.activeSelf)
            countdownRoot.SetActive(false);
    }
}
