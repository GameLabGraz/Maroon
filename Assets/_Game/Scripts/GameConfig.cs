using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Maroon {
    public class GameConfig : MonoBehaviour {

        #region Singleton

        private static GameConfig instance = null;

        private void Awake() {
            Assert.IsNull(GameConfig.instance, "Multiple GameConfigs have been found!");
            GameConfig.instance = this;
        }

        #endregion


        #region Inspector

        [SerializeField] private InputMode inputMode = InputMode.FirstPerson;

        #endregion


        #region Accessors

        public static InputMode InputMode {
            get { return GameConfig.instance.inputMode; }
        }

        #endregion

    }
}