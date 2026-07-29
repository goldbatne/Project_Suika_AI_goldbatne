using UnityEngine;

// 화면비에 따라 orthographicSize를 조정해 "보이는 가로 폭"을 기기와 무관하게 일정하게 유지.
// ※ orthographicSize만 건드리며 Transform(위치)은 절대 만지지 않음 → CameraShake와 충돌 없음.
[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    [Tooltip("화면 좌우로 보여줄 월드 반폭. 벽이 ±2.4이므로 2.7이면 양옆에 0.3씩 여유.")]
    [SerializeField] private float targetHalfWidth = 2.7f;

    [Tooltip("orthographicSize 최소값. 가로가 넓은 화면(태블릿/PC 가로)에서 세로가 잘리는 것 방지. " +
             "floor(y=-4)·spawnY(3.7)가 항상 화면 안에 들어오도록 하는 안전장치.")]
    [SerializeField] private float minSize = 4.6f;

    private Camera cam;
    private int lastWidth;
    private int lastHeight;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Apply(); // 시작 즉시 1회 적용
    }

    void Update()
    {
        // 해상도(화면 비율)가 바뀐 경우에만 재계산 — 매 프레임 계산 금지
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            Apply();
        }
    }

    private void Apply()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam == null || !cam.orthographic) return;

        // 세로(orthographicSize)를 조절해 가로 폭을 고정
        float fitSize = targetHalfWidth / cam.aspect;

        // minSize 클램프: 가로가 넓은 화면에선 fitSize가 작아져 세로가 잘리므로 하한 보장
        cam.orthographicSize = Mathf.Max(fitSize, minSize);

        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }
}
