using UnityEngine;

namespace Tool.Map
{
    public class AutoMapView : MonoBehaviour
    {
        #region 変数群
        [SerializeField]
        public Cinemachine.CinemachineVirtualCamera virtualCamera;
        [SerializeField]
        public PersonDataView personDataView;
        [SerializeField]
        public SelfIntroductionView selfIntroductionView;
        [SerializeField]
        public SpecialtyView specialtyView;
        [SerializeField]
        public SkillsView skillsView;
        #endregion 変数群
    }
}
