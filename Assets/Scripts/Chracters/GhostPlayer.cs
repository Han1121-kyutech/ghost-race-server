using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostPlayer : MonoBehaviour
{
    // 再生するデータ
    private GhostData data;

    // ★重要：GhostLoaderから呼ばれる関数
    // 「データを受け取って、すぐに再生を始める」
    public void SetupAndPlay(GhostData newData)
    {
        this.data = newData;

        // データが空でなければ再生スタート
        if (data != null && data.frames != null && data.frames.Count > 0)
        {
            StartCoroutine(PlayCoroutine());
        }
        else
        {
            Debug.LogError("渡されたゴーストデータが空です！");
        }
    }

    IEnumerator PlayCoroutine()
    {
        // データの最初から最後までループ
        for (int i = 0; i < data.frames.Count; i++)
        {
            GhostFrame frame = data.frames[i];

            // 位置と回転を再現
            transform.position = frame.pos;
            transform.rotation = frame.rot;

            // 0.1秒待つ（録画間隔と同じにする）
            yield return new WaitForSeconds(0.1f);
        }

        // 再生が終わったらどうするか？（とりあえずログを出す）
        // Debug.Log("ゴーストがゴールしました");
    }
}