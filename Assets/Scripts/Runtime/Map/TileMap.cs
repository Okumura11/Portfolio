using UnityEngine;

namespace Tool.Map
{
    public class TileMap : MonoBehaviour
    {
        #region �ϐ��Q
        /// <summary>�^�C���T�C�Y</summary>
        private readonly Vector3 kTileSize = Vector3.one;
        /// <summary>�I�����Ă��鏊�̐��̐F</summary>
        private readonly Color kSelectLineColor = Color.red;
        /// <summary>�O�̐��̐F</summary>
        private readonly Color kOutLineColor = Color.blue;
        /// <summary>�}�b�v�̐��̐F</summary>
        private readonly Color kMapLineColor = Color.white;

        [SerializeField, Header("�^�C���̍s��")]
        public int rows = 25;
        [SerializeField, Header("�^�C���̗�")]
        public int columns = 100;
        /// <summary>�I���^�C���ʒu</summary>
        private Vector3 selectTilePosition = Vector3.zero;
        #endregion �ϐ��Q

        #region �X�V����
        // �O���b�h�̕`�揈��
        private void OnDrawGizmosSelected()
        {
            // �I�u�W�F�N�g�̏����ʒu�̎擾
            Vector3 position = transform.position;
            // �O�̐��̐F�����߂�
            Gizmos.color = kOutLineColor;
            // ��������E���̐�������
            Gizmos.DrawLine(position,
                position + new Vector3(columns * kTileSize.x, 0, 0));
            // �������獶��̐�������
            Gizmos.DrawLine(position,
                position + new Vector3(0, rows * kTileSize.y, 0));
            // �E������E��̐�������
            Gizmos.DrawLine(position + new Vector3(columns * kTileSize.x, 0, 0),
                position + new Vector3(columns * kTileSize.x, rows * kTileSize.y, 0));
            // ���ォ��E��̐�������
            Gizmos.DrawLine(position + new Vector3(0, rows * kTileSize.y, 0)
                , position + new Vector3(columns * kTileSize.x, rows * kTileSize.y, 0));
            // �}�b�v�̐F�����߂�
            Gizmos.color = kMapLineColor;
            // �񐔕���
            for (float i = 1; i < columns; i++)
            {
                // ���̐�������
                Gizmos.DrawLine(position + new Vector3(i * kTileSize.x, 0, 0), position + new Vector3(i * kTileSize.x, rows * kTileSize.y, 0));
            }
            // �s������
            for (float i = 1; i < rows; i++)
            {
                // �c�̐�������
                Gizmos.DrawLine(position + new Vector3(0, i * kTileSize.y, 0), position + new Vector3(columns * kTileSize.x, i * kTileSize.y, 0));
            }
            // �I�����Ă���Ƃ���̐F�����߂�
            Gizmos.color = kSelectLineColor;
            // �I�����Ă��鏊�Ɏ��i������
            Gizmos.DrawWireCube(selectTilePosition, new Vector3(kTileSize.x, kTileSize.y, 1) * 1.1f);
        }
        #endregion �X�V����

        #region �擾����
        /// <summary>
        /// �^�C���T�C�Y�擾����
        /// </summary>
        /// <returns>�^�C���T�C�Y</returns>
        public Vector3 GetTileSize()
        {
            return kTileSize;
        }
        #endregion �擾����

        #region �ݒ菈��
        /// <summary>
        /// �I���^�C���ʒu�ݒ菈��
        /// </summary>
        /// <param name="pos"></param>
        public void SetSelectTilePosition(Vector3 pos)
        {
            selectTilePosition = pos;
        }
        #endregion �ݒ菈��
    }
}