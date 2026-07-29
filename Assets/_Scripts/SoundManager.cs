using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("--- 오디오 소스 ---")]
    public AudioSource sfxPlayer; // 효과음 재생기
    public AudioSource bgmPlayer; // 배경음악 재생기

    [Header("--- 오디오 클립 (소리 파일) ---")]
    public AudioClip mergeSound;  // 합체 소리
    public AudioClip dropSound;   // 과일 떨어지는 소리
    public AudioClip bgmSound;    // 배경음악

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 배경음악 자동 재생
        if (bgmSound != null && bgmPlayer != null)
        {
            bgmPlayer.clip = bgmSound;
            bgmPlayer.loop = true; // 무한 반복
            bgmPlayer.volume = 0.5f; // 너무 시끄러우면 줄여
            bgmPlayer.Play();
        }
    }

    // 외부에서 부를 함수: "합체 소리 내!"
    public void PlayMergeSound()
    {
        if (sfxPlayer != null && mergeSound != null)
        {
            // PlayOneShot: 소리가 겹쳐도 끊기지 않고 겹쳐서 재생됨 (중요!)
            sfxPlayer.PlayOneShot(mergeSound);
        }
    }

    public void PlayDropSound()
    {
        if (sfxPlayer != null && dropSound != null)
        {
            sfxPlayer.PlayOneShot(dropSound, 0.7f); // 약간 작게
        }
    }
}