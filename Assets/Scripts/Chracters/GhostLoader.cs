using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GhostLoader : MonoBehaviour
{
    // サーバーのURL（リスト取得用）
    string serverUrl = "https://ghost-race-server.onrender.com/list";

    // 生み出すゴーストの設計図（さっき作ったプレハブをセットする）
    public GameObject ghostPrefab;

    // サーバーからの返事を受け取るためのクラス
    [System.Serializable]
    public class GhostListResponse
    {
        public string status;
        public List<GhostPayload> ghosts; // GhostRecorderと同じクラスを使う
    }

    void Start()
    {
        // ゲーム開始時にダウンロード開始
        StartCoroutine(DownloadGhosts());
    }

    void Update()
    {
    // Lキー（Load）を押したら、サーバーから最新データを取ってくる
        if (Input.GetKeyDown(KeyCode.L))
        {
            // 今いるゴーストを一度全員消してから読み直す
            GameObject[] oldGhosts = GameObject.FindGameObjectsWithTag("Player"); // ゴーストにタグがついている場合
            // もしくは単純に名前で検索して消す処理など
        
            StartCoroutine(DownloadGhosts());
            Debug.Log("最新のゴーストを再読み込み中...");
        }
    }

    IEnumerator DownloadGhosts()
    {
        Debug.Log("ゴーストのリストをダウンロード中...");

        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("受信成功！");
                string json = request.downloadHandler.text;

                // JSONを解析
                GhostListResponse response = JsonUtility.FromJson<GhostListResponse>(json);

                if (response != null && response.ghosts != null)
                {
                    // ループして全員出現させる！
                    foreach (var ghostData in response.ghosts)
                    {
                        SpawnGhost(ghostData);
                    }
                }
            }
            else
            {
                Debug.LogError("ダウンロード失敗: " + request.error);
            }
        }
    }

    void SpawnGhost(GhostPayload payload)
    {
        // 偽データ（PythonTestUser）などは除外する
        if (payload.motion_data == null || payload.motion_data.frames == null || payload.motion_data.frames.Count == 0)
        {
            return;
        }

        // プレハブからゴーストを作成
        GameObject newGhost = Instantiate(ghostPrefab);
        
        // 名前をつける（インスペクターで見やすくするため）
        newGhost.name = "Ghost_" + payload.player_name;

        // ゴーストの色をランダムに変えてみる（おまけ）
        var render = newGhost.GetComponent<Renderer>();
        if(render != null) render.material.color = new Color(1f, 1f, 1f, 0.5f); // 半透明ホワイト

        // データを渡して再生開始！
        GhostPlayer playerScript = newGhost.GetComponent<GhostPlayer>();
        playerScript.SetupAndPlay(payload.motion_data);
        
        Debug.Log(payload.player_name + " のゴーストを召喚しました！タイム: " + payload.clear_time);
    }
}