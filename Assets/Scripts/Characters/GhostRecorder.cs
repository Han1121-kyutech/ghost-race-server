using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GhostRecorder : MonoBehaviour
{
    // ★RenderのURL（自分のアプリ名になっているか確認！）
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
            recordedData.frames.Add(frame);
            
            yield return new WaitForSeconds(0.1f); // 0.1秒ごとに記録
        }
    }

    IEnumerator UploadGhostData(string playerName, float time)
    {
        Debug.Log("サーバーへアップロード中...");

        GhostPayload payload = new GhostPayload();
        payload.player_name = playerName;
        payload.clear_time = time;
        payload.motion_data = recordedData;

        string json = JsonUtility.ToJson(payload);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("アップロード成功！");
            }
            else
            {
                Debug.LogError("アップロード失敗: " + request.error);
            }
        }
    }
}