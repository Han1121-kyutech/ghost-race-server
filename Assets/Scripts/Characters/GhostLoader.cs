using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq; // ★データの並び替えに必要

public class GhostLoader : MonoBehaviour
{
    // ★RenderのURL（末尾は /list）
    public string serverUrl = "https://ghost-race-server.onrender.com/list";
    
    public GameObject ghostPrefab;

    // サーバーからの返事を受け取る用
    [System.Serializable]
    public class GhostListResponse
    {
        public string status;
        public List<GhostPayload> ghosts;
    }

    // GameManagerから呼ばれる：ゴースト読み込み開始
    public void LoadGhosts()
    {
        StartCoroutine(DownloadGhosts());
    }

    IEnumerator DownloadGhosts()
    {
        Debug.Log("ゴーストのリストを取得中...");

        using (UnityWebRequest request = UnityWebRequest.Get(serverUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                
                // 受信データを確認
                GhostListResponse response = JsonUtility.FromJson<GhostListResponse>(json);

                if (response != null && response.ghosts != null)
                {
                    // いったん今いるゴーストを消す（もしあれば）
                    GameObject[] existingGhosts = GameObject.FindGameObjectsWithTag("Player");
                    foreach(var g in existingGhosts) {
                        if(g.name.StartsWith("Ghost_")) Destroy(g);
                    }

                    // ★★★ ここを変更しました（Top3に絞る処理） ★★★
                    
                    // 1. タイムが0より大きい（バグデータ除外）
                    // 2. タイムが速い順（小さい順）に並べる
                    // 3. 上から3人だけ取る
                    var top1Ghosts = response.ghosts
                        .Where(g => g.clear_time > 0)
                        .OrderBy(g => g.clear_time)
                        .Take(1)
                        .ToList();

                    // 選ばれし3人だけを出現させる
                    foreach (var ghostData in top1Ghosts)
                    {
                        SpawnGhost(ghostData);
                    }
                    
                    Debug.Log($"ランキング上位 {top1Ghosts.Count} 名のゴーストを召喚しました！");
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
        if (payload.motion_data == null || payload.motion_data.frames == null || payload.motion_data.frames.Count == 0) return;

        GameObject newGhost = Instantiate(ghostPrefab);
        newGhost.name = "Ghost_" + payload.player_name;

        // 半透明にする（Rendererがあれば）
        var render = newGhost.GetComponent<Renderer>();
        if (render != null) render.material.color = new Color(1f, 1f, 1f, 0.5f);

        // 再生スクリプトにデータを渡す
        GhostPlayer playerScript = newGhost.GetComponent<GhostPlayer>();
        if (playerScript != null)
        {
            playerScript.SetupAndPlay(payload.motion_data);
        }
    }
}