using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maroon {
  public class InputModeObject : MonoBehaviour {
    [SerializeField] private InputMode inputMode = InputMode.VRTK;

    public void Start() {
      // IMPORTANT: Keep in mind that awake will not be called on objects that are disabled during edit time.
      // There's no way for us to receive a callback on such an object without resorting to an object higher
      // up the scene hierarchy that would take care of switching objects out. This would be way less comfortable
      // to use though.

      if (GameConfig.InputMode != inputMode) {
        gameObject.SetActive(false);
      }
    }
  }
}