using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수

public class MainTitle : MonoBehaviour
{
    // 시작 버튼에 연결할 함수
    public void ClickStart()
    {
        // 실제 게임 씬 이름과 정확히 일치해야 함
        SceneManager.LoadScene("GameScene");
    }
}
