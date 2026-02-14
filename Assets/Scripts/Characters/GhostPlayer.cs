using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    private GhostData data;
    private float elapsedTime = 0f; 
    private bool isPlaying = false; 

    // ① SetupAndPlayを「Prepare（準備）」に名称変更し、スタート処理を分離
    public void SetupAndPlay(GhostData newData)
    {
        this.data = newData;
        this.elapsedTime = 0f; 

        if (data != null && data.frames != null && data.frames.Count > 0)
        {
            // 防衛的プログラミング：もしデータに間隔が入っていなければ0.1秒とする
            if (data.recordInterval <= 0) data.recordInterval = 0.1f;

            // スタート地点（最初のフレーム）に配置だけしておく
            transform.position = data.frames[0].pos;
            transform.rotation = data.frames[0].rot;
            
            isPlaying = false; // まだ走らせない
            Debug.Log($"ゴースト準備完了：{data.player_name} (間隔: {data.recordInterval}s)");
        }
        else
        {
            Debug.LogError("渡されたゴーストデータが空です！");
        }
    }

    // ② カウントダウン終了時に外から叩くメソッド
    public void StartGhost()
    {
        if (data != null && data.frames.Count > 0)
        {
            isPlaying = true;
            Debug.Log("ゴースト、スタート！");
        }
    }

    void Update()
    {
        if (!isPlaying || data == null) return;

        elapsedTime += Time.deltaTime;

        // ③ 魔法の数字「0.1f」を「data.recordInterval」に置換
        float interval = data.recordInterval;
        int index = Mathf.FloorToInt(elapsedTime / interval);

        if (index >= data.frames.Count - 1)
        {
            var lastFrame = data.frames[data.frames.Count - 1];
            transform.position = lastFrame.pos;
            transform.rotation = lastFrame.rot;
            
            isPlaying = false;
            Debug.Log("ゴーストが完走しました");
            return;
        }

        GhostFrame frameA = data.frames[index];
        GhostFrame frameB = data.frames[index + 1];

        // 進行度 $t$ の計算も柔軟に対応
        float t = (elapsedTime % interval) / interval;

        transform.position = Vector3.Lerp(frameA.pos, frameB.pos, t);
        transform.rotation = Quaternion.Lerp(frameA.rot, frameB.rot, t);
    }
}