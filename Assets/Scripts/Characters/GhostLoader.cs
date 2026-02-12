using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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

                    // 全員出現させる
                    foreach (var ghostData in response.ghosts)
                    {
                        SpawnGhost(ghostData);
                    }
                    Debug.Log("ゴースト召喚完了！");
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