using UnityEngine;

namespace StateEvents
{
	public class PlayerAnimationComponent
    {
        #region 変数群
        /// <summary>アイドルアニメーション</summary>
        public static readonly int IdleAnim = Animator.StringToHash("Idle");
		/// <summary>死亡アニメーション</summary>
		public static readonly int DeathAnim = Animator.StringToHash("Death");
		/// <summary>ジャンプアニメーション</summary>
		public static readonly int JumpAnim = Animator.StringToHash("Jump");
		/// <summary>移動アニメーション</summary>
		public static readonly int RunAnim = Animator.StringToHash("Run");
		/// <summary>勝利アニメーション</summary>
		public static readonly int Victory = Animator.StringToHash("Victory");
		private Animator animator;
		#endregion 変数群

		#region 初期化
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="anim">アニメーションコントローラ</param>
		public PlayerAnimationComponent(Animator anim)
		{
			animator = anim;
		}
        #endregion 初期化

		#region 再生処理
		/// <summary>
		/// アニメーション再生
		/// </summary>
		/// <param name="stateHash">アニメーションステート</param>
		/// <param name="duration">フェード時間</param>
		/// <param name="offset">オフセット</param>
		/// <returns>再生できたか</returns>
		public bool PlayState(int stateHash, float duration = 0.1f, float offset = 0f)
		{
			if (!HasState(stateHash))
			{
				return false;
			}

			var curStatenfo = GetLeadingStateInfo();
			if ((curStatenfo.shortNameHash == stateHash) || animator.IsInTransition(0))
			{
				animator.Play(stateHash, 0, offset);
			}
			else
			{
				animator.CrossFade(stateHash, duration, 0, offset);
			}

			return true;
		}
		#endregion 再生処理

		#region 代入処理
		/// <summary>
		/// Boolパラメータ代入処理
		/// </summary>
		/// <param name="stateHash">パラメータハッシュ</param>
		/// <param name="value">値</param>
		public void SetBool(int stateHash, bool value)
		{
			animator.SetBool(stateHash, value);
		}

		/// <summary>
		/// トリガー代入処理
		/// </summary>
		/// <param name="stateHash">パラメータハッシュ</param>
		/// <param name="value">値</param>
		public void SetTrigger(int stateHash, bool value)
		{
			if (value)
			{
				animator.SetTrigger(stateHash);
			}
			else
			{
				animator.ResetTrigger(stateHash);
			}
		}
		#endregion 代入処理

		#region 取得処理
		/// <summary>
		/// アニメーション再生中か取得
		/// </summary>
		/// <param name="stateHash">アニメーションハッシュ</param>
		/// <returns>そのアニメーションを再生しているか</returns>
		public bool IsState(int stateHash)
		{
			var stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
			var curStateHash = stateInfo.shortNameHash;
			return curStateHash == stateHash;
		}

		/// <summary>
		/// アニメーション再生中か取得
		/// </summary>
		/// <param name="stateHash">アニメーションハッシュ</param>
		/// <returns>そのアニメーションを再生しているか</returns>
		public bool IsState(int[] stateHash)
		{
			var stateInfo = animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
			for (var i = 0; i < stateHash.Length; ++i)
			{
				if (stateHash[i] == stateInfo.shortNameHash)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// アニメーション再生中か取得
		/// </summary>
		/// <param name="stateHash">アニメーションハッシュ</param>
		/// <returns>そのアニメーションを再生しているか</returns>
		private bool HasState(int stateHash)
		{
			return animator.HasState(0, stateHash);
		}

		/// <summary>
		/// 現在のアニメーション情報を取得
		/// </summary>
		/// <returns>現在のアニメーション情報</returns>
		private AnimatorStateInfo GetLeadingStateInfo()
		{
			return animator.IsInTransition(0) ? animator.GetNextAnimatorStateInfo(0) : animator.GetCurrentAnimatorStateInfo(0);
		}
		#endregion 取得処理
	}
}
