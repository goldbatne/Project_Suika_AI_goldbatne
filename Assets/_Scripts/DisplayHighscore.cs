using UnityEngine;
using TMPro;

public class DisplayHighscore : MonoBehaviour
{
    public TMP_Text scoreText;

    void Start()
    {
        // 저장된 점수 불러오기 (없으면 0)
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);

        if (scoreText != null)
            scoreText.text = "Best: " + bestScore;
    }
}