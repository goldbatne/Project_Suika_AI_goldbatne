using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    // 원래 카메라 위치 저장용
    private Vector3 originalPos;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        originalPos = transform.localPosition;
    }

    // 외부에서 부를 함수: "0.2초 동안, 0.3의 강도로 흔들어라!"
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // 랜덤한 위치로 카메라를 미친듯이 이동
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            // 다음 프레임까지 대기
            yield return null;
        }

        // 흔들기 끝났으면 원래 위치로 복귀 (중요!)
        transform.localPosition = originalPos;
    }
}