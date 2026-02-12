using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // ★通信用ライブラリ

// どこからでも参照できるようにクラスを外に出しました
[System.Serializable]
public class GhostFrame
{
    public float time;
    public Vector3 pos;
    public Quaternion rot;
}

[System.Serializable]
public class GhostData
{
    public List<GhostFrame> frames = new List<GhostFrame>();
}

// サーバーに送るための手紙の封筒（データ形式）
[System.Serializable]
public class GhostPayload
{
    public string player_name;
    public float clear_time;
    public GhostData motion_data;
}

public class GhostRecorder : MonoBehaviour
{
    private bool isRecording = false;
    private GhostData recordedData = new GhostData();
    private float startTime;

    // ★Pythonサーバーのアドレス（末尾の /upload を忘れずに！）
    private string serverUrl = "https://ghost-race-server.onrender.com/upload";

    void Update()
    {
        // Rキーで録画開始
        if (Input.GetKeyDown(KeyCode.R) && !isRecording)
        {
            StartRecording();
        }
        // もう一度Rキーで録画終了
        else if (Input.GetKeyDown(KeyCode.R) && isRecording)
        {
            StopRecording();
        }

        // ★Uキーでアップロード（テスト用）
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(UploadGhostData());
        }
    }

    void StartRecording()
    {
        recordedData = new GhostData(); // データをリセット
        recordedData.frames.Clear();
        isRecording = true;
        startTime = Time.time;
        StartCoroutine(RecordCoroutine());
        Debug.Log("録画開始！");
    }

    void StopRecording()
    {
        isRecording = false;
        Debug.Log("録画終了！データ数: " + recordedData.frames.Count);
        
        // JSONを確認したいならここでおまけ表示
        string json = JsonUtility.ToJson(recordedData);
        Debug.Log("データ生成完了。Uキーを押すとアップロードします。");
    }

    IEnumerator RecordCoroutine()
    {
        while (isRecording)
        {
            GhostFrame frame = new GhostFrame();
            frame.time = Time.time - startTime;
            frame.pos = transform.position;
            frame.rot = transform.rotation;

            recordedData.frames.Add(frame);

            yield return new WaitForSeconds(0.1f); // 0.1秒ごとに記録
        }
    }

    // ★サーバーにデータを送るコルーチン
    IEnumerator UploadGhostData()
    {
        Debug.Log("アップロード開始...");

        // 送るデータを作る
        GhostPayload payload = new GhostPayload();
        payload.player_name = "UnityPlayer_" + Random.Range(0, 1000); // 名前を適当に決める
        payload.clear_time = Time.time - startTime; // とりあえず今の時間
        payload.motion_data = recordedData;

        // JSONに変換
        string json = JsonUtility.ToJson(payload);

        // POSTリクエストを作成
        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 送信！
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("成功！サーバーからの返事: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("失敗...: " + request.error);
            }
        }
    }
}