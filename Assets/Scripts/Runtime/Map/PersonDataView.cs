using TMPro;
using UnityEngine;

namespace Tool.Map
{
    #region 列挙群
    /// <summary>
    /// 性別種類
    /// </summary>
    public enum EPersonGender
    {
        /// <summary>男性</summary>
        Male,
        /// <summary>女性</summary>
        Female,
    }
    #endregion 列挙群

    #region データクラス
    [System.Serializable]
    public class PersonData
    {
        [Header("名前"), SerializeField]
        public string Name = "";
        [Header("性別"), SerializeField]
        public EPersonGender Gender = EPersonGender.Male;
        [Header("年齢"), SerializeField]
        public int Age = 0;
        [Header("最寄り駅"), SerializeField]
        public string Station = "";
    }
    #endregion データクラス

    public class PersonDataView : MonoBehaviour
    {
        #region 変数群
        [SerializeField]
        private TextMeshProUGUI name;
        [SerializeField]
        private TextMeshProUGUI gender;
        [SerializeField]
        private TextMeshProUGUI age;
        [SerializeField]
        private TextMeshProUGUI station;
        #endregion 変数群

        #region 設定処理
        /// <summary>
        /// データ設定処理
        /// </summary>
        /// <param name="data">データ</param>
        public void SetData(PersonData data)
        {
            name.text = data.Name;
            gender.text = data.Gender == EPersonGender.Male ? "男性" : "女性";
            age.text = $"{data.Age}歳";
            station.text = data.Station;
        }
        #endregion 設定処理
    }
}
