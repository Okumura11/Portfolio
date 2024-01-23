#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace Tool.Map
{
    #region スクリプタブルオブジェクト
    [CreateAssetMenu(fileName = "CreateAutoMap", menuName = "ScriptableObjects/CreateAutoMap")]
    public class CreateAutoMap : ScriptableObject
    {
        [Header("パーソンデータ"), SerializeField]
        public PersonData PersonDatas;
        [Header("自己紹介・経歴"), SerializeField, TextArea(1, 6)]
        public string SelfIntroduction = "";
        [Header("得意分野・強み・その他"), SerializeField, TextArea(1, 6)]
        public string Specialty = "";
        [Header("経験/保有スキル"), SerializeField]
        public SkillData[] SkillDatas = new SkillData[4];
    }
    #endregion スクリプタブルオブジェクト

#if UNITY_EDITOR
    [CustomEditor(typeof(CreateAutoMap))]
    public class CreateAutoMapEditor : Editor
    {
        #region インスペクター
        /// <summary>
        /// InspectorのGUIを更新
        /// </summary>
        public override void OnInspectorGUI()
        {
            //  Script,生成テーブルを表示
            base.OnInspectorGUI();

            SerializedProperty prop = this.serializedObject.FindProperty("SkillDatas");
            if (prop != null)
            {
                while (prop.arraySize > 4)
                {
                    prop.DeleteArrayElementAtIndex(4);
                }

                while (prop.arraySize < 4)
                {
                    prop.InsertArrayElementAtIndex(prop.arraySize);
                }
            }
            this.serializedObject.ApplyModifiedPropertiesWithoutUndo();

            // 作成ボタンを押したか
            if (GUILayout.Button("作成"))
            {
                //シーン作成
                CreateScene();
            }
        }
        #endregion インスペクター

        #region シーン作成処理
        /// <summary>
        /// シーン生成処理
        /// </summary>
        private void CreateScene()
        {
            //空のシーンを作成
            UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene, UnityEditor.SceneManagement.NewSceneMode.Single);
            //シーンの中身を作成
            CreateAutoMap myTarget = (CreateAutoMap)target;
            GameObject rootPrefab = Instantiate((GameObject)Resources.Load("Prefabs/SampleScenePrefab"), Vector3.zero, Quaternion.identity);
            GameObject playerPrefab = Instantiate((GameObject)Resources.Load($"Prefabs/{myTarget.PersonDatas.Gender}"),
                new Vector3(1, 1, 0), Quaternion.Euler(0, 90, 0), rootPrefab.transform);
            AutoMapView autoMapView = rootPrefab.GetComponent<AutoMapView>();
            autoMapView.virtualCamera.Follow = playerPrefab.transform;
            autoMapView.personDataView.SetData(myTarget.PersonDatas);
            autoMapView.selfIntroductionView.SetData(myTarget.SelfIntroduction);
            autoMapView.specialtyView.SetData(myTarget.Specialty);
            autoMapView.skillsView.SetData(myTarget.SkillDatas);

            //作成したシーンを起動
            EditorWindow.FocusWindowIfItsOpen(typeof(SceneView));
        }
        #endregion シーン作成処理
    }
#endif
}
