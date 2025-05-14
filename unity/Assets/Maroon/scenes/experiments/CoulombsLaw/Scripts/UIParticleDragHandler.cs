using PlatformControls.PC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Maroon.Experiments.CoulombsLaw
{
    public class UIParticleDragHandler : UIItemDragHandlerSimple, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private PC_Slider _chargeValueSlider;
        [SerializeField] private ChargedParticle particlePrefab = null;
        [SerializeField] private Toggle _fixPositionToggle = null;

        [Header("Image Components")]
        [SerializeField] private Image BackgroundImage;
        [SerializeField] private Image MinusImage;
        [SerializeField] private Image PlusImage;

        private float _chargeValue = 0.0f;

        public float ChargeValue
        {
            get => _chargeValue;
            set
            {
                _chargeValue = value;

                if (Mathf.Abs(_chargeValue) < Mathf.Epsilon)
                {
                    BackgroundImage.color = Color.green;
                    MinusImage.gameObject.SetActive(false);
                    PlusImage.gameObject.SetActive(false);
                    return;
                }

                MinusImage.gameObject.SetActive(_chargeValue < 0);
                PlusImage.gameObject.SetActive(_chargeValue > 0);
                BackgroundImage.color = _chargeValue < 0 ? Color.blue : Color.red;
            }
        }

        protected override void Start()
        {
            base.Start();

            ChargeValue = _chargeValueSlider.value;
            _chargeValueSlider.onValueChanged.AddListener((newValue) => ChargeValue = newValue);
        }

        protected override void ShowObject(Vector3 position, Transform parent)
        {
            var particle = Instantiate(particlePrefab, position, Quaternion.identity, parent);
            particle.electricCharge = ChargeValue;
            particle.UpdateParticleColor();
            particle.SetFixPosition(_fixPositionToggle.isOn);
        }
    }
}
