using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("--- 이펙트 ---")]
    public GameObject mergeEffectPrefab; // 합체 시 생성할 이펙트
    [Tooltip("이 과일의 레벨 (0: 체리, 1: 딸기 ...)")]
    public int level;

    [Tooltip("합체 시 생성할 다음 단계 과일 프리팹 (최종 과일은 비워둠)")]
    public GameObject nextLevelPrefab;

    [Header("--- 최종 과일 밸런스 ---")]
    [Tooltip("최종 과일끼리 합쳐져 소멸할 때 주는 보너스 점수")]
    [SerializeField] private int finalMergeScore = 100;

    [Tooltip("실제로 투하됐는지 (대기 중인 과일은 false → 합체되지 않음)")]
    public bool isDropped = false;

    private bool hasMerged = false; // 중복 합체 방지 플래그

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. 이미 합체 처리 중이면 무시 (동시 충돌 방지)
        if (hasMerged) return;

        // 2. 아직 투하되지 않은 대기 과일은 어떤 경우에도 합체 안 함
        if (!isDropped) return;

        // 3. 부딪힌 상대가 'Fruit' 스크립트를 가지고 있는지 확인
        Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();

        // 4. 상대도 '투하된' 같은 레벨 과일일 때만 합체 (대기 과일은 상대여도 제외)
        if (otherFruit != null && otherFruit.isDropped && otherFruit.level == this.level)
        {
            // 두 과일이 서로를 합치려 할 때 InstanceID 비교로 한 쪽만 처리
            if (this.GetInstanceID() < otherFruit.GetInstanceID())
            {
                Merge(otherFruit);
            }
        }
    }

    void Merge(Fruit other)
    {
        // 두 과일을 '합체 완료' 상태로 잠금
        this.hasMerged = true;
        other.hasMerged = true;

        // 4. 두 과일의 중간 위치 계산
        Vector3 spawnPos = (this.transform.position + other.transform.position) / 2;

        // [공통] 이펙트 + 사운드 재생 (진화든 최종 소멸이든 항상)
        if (mergeEffectPrefab != null)
        {
            Instantiate(mergeEffectPrefab, spawnPos, Quaternion.identity);
        }
        SoundManager.Instance.PlayMergeSound();

        // [공통] 레벨 3 이상은 카메라 흔들기
        if (level >= 3)
        {
            float shakePower = (level * 0.05f);
            CameraShake.Instance.Shake(0.15f, shakePower);
        }

        // 5. 분기: 다음 단계가 있으면 진화, 없으면(최종) 둘 다 소멸 + 보너스
        if (nextLevelPrefab != null)
        {
            // (진화) 다음 단계 과일 생성
            GameObject newFruit = Instantiate(nextLevelPrefab, spawnPos, Quaternion.identity);

            // 이미 판에 있는 과일이므로 생성 즉시 '투하됨' 처리 → 바로 정상 합체 가능
            Fruit newFruitComp = newFruit.GetComponent<Fruit>();
            if (newFruitComp != null) newFruitComp.isDropped = true;

            Rigidbody2D rb = newFruit.GetComponent<Rigidbody2D>();
            if (rb != null) rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);

            // 일반 합체 점수: 10 * (level + 1)
            int scoreToAdd = 10 * (level + 1);
            GameManager.Instance.AddScore(scoreToAdd);

            Debug.Log($"{level}레벨 과일 합체 진화!");
        }
        else
        {
            // (최종) 다음 과일을 만들지 않고 둘 다 소멸 + 보너스 점수
            GameManager.Instance.AddScore(finalMergeScore);

            Debug.Log($"최종 과일 합체! 둘 다 소멸 + 보너스 {finalMergeScore}점");
        }

        // 6. 원본 과일 두 개 제거
        Destroy(this.gameObject);
        Destroy(other.gameObject);
    }
}
