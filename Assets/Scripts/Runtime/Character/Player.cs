using StateEvents;
using UnityEngine;

namespace Character
{
    public class Player : MonoBehaviour
    {
        #region 列挙群
        /// <summary>
        /// プレイヤー行動状態
        /// </summary>
        private enum PlayerStatus
        {
            /// <summary>生存</summary>
            Alive,
            /// <summary>死亡</summary>
            Death,
            /// <summary>勝利</summary>
            Victory,
        }
        #endregion 列挙群

        #region 変数群
        /// <summary>死亡アニメーション時間</summary>
        private const int kDeathAnimFrame = 90;
        /// <summary>死亡時間（水もしくは落ちた時）</summary>
        private const int kDeathFrame = 30;
        /// <summary>勝利アニメーション時間</summary>
        private const int kVictoryFrame = 120;
        /// <summary>レイの距離</summary>
        private const float kRayDistance = 0.2f;

        [SerializeField, Header("移動速度")]
        private float speed = 3;
        [SerializeField, Header("ジャンプ力")]
        private float jumpPower = 1800;
        /// <summary>プレイヤーアニメーション</summary>
        private PlayerAnimationComponent playerAnimation = null;
        /// <summary>物理用のコンポーネント</summary>
        private Rigidbody rb = null;
        /// <summary>地面にいるか</summary>
        private bool isGrounded = false;
        /// <summary>移動可能か</summary>
        private bool isCanMove = false;
        /// <summary>時間カウント</summary>
        private int frame = 0;
        /// <summary>時間</summary>
        private int maxFrame = 0;
        /// <summary>開始位置</summary>
        private Vector3 startPos = Vector3.zero;
        /// <summary>行動状態</summary>
        private PlayerStatus playerStatus = PlayerStatus.Alive;
        #endregion 変数群

        #region MonoBehaviourイベント
        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Awake()
        {
            Animator animator = GetComponentInChildren<Animator>();
            playerAnimation = new PlayerAnimationComponent(animator);
            rb = GetComponent<Rigidbody>();
            playerStatus = PlayerStatus.Alive;
            frame = 0;
            startPos = transform.localPosition;
            isGrounded = false;
            isCanMove = false;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        private void Update()
        {
            switch (playerStatus)
            {
                case PlayerStatus.Alive:
                    {
                        AliveUpdate();
                    }
                    break;

                case PlayerStatus.Death:
                case PlayerStatus.Victory:
                    {
                        Respawn();
                    }
                    break;
            }
        }
        #endregion MonoBehaviourイベント

        #region 更新処理
        /// <summary>
        /// 生存中処理
        /// </summary>
        private void AliveUpdate()
        {
            if (transform.localPosition.y < 0)
            {
                playerStatus = PlayerStatus.Death;
                maxFrame = kDeathFrame;
            }


            UpdateGrounded();

            // 操作処理
            //***********************************************************************************************
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.localPosition += isCanMove ? (Vector3.right * speed * Time.deltaTime) : Vector3.zero;
                transform.localRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                if (playerAnimation.IsState(PlayerAnimationComponent.IdleAnim) && isGrounded)
                {
                    playerAnimation.PlayState(PlayerAnimationComponent.RunAnim);
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.localPosition += isCanMove ? (Vector3.left * speed * Time.deltaTime) : Vector3.zero;
                transform.localRotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
                if (playerAnimation.IsState(PlayerAnimationComponent.IdleAnim) && isGrounded)
                {
                    playerAnimation.PlayState(PlayerAnimationComponent.RunAnim);
                }
            }

            if (playerAnimation.IsState(PlayerAnimationComponent.RunAnim))
            {
                if (!Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
                {
                    playerAnimation.PlayState(PlayerAnimationComponent.IdleAnim);
                }
            }

            if (playerAnimation.IsState(PlayerAnimationComponent.RunAnim) || playerAnimation.IsState(PlayerAnimationComponent.IdleAnim))
            {
                if (isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
                {
                    playerAnimation.PlayState(PlayerAnimationComponent.JumpAnim);
                    rb.AddForce(Vector3.up * jumpPower);
                }
            }
            //***********************************************************************************************
        }

        /// <summary>
        /// リスポーン処理
        /// </summary>
        private void Respawn()
        {
            frame++;
            if (frame >= maxFrame)
            {
                frame = 0;
                playerStatus = PlayerStatus.Alive;
                transform.localPosition = startPos;
                playerAnimation.PlayState(PlayerAnimationComponent.IdleAnim);
            }
        }

        /// <summary>
        /// 地面判定処理
        /// </summary>
        private void UpdateGrounded()
        {
            Vector3 rayPos = transform.position + (Vector3.up * 0.1f);
            RaycastHit[] hits = Physics.RaycastAll(rayPos, Vector3.down, kRayDistance);
            isGrounded = false;
            isCanMove = true;

            if (hits != null)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag == "Ground")
                    {
                        isGrounded = true;
                        break;
                    }
                }
            }

            hits = Physics.RaycastAll(rayPos, transform.localRotation * Vector3.forward, kRayDistance + 0.5f);
            if (hits != null)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag == "Ground")
                    {
                        isCanMove = false;
                        break;
                    }
                }
            }
        }
        #endregion 更新処理

        #region 当たり判定
        /// <summary>
        /// 当たり判定処理
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter(Collider collision)
        {
            if (playerStatus != PlayerStatus.Alive)
            {
                return;
            }

            if (collision.gameObject.tag == "Water")
            {
                playerStatus = PlayerStatus.Death;
                maxFrame = kDeathFrame;
            }
            else if (collision.gameObject.tag == "Trap")
            {
                playerStatus = PlayerStatus.Death;
                playerAnimation.PlayState(PlayerAnimationComponent.DeathAnim);
                maxFrame = kDeathAnimFrame;
            }
            else if (collision.gameObject.tag == "Goal")
            {
                playerStatus = PlayerStatus.Victory;
                playerAnimation.PlayState(PlayerAnimationComponent.Victory);
                maxFrame = kVictoryFrame;
            }
        }
        #endregion 当たり判定
    }
}
