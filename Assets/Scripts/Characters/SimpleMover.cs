using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class SimpleMover : MonoBehaviour
{
    public float speed = 5.0f;
    public float jumpForce = 5.0f;
    private Rigidbody rb;
    private bool isGrounded;
    public bool StartMove = false; // これが true のときだけ動けるようにする
    bool startlogcount = false;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(StartMove == true) {
            // ゲームが始まっていなければ動かさない
            if (GameManager.instance.start == false) return;

            // --- 1. 入力の修正 ---
            float x = 0;
            float z = 0;

            // 横移動（A / D）
            if (Keyboard.current.dKey.isPressed) x = 1;
            else if (Keyboard.current.aKey.isPressed) x = -1;

            // 前後移動（W / S） ※ここを修正！
            if (Keyboard.current.wKey.isPressed) z = 1;
            else if (Keyboard.current.sKey.isPressed) z = -1;

            // --- 2. 移動方向の修正 ---
            // 単なる new Vector3(x, 0, z) だと「世界の方角」になってしまう。
            // transform.right（右方向）と transform.forward（前方向）を使って、
            // 「自分から見た移動方向」を作る。
            Vector3 moveDir = (transform.right * x + transform.forward * z).normalized;

            // --- 3. 速度の適用 ---
            // 入力がある時だけ速度を上書きする（何もしないときは慣性を残すか、止めるかはお好みで）
            // ここでは「キーを離すと止まる」挙動にします
            Vector3 newVelocity = moveDir * speed;

            // Y軸（ジャンプ・落下）の速度は、現在の物理挙動を維持する！
            newVelocity.y = rb.linearVelocity.y; // Unity 6対応

            rb.linearVelocity = newVelocity;

            // --- 4. ジャンプ ---
            if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded == true)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
        }
        else if(startlogcount == false)
        {
            Debug.Log("Startを押してください。");
            startlogcount = true;
        }
    }

    // 地面判定の簡易版
    // ※注意：これだと「壁」にぶつかってもジャンプ回復する可能性があります
    void OnCollisionStay(Collision collision) // Enterだと「乗っている間」が判定できないのでStayに変更
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    
    // 足が離れたら false にする処理も必要
    void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}