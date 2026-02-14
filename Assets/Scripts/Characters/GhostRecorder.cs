using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.InputSystem; 

public class GhostRecorder : MonoBehaviour
{
    // ★RenderのURL
    private string serverUrl = "https://ghost-race-server.onrender.com/upload"; 

    private bool isRecording = false;
    private GhostData recordedData = new GhostData();
    private float recordStartTime;

    // GameManagerから呼ばれる：録画開始
    public void StartRecording()
    {
        recordedData = new GhostData();
        recordedData.frames = new List<GhostFrame>(); // リスト初期化
        isRecording = true;
        recordStartTime = Time.time;
        
        StartCoroutine(RecordCoroutine());
        Debug.Log("【Recorder】録画を開始しました");
    }

    // GameManagerから呼ばれる：停止してアップロード
    public void StopAndUpload(string playerName, float time)
    {
        isRecording = false;
        // 録画停止後、少し待たずに即アップロード処理へ
        StartCoroutine(UploadGhostData(playerName, time));
    }

    IEnumerator RecordCoroutine()
    {
        while (isRecording)
        {
            GhostFrame frame = new GhostFrame();
            frame.time = Time.time - recordStartTime;
            frame.pos = transform.position;
            frame.rot = transform.rotation;
            
            // 記録用データにフレームを追加
            recordedData.frames.Add(frame);
            
            yield return new WaitForSeconds(0.1f); // 0.1秒ごとに記録
        }
    }

    IEnumerator UploadGhostData(string playerName, float time)
    {
        Debug.Log("サーバーへアップロード中...");

        // 送信用に新しいデータを作成します
        GhostData payload = new GhostData();
        
        // 1. 基本情報の入力
        payload.player_name = playerName;
        payload.clear_time = time;
        
        // 2. ★追加：サーバーに必要な「合言葉」と「記録間隔」
        payload.secret_key = "tyohan1121"; 
        payload.recordInterval = 0.1f;     

        // 3. ★修正：入れ子にせず、記録したフレームリストを直接渡します
        // これで "frames": [...] の中に座標データが入ります
        payload.frames = recordedData.frames; 

        // JSON化
        string json = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // ログで送信内容を確認（framesに中身が入っているか確認できます）
            Debug.Log("【Unity送信直前】: " + json);
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("通信結果: " + request.downloadHandler.text);
            }
            else
            {
                // エラー内容を表示
                Debug.LogError("アップロード失敗: " + request.error);
                Debug.LogError("サーバーからの返答: " + request.downloadHandler.text);
            }
        }
    }
}