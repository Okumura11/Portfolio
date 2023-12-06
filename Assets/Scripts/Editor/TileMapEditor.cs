using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tool.Map
{
    [CustomEditor(typeof(TileMap))]
    public class TileMapEditor : Editor
    {
        #region �񋓌Q
        /// <summary>
        /// �^�C�����
        /// </summary>
        private enum ETileType
        {
            /// <summary>��</summary>
            Bridge,
            /// <summary>�{�^��</summary>
            ButtonPlatformBlue,
            /// <summary>�ԃ{�^��</summary>
            ButtonPlatformRed,
            /// <summary>��</summary>
            Star,
            /// <summary>��</summary>
            Grass,
            /// <summary>�y</summary>
            Dirt,
            /// <summary>�</summary>
            Trap,
            /// <summary>���i���u���b�N�j</summary>
            Water1,
            /// <summary>���i�S�u���b�N�j</summary>
            Water2,
            /// <summary>�S�[��</summary>
            Goal,

            /// <summary>�ő�</summary>
            Max,
        }

        /// <summary>
        /// �}�E�X�N���b�N���
        /// </summary>
        private enum EMouseKeyType
        {
            /// <summary>��</summary>
            Left,
            /// <summary>�E</summary>
            Right,
            /// <summary>�z�C�[��</summary>
            Wheel,
        }
        #endregion �񋓌Q

        #region �ϐ��Q
        /// <summary>�^�C���̃A�h��</summary>
        private readonly Dictionary<int, string> kTilePrefabAddress = new Dictionary<int, string>()
        {
            { (int)ETileType.Bridge, "Prefabs/Bridge" },
            { (int)ETileType.ButtonPlatformBlue, "Prefabs/ButtonPlatformBlue" },
            { (int)ETileType.ButtonPlatformRed, "Prefabs/ButtonPlatformRed" },
            { (int)ETileType.Star, "Prefabs/Star" },
            { (int)ETileType.Grass, "Prefabs/Grass" },
            { (int)ETileType.Dirt, "Prefabs/Dirt" },
            { (int)ETileType.Trap, "Prefabs/Trap" },
            { (int)ETileType.Water1, "Prefabs/Water1" },
            { (int)ETileType.Water2, "Prefabs/Water2" },
            { (int)ETileType.Goal, "Prefabs/Waypoint" },
        };

        /// <summary>�}�E�X�ʒu</summary>
        private Vector3 mouseHitPos = Vector3.zero;
        /// <summary>�I���^�C�����</summary>
        private ETileType tileType = ETileType.Bridge;
        #endregion �ϐ��Q

        #region �G�f�B�^�[����
        /// <summary>
        /// �G�f�B�^�̃V�[���r���[����
        /// </summary>
        private void OnSceneGUI()
        {
            // �}�E�X�̈ʒu���^�C���}�b�v�ɓ������Ă���
            if (UpdateHitPosition())
            {
                // �V�[���r���[�̍X�V
                SceneView.RepaintAll();
            }

            // �}�E�X�̈ʒu���v�Z���đI�����Ă���Ƃ�������߂�
            RecalculateMarkerPosition();

            // ���݂̃C�x���g�ւ̎Q�Ƃ��擾���܂�
            Event current = Event.current;

            // �}�E�X�̈ʒu�����C���[�̏ゾ������
            if (IsMouseOnLayer())
            {
                // �}�E�X���������u�Ԃ������̓X���C�h����������
                if (current.type == EventType.MouseDown || current.type == EventType.MouseDrag)
                {
                    // �����Ă�̂��E�{�^����������
                    if (current.button == (int)EMouseKeyType.Right)
                    {
                        // ���̃u���b�N������
                        RemoveTile();
                        current.Use();
                    }
                    // �����Ă�̂����{�^����������
                    else if (current.button == (int)EMouseKeyType.Left)
                    {
                        // �u���b�N�̕`�揈��
                        CreateTile();
                        current.Use();
                    }
                    // �����Ă�̂��z�C�[����������
                    else if (current.button == (int)EMouseKeyType.Wheel && current.type == EventType.MouseDown)
                    {
                        tileType++;
                        if(tileType == ETileType.Max)
                        {
                            tileType = ETileType.Bridge;
                        }
                        current.Use();
                    }
                }
            }


            // �V�[���r���[��UI�`�b�v��`�悵�āA�^�C���̕`����@�Ə������@�����[�U�[�ɒʒm���܂�
            Handles.BeginGUI();
            GUI.DrawTexture(new Rect(10, Screen.height - 145, 50, 50), AssetPreview.GetAssetPreview((GameObject)Resources.Load(kTilePrefabAddress[(int)tileType])));
            GUI.Label(new Rect(10, Screen.height - 135, 200, 100), $"�I���^�C��: {tileType}");
            GUI.Label(new Rect(10, Screen.height - 120, 150, 100), "�}�E�X���N���b�N: ����");
            GUI.Label(new Rect(10, Screen.height - 105, 150, 100), "�}�E�X�E�N���b�N: �폜");
            Handles.EndGUI();
        }

        /// <summary>
        /// �I�u�W�F�N�g���A�N�e�B�u�ɂȂ����ۂ̏���
        /// </summary>
        private void OnEnable()
        {
            // ���݂̃c�[����\���c�[���ɐݒ肵�܂�
            Tools.current = UnityEditor.Tool.View;
            Tools.viewTool = ViewTool.FPS;
        }
        #endregion �G�f�B�^�[����

        #region �^�C������
        /// <summary>
        /// �^�C����������
        /// </summary>
        private void CreateTile()
        {
            // TileMap���擾
            TileMap map = (TileMap)target;

            // Tile�̃T�C�Y�擾
            Vector3 tileSize = map.GetTileSize();

            // �}�E�X�̈ʒu���ǂ̃^�C���ɂ��邩�擾
            Vector2 tilePos = GetTilePositionFromMouseLocation();

            // ���̈ʒu�̃u���b�N�̒��g������
            GameObject cube = GameObject.Find(string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y));

            // ���̃u���b�N�����݂��Ă���Ԃ�
            if (cube != null && cube.transform.parent != map.transform)
            {
                return;
            }

            //�@null��������
            if (cube == null)
            {
                // �u���b�N���쐬����
                GameObject prefab = (GameObject)Resources.Load(kTilePrefabAddress[(int)tileType]);
                cube = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }

            // �u���b�N�̈ʒu�̏�����
            Vector3 tilePositionInLocalSpace = new Vector3((tilePos.x * tileSize.x) + (tileSize.x / 2), (tilePos.y * tileSize.y) + (tileSize.y / 2));
            cube.transform.position = map.transform.position + tilePositionInLocalSpace;

            // �T�C�Y�̏�����
            cube.transform.localScale = tileSize;

            // �e�q�֌W������
            cube.transform.parent = map.transform;

            // �u���b�N�̖��O�̏�����
            cube.name = string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y);
        }

        /// <summary>
        /// �^�C���폜����
        /// </summary>
        private void RemoveTile()
        {
            // TileMap���擾
            TileMap map = (TileMap)target;

            // �}�E�X�̈ʒu���ǂ̃^�C���ɂ��邩�擾
            Vector2 tilePos = GetTilePositionFromMouseLocation();

            // �������Ă�^�C�����擾
            GameObject cube = GameObject.Find(string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y));

            // �u���b�N�̒����Ȃ�������
            if (cube != null && cube.transform.parent == map.transform)
            {
                // ���̃u���b�N������
                UnityEngine.Object.DestroyImmediate(cube);
            }
        }
        #endregion �^�C������

        #region ���쏈��
        /// <summary>
        /// �}�E�X�̈ʒu���ǂ̃^�C���ɂ��邩�擾����
        /// </summary>
        /// <returns></returns>
        private Vector2 GetTilePositionFromMouseLocation()
        {
            // TileMap���擾s
            TileMap map = (TileMap)target;

            // Tile�̃T�C�Y�擾
            Vector3 tileSize = map.GetTileSize();

            // �}�E�X�̓������Ă�ʒu�����ƍs�̈ʒu
            Vector3 pos = new Vector3(mouseHitPos.x / tileSize.x, mouseHitPos.y / tileSize.y, map.transform.position.z);

            // �l�̌ܓ�����
            pos = new Vector3((int)Math.Round(pos.x, 5, MidpointRounding.ToEven), (int)Math.Round(pos.y, 5, MidpointRounding.ToEven), 0);

            int col = (int)pos.x;
            int row = (int)pos.y;

            // ��������ő������
            if (row < 0) row = 0;
            if (row > map.rows - 1) row = map.rows - 1;
            if (col < 0) col = 0;
            if (col > map.columns - 1) col = map.columns - 1;

            // �������Ă��ƍs��Ԃ�
            return new Vector2(col, row);
        }

        /// <summary>
        /// �}�E�X�̈ʒu���}�b�v�ɓ������Ă邩�̏���
        /// </summary>
        /// <returns></returns>
        private bool IsMouseOnLayer()
        {
            // TileMap�̎擾
            TileMap map = (TileMap)target;

            // Tile�̃T�C�Y�擾
            Vector3 tileSize = map.GetTileSize();

            // �������Ă���true��Ԃ�
            if (mouseHitPos.x > 0 && mouseHitPos.x < (map.columns * tileSize.x) &&
                   mouseHitPos.y > 0 && mouseHitPos.y < (map.rows * tileSize.y))
                return true;

            // �������false��Ԃ�
            return false;
        }

        /// <summary>
        /// �}�E�X�̈ʒu���ǂ̃u���b�N�ɓ������Ă邩�v�Z���鏈��
        /// </summary>
        private void RecalculateMarkerPosition()
        {
            // TileMap�̎擾
            TileMap map = (TileMap)target;

            // Tile�̃T�C�Y�擾
            Vector3 tileSize = map.GetTileSize();

            // �������Ă�u���b�N�̏ꏊ���擾
            Vector2 tilepos = GetTilePositionFromMouseLocation();

            // �������Ă�u���b�N�̈ʒu���擾
            Vector3 pos = new Vector3(tilepos.x * tileSize.x, tilepos.y * tileSize.y, 0);

            // �I�����Ă�u���b�N����
            map.SetSelectTilePosition(map.transform.position +
                new Vector3(pos.x + (tileSize.x / 2), pos.y + (tileSize.y / 2), 0));
        }

        /// <summary>
        /// �}�E�X�̈ʒu���^�C���}�b�v�ɓ������Ă���
        /// </summary>
        /// <returns></returns>
        private bool UpdateHitPosition()
        {
            // TileMap�̎擾
            TileMap map = (TileMap)target;

            // ���ʃI�u�W�F�N�g�����
            Plane p = new Plane(map.transform.TransformDirection(Vector3.forward), map.transform.position);

            // ���݂̃}�E�X�ʒu���烌�C�^�C�v���\�z����
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            // �������Ă�ʒu���擾
            Vector3 hit = new Vector3();

            // �������Ă�ʒu�܂ł̋���
            float dist;

            // ���ʂƌ�������ꏊ����肷�邽�߂Ɍ����𓊉e����
            if (p.Raycast(ray, out dist))
            {
                // ���������ʂɓ�����̂ŁA���[���h��Ԃł̓�����ʒu���v�Z���܂��B
                hit = ray.origin + (ray.direction.normalized * dist);
            }

            // �������Ă�ʒu�����[���h��Ԃ��烍�[�J����Ԃɕϊ�����
            Vector3 value = map.transform.InverseTransformPoint(hit);

            // �l���قȂ�ꍇ�́A���݂̃}�E�X�̃q�b�g�ʒu��true 
            if (value != mouseHitPos)
            {
                mouseHitPos = value;
                return true;
            }

            return false;
        }
        #endregion ���쏈��
    }
}