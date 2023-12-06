using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tool.Map
{
    [CustomEditor(typeof(TileMap))]
    public class TileMapEditor : Editor
    {
        #region 列挙群
        /// <summary>
        /// タイル種類
        /// </summary>
        private enum ETileType
        {
            /// <summary>橋</summary>
            Bridge,
            /// <summary>青ボタン</summary>
            ButtonPlatformBlue,
            /// <summary>赤ボタン</summary>
            ButtonPlatformRed,
            /// <summary>星</summary>
            Star,
            /// <summary>草</summary>
            Grass,
            /// <summary>土</summary>
            Dirt,
            /// <summary>罠</summary>
            Trap,
            /// <summary>水（半ブロック）</summary>
            Water1,
            /// <summary>水（全ブロック）</summary>
            Water2,
            /// <summary>ゴール</summary>
            Goal,

            /// <summary>最大</summary>
            Max,
        }

        /// <summary>
        /// マウスクリック種類
        /// </summary>
        private enum EMouseKeyType
        {
            /// <summary>左</summary>
            Left,
            /// <summary>右</summary>
            Right,
            /// <summary>ホイール</summary>
            Wheel,
        }
        #endregion 列挙群

        #region 変数群
        /// <summary>タイルのアドレ</summary>
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

        /// <summary>マウス位置</summary>
        private Vector3 mouseHitPos = Vector3.zero;
        /// <summary>選択タイル種類</summary>
        private ETileType tileType = ETileType.Bridge;
        #endregion 変数群

        #region エディター処理
        /// <summary>
        /// エディタのシーンビュー処理
        /// </summary>
        private void OnSceneGUI()
        {
            // マウスの位置がタイルマップに当たってたら
            if (UpdateHitPosition())
            {
                // シーンビューの更新
                SceneView.RepaintAll();
            }

            // マウスの位置を計算して選択しているところを決める
            RecalculateMarkerPosition();

            // 現在のイベントへの参照を取得します
            Event current = Event.current;

            // マウスの位置がレイヤーの上だったら
            if (IsMouseOnLayer())
            {
                // マウスが押した瞬間もしくはスライド中だったら
                if (current.type == EventType.MouseDown || current.type == EventType.MouseDrag)
                {
                    // 押してるのが右ボタンだったら
                    if (current.button == (int)EMouseKeyType.Right)
                    {
                        // そのブロックを消す
                        RemoveTile();
                        current.Use();
                    }
                    // 押してるのが左ボタンだったら
                    else if (current.button == (int)EMouseKeyType.Left)
                    {
                        // ブロックの描画処理
                        CreateTile();
                        current.Use();
                    }
                    // 押してるのがホイールだったら
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


            // シーンビューでUIチップを描画して、タイルの描画方法と消去方法をユーザーに通知します
            Handles.BeginGUI();
            GUI.DrawTexture(new Rect(10, Screen.height - 145, 50, 50), AssetPreview.GetAssetPreview((GameObject)Resources.Load(kTilePrefabAddress[(int)tileType])));
            GUI.Label(new Rect(10, Screen.height - 135, 200, 100), $"選択タイル: {tileType}");
            GUI.Label(new Rect(10, Screen.height - 120, 150, 100), "マウス左クリック: 生成");
            GUI.Label(new Rect(10, Screen.height - 105, 150, 100), "マウス右クリック: 削除");
            Handles.EndGUI();
        }

        /// <summary>
        /// オブジェクトがアクティブになった際の処理
        /// </summary>
        private void OnEnable()
        {
            // 現在のツールを表示ツールに設定します
            Tools.current = UnityEditor.Tool.View;
            Tools.viewTool = ViewTool.FPS;
        }
        #endregion エディター処理

        #region タイル処理
        /// <summary>
        /// タイル生成処理
        /// </summary>
        private void CreateTile()
        {
            // TileMapを取得
            TileMap map = (TileMap)target;

            // Tileのサイズ取得
            Vector3 tileSize = map.GetTileSize();

            // マウスの位置がどのタイルにあるか取得
            Vector2 tilePos = GetTilePositionFromMouseLocation();

            // その位置のブロックの中身を入れる
            GameObject cube = GameObject.Find(string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y));

            // そのブロックが存在してたら返す
            if (cube != null && cube.transform.parent != map.transform)
            {
                return;
            }

            //　nullだったら
            if (cube == null)
            {
                // ブロックを作成する
                GameObject prefab = (GameObject)Resources.Load(kTilePrefabAddress[(int)tileType]);
                cube = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }

            // ブロックの位置の初期化
            Vector3 tilePositionInLocalSpace = new Vector3((tilePos.x * tileSize.x) + (tileSize.x / 2), (tilePos.y * tileSize.y) + (tileSize.y / 2));
            cube.transform.position = map.transform.position + tilePositionInLocalSpace;

            // サイズの初期化
            cube.transform.localScale = tileSize;

            // 親子関係を結ぶ
            cube.transform.parent = map.transform;

            // ブロックの名前の初期化
            cube.name = string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y);
        }

        /// <summary>
        /// タイル削除処理
        /// </summary>
        private void RemoveTile()
        {
            // TileMapを取得
            TileMap map = (TileMap)target;

            // マウスの位置がどのタイルにあるか取得
            Vector2 tilePos = GetTilePositionFromMouseLocation();

            // 当たってるタイルを取得
            GameObject cube = GameObject.Find(string.Format("Tile_{0}_{1}", tilePos.x, tilePos.y));

            // ブロックの中がなかったら
            if (cube != null && cube.transform.parent == map.transform)
            {
                // そのブロックを消す
                UnityEngine.Object.DestroyImmediate(cube);
            }
        }
        #endregion タイル処理

        #region 操作処理
        /// <summary>
        /// マウスの位置がどのタイルにあるか取得処理
        /// </summary>
        /// <returns></returns>
        private Vector2 GetTilePositionFromMouseLocation()
        {
            // TileMapを取得s
            TileMap map = (TileMap)target;

            // Tileのサイズ取得
            Vector3 tileSize = map.GetTileSize();

            // マウスの当たってる位置から列と行の位置
            Vector3 pos = new Vector3(mouseHitPos.x / tileSize.x, mouseHitPos.y / tileSize.y, map.transform.position.z);

            // 四捨五入する
            pos = new Vector3((int)Math.Round(pos.x, 5, MidpointRounding.ToEven), (int)Math.Round(pos.y, 5, MidpointRounding.ToEven), 0);

            int col = (int)pos.x;
            int row = (int)pos.y;

            // 超えたら最大を入れる
            if (row < 0) row = 0;
            if (row > map.rows - 1) row = map.rows - 1;
            if (col < 0) col = 0;
            if (col > map.columns - 1) col = map.columns - 1;

            // 当たってる列と行を返す
            return new Vector2(col, row);
        }

        /// <summary>
        /// マウスの位置がマップに当たってるかの処理
        /// </summary>
        /// <returns></returns>
        private bool IsMouseOnLayer()
        {
            // TileMapの取得
            TileMap map = (TileMap)target;

            // Tileのサイズ取得
            Vector3 tileSize = map.GetTileSize();

            // 当たってたらtrueを返す
            if (mouseHitPos.x > 0 && mouseHitPos.x < (map.columns * tileSize.x) &&
                   mouseHitPos.y > 0 && mouseHitPos.y < (map.rows * tileSize.y))
                return true;

            // 違ったらfalseを返す
            return false;
        }

        /// <summary>
        /// マウスの位置がどのブロックに当たってるか計算する処理
        /// </summary>
        private void RecalculateMarkerPosition()
        {
            // TileMapの取得
            TileMap map = (TileMap)target;

            // Tileのサイズ取得
            Vector3 tileSize = map.GetTileSize();

            // 当たってるブロックの場所を取得
            Vector2 tilepos = GetTilePositionFromMouseLocation();

            // 当たってるブロックの位置を取得
            Vector3 pos = new Vector3(tilepos.x * tileSize.x, tilepos.y * tileSize.y, 0);

            // 選択してるブロックを代入
            map.SetSelectTilePosition(map.transform.position +
                new Vector3(pos.x + (tileSize.x / 2), pos.y + (tileSize.y / 2), 0));
        }

        /// <summary>
        /// マウスの位置がタイルマップに当たってたら
        /// </summary>
        /// <returns></returns>
        private bool UpdateHitPosition()
        {
            // TileMapの取得
            TileMap map = (TileMap)target;

            // 平面オブジェクトを作る
            Plane p = new Plane(map.transform.TransformDirection(Vector3.forward), map.transform.position);

            // 現在のマウス位置からレイタイプを構築する
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            // 当たってる位置を取得
            Vector3 hit = new Vector3();

            // 当たってる位置までの距離
            float dist;

            // 平面と交差する場所を特定するために光線を投影する
            if (p.Raycast(ray, out dist))
            {
                // 光線が平面に当たるので、ワールド空間での当たる位置を計算します。
                hit = ray.origin + (ray.direction.normalized * dist);
            }

            // 当たってる位置をワールド空間からローカル空間に変換する
            Vector3 value = map.transform.InverseTransformPoint(hit);

            // 値が異なる場合は、現在のマウスのヒット位置がtrue 
            if (value != mouseHitPos)
            {
                mouseHitPos = value;
                return true;
            }

            return false;
        }
        #endregion 操作処理
    }
}