using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;

namespace Assets.Raitichan.Script.VRCAvatarBuilder {
	public class ExpressionMenuEditor : MonoBehaviour {

		[SerializeField]
		private string _workingDirectry;
		public string WorkingDirectry {
			get { return this._workingDirectry; }
			set { this._workingDirectry = value; }
		}

		[SerializeField]
		private VRCExpressionParameters _parameters = null;
		public VRCExpressionParameters Parameters {
			get { return this._parameters; }
			set { this._parameters = value; }
		}

		[SerializeField]
		private VRCExpressionsMenu _menuRoot = null;
		public VRCExpressionsMenu MenuRoot {
			get { return this._menuRoot; }
			set { this._menuRoot = value; }
		}

		[SerializeField]
		private bool _isOpenParams = false;
		public bool IsOpenParams {
			get { return this._isOpenParams; }
			set { this._isOpenParams = value; }
		}

		[SerializeField]
		private bool _isOpenMenu = false;
		public bool IsOpenMenu {
			get { return this._isOpenMenu; }
			set { this._isOpenMenu = value; }
		}



	}

}