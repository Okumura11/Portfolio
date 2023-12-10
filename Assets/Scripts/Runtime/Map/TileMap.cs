using UnityEngine;

namespace Tool.Map
{
    public class TileMap : MonoBehaviour
    {
        #region 変数群
        /// <summary>タイルサイズ</summary>
        private readonly Vector3 kTileSize = Vector3.one;
        /// <summary>選択している所の線の色</summary>
        private readonly Color kSelectLineColor = Color.red;
        /// <summary>外の線の色</summary>
        private readonly Color kOutLineColor = Color.blue;
        /// <summary>マップの線の色</summary>
        private readonly Color kMapLineColor = Color.white;

        [SerializeField, Header("タイルの行数")]
        public int rows = 25;
        [SerializeField, Header("タイルの列数")]
        public int columns = 100;
        /// <summary>選択タイル位置</summary>
        private Vector3 selectTilePosition = Vector3.zero;
        #endregion 変数群

        #region 更新処理
        // グリッドの描画処理
        private void OnDrawGizmosSelected()
        {
            // オブジェクトの初期位置の取得
            Vector3 position = transform.position;
            // 外の線の色を決める
            Gizmos.color = kOutLineColor;
            // 左下から右下の線を引く
            Gizmos.DrawLine(position,
                position + new Vector3(columns * kTileSize.x, 0, 0));
            // 左下から左上の線を引く
            Gizmos.DrawLine(position,
                position + new Vector3(0, rows * kTileSize.y, 0));
            // 右下から右上の線を引く
            Gizmos.DrawLine(position + new Vector3(columns * kTileSize.x, 0, 0),
                position + new Vector3(columns * kTileSize.x, rows * kTileSize.y, 0));
            // 左上から右上の線を引く
            Gizmos.DrawLine(position + new Vector3(0, rows * kTileSize.y, 0)
                , position + new Vector3(columns * kTileSize.x, rows * kTileSize.y, 0));
            // マップの色を決める
            Gizmos.color = kMapLineColor;
            // 列数分回す
            for (float i = 1; i < columns; i++)
            {
                // 横の線を引く
                Gizmos.DrawLine(position + new Vector3(i * kTileSize.x, 0, 0), position + new Vector3(i * kTileSize.x, rows * kTileSize.y, 0));
            }
            // 行数分回す
            for (float i = 1; i < rows; i++)
            {
                // 縦の線を引く
                Gizmos.DrawLine(position + new Vector3(0, i * kTileSize.y, 0), position + new Vector3(columns * kTileSize.x, i * kTileSize.y, 0));
            }
            // 選択しているところの色を決める
            Gizmos.color = kSelectLineColor;
            // 選択している所に資格を書く
            Gizmos.DrawWireCube(selectTilePosition, new Vector3(kTileSize.x, kTileSize.y, 1) * 1.1f);
        }
        #endregion 更新処理

        #region 取得処理
        /// <summary>
        /// タイルサイズ取得処理
        /// </summary>
        /// <returns>タイルサイズ</returns>
        public Vector3 GetTileSize()
        {
            return kTileSize;
        }
        #endregion 取得処理

        #region 設定処理
        /// <summary>
        /// 選択タイル位置設定処理
        /// </summary>
        /// <param name="pos"></param>
        public void SetSelectTilePosition(Vector3 pos)
        {
            selectTilePosition = pos;
        }
        #endregion 設定処理
    }
}