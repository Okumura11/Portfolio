using TMPro;
using UnityEngine;

namespace Tool.Map
{
    public class SelfIntroductionView : MonoBehaviour
    {
        #region 変数群
        [SerializeField]
        private TextMeshProUGUI selfIntroduction;
        #endregion 変数群

        #region 設定処理
        /// <summary>
        /// データ設定処理
        /// </summary>
        /// <param name="str">文字</param>
        public void SetData(string str)
        {
            selfIntroduction.text = str;
        }
        #endregion 設定処理
    }
}
