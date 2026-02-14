using UnityEngine;
using UnityEngine.InputSystem; // これを足さないと新しい機能が使えません

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // マウスの感度
    public Transform playerBody;         // プレイヤー本体のTransform

    float xRotation = 0f;

    void Start()
    {
        if (GameManager.instance.start == true)
        {
            // マウスカーソルを画面中央に固定して消す（ゲーム中に邪魔にならないように）
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    void Update()
    {
        if (GameManager.instance.start == true)
        {

            // マウスの入力を取得
            float mouseX = Mouse.current.delta.x.ReadValue()* mouseSensitivity * Time.deltaTime;
            float mouseY = Mouse.current.delta.y.ReadValue()* mouseSensitivity * Time.deltaTime;

            // 上下の回転（カメラだけを上下に動かす）
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 真上や真後ろを見すぎないように制限

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // 左右の回転（プレイヤー本体ごと回転させる）
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}