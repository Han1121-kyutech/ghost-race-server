using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // これを足さないと新しい機能が使えません

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI設定")]
    public GameObject titlePanel;
    public InputField nameInput;
    public GameObject resultPanel;
    public Text resultTimeText;

    [Header("ゲーム設定")]
    public GhostRecorder recorder; // Playerをアタッチ
    public GhostLoader loader;     // GhostManagerをアタッチ

    private string playerName = "";
    private bool isPlaying = false;
    private float startTime;
    public bool start = false;//操作可能かどうか

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 起動時の初期化
        if (titlePanel != null) titlePanel.SetActive(true);
        if (resultPanel != null) resultPanel.SetActive(false);

        // 前回の名前を復元
        if (PlayerPrefs.HasKey("PlayerName") && nameInput != null)
        {
            nameInput.text = PlayerPrefs.GetString("PlayerName");
        }
    }

    // ★スタートボタン用
    public void OnStartButtonClick()
    {
        if (nameInput != null && nameInput.text != "")
        {
            playerName = nameInput.text;
            PlayerPrefs.SetString("PlayerName", playerName);
            start = true;//操作可能
        } else {
            Debug.LogWarning("名前を入力してください！");
            return;
        }

        if (titlePanel != null) titlePanel.SetActive(false);
        StartGame();
    }

    void StartGame()
    {
        isPlaying = true;
        startTime = Time.time;

        // 録画開始(仮)
        if (recorder != null) recorder.StartRecording();
        // ゴースト読み込み開始
        if (loader != null) loader.LoadGhosts();
    }

    // ★ゴール処理（Gキーなどで呼ぶ）
    public void FinishGame()
    {
        if (!isPlaying) return;

        isPlaying = false;
        start = false;//ゴールした後に操作させない
        float clearTime = Time.time - startTime;

        // 録画停止＆アップロード
        if (recorder != null) recorder.StopAndUpload(playerName, clearTime);

        // 結果表示
        if (resultPanel != null) resultPanel.SetActive(true);
        if (resultTimeText != null) resultTimeText.text = "Time: " + clearTime.ToString("F2") + "s";
    }

    // ★リトライボタン用
    public void OnRetryButtonClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        // 仮：Gキーでゴール
        if (isPlaying && (Keyboard.current.gKey.wasPressedThisFrame))
        {
            FinishGame();
        }
    }
}