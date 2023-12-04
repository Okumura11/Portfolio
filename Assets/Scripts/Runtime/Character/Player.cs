using StateEvents;
using UnityEngine;

namespace Character
{
    public class Player : MonoBehaviour
    {
        #region �񋓌Q
        /// <summary>
        /// �v���C���[�s�����
        /// </summary>
        private enum PlayerStatus
        {
            /// <summary>����</summary>
            Alive,
            /// <summary>���S</summary>
            Death,
            /// <summary>����</summary>
            Victory,
        }
        #endregion �񋓌Q

        #region �ϐ��Q
        /// <summary>���S�A�j���[�V��������</summary>
        private const int kDeathAnimFrame = 90;
        /// <summary>���S���ԁi���������͗��������j</summary>
        private const int kDeathFrame = 30;
        /// <summary>�����A�j���[�V��������</summary>
        private const int kVictoryFrame = 120;
        /// <summary>���C�̋���</summary>
        private const float kRayDistance = 0.2f;

        [SerializeField, Header("�ړ����x")]
        private float speed = 3;
        [SerializeField, Header("�W�����v��")]
        private float jumpPower = 1800;
        /// <summary>�v���C���[�A�j���[�V����</summary>
        private PlayerAnimationComponent playerAnimation = null;
        /// <summary>�����p�̃R���|�[�l���g</summary>
        private Rigidbody rb = null;
        /// <summary>�n�ʂɂ��邩</summary>
        private bool isGrounded = false;
        /// <summary>�ړ��\��</summary>
        private bool isCanMove = false;
        /// <summary>���ԃJ�E���g</summary>
        private int frame = 0;
        /// <summary>����</summary>
        private int maxFrame = 0;
        /// <summary>�J�n�ʒu</summary>
        private Vector3 startPos = Vector3.zero;
        /// <summary>�s�����</summary>
        private PlayerStatus playerStatus = PlayerStatus.Alive;
        #endregion �ϐ��Q

        #region MonoBehaviour�C�x���g
        /// <summary>
        /// ����������
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
        /// �X�V����
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
        #endregion MonoBehaviour�C�x���g

        #region �X�V����
        /// <summary>
        /// ����������
        /// </summary>
        private void AliveUpdate()
        {
            if (transform.localPosition.y < 0)
            {
                playerStatus = PlayerStatus.Death;
                maxFrame = kDeathFrame;
            }


            UpdateGrounded();

            // ���쏈��
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
        /// ���X�|�[������
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
        /// �n�ʔ��菈��
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
        #endregion �X�V����

        #region �����蔻��
        /// <summary>
        /// �����蔻�菈��
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
        #endregion �����蔻��
    }
}
