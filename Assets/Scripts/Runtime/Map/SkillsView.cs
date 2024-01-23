using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tool.Map
{
    [System.Serializable]
    public class SkillData
    {
        [Header("スキル"), SerializeField]
        public string Skill = "";
        [Header("レベル"), Range(0, 5)]
        public int Level = 0;
    }

    public class SkillsView : MonoBehaviour
    {
        [System.Serializable]
        private class StarValue
        {
            public GameObject[] array;
        }

        #region 変数群
        [SerializeField]
        private TextMeshProUGUI[] titles;
        [SerializeField]
        private StarValue[] stars;
        #endregion 変数群

        #region 設定処理
        /// <summary>
        /// データ設定処理
        /// </summary>
        /// <param name="data">データ</param>
        public void SetData(SkillData[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                SetTitle(i, data[i].Skill);
                SetStar(i, data[i].Level);
            }
        }

        /// <summary>
        /// タイトル設定処理
        /// </summary>
        /// <param name="index">配列番号</param>
        /// <param name="str">文字</param>
        private void SetTitle(int index, string str)
        {
            if (titles.Length > index)
            {
                titles[index].text = str;
            }
        }

        /// <summary>
        /// 星設定処理
        /// </summary>
        /// <param name="index">配列番号</param>
        /// <param name="value">値</param>
        private void SetStar(int index, int value)
        {
            if (stars.Length > index)
            {
                for (int i = 0; i < stars[index].array.Length; i++)
                {
                    stars[index].array[i].SetActive(i < value);
                }
            }
        }
        #endregion 設定処理
    }
}
