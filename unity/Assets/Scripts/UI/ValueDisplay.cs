using UnityEngine;

namespace UI
{
    public abstract class ValueDisplay : MonoBehaviour
    {
        /// <summary>
        /// The game object from which a value should be displayed
        /// </summary>
        [SerializeField]
        private GameObject _valueObject;

        /// <summary>
        /// The value getter method name which is called to get the value.
        /// The method must take exactly one argument of type MessageArgs.
        /// The argument will be used to get the value.
        /// </summary>
        [SerializeField]
        private string _valueGetterMethod;

        protected T GetValue<T>()
        {
            var messageArgs = new MessageArgs();
            _valueObject.SendMessage(_valueGetterMethod, messageArgs);

            return (T)messageArgs.value;
        }
    }
}
