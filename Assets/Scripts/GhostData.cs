using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // これを足さないと新しい機能が使えません

// 1フレームごとの動きデータ
[System.Serializable]
public class GhostFrame
{
    public float time;
    public Vector3 pos;
    public Quaternion rot;
}

// 録画データ全体
[System.Serializable]
public class GhostData
{
    public List<GhostFrame> frames = new List<GhostFrame>();
}

// サーバー通信用の封筒（名前やタイムも含む）
[System.Serializable]
public class GhostPayload
{
    public string player_name;
    public float clear_time;
    public GhostData motion_data;
    public string secret_key = "tyohan1121";// これがないとサーバーが受け付けてくれません（バグ防止のため）

}