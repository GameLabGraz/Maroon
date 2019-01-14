using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace taecg.tool.hairShader
{
    public class HairShaderDEMOMgr : MonoBehaviour
    {
        public Toggle PointLightToggle;
        //public Toggle AutoRotateToggle;
        public Slider DirectionalLightSlider;
        public Button PreviewBtn;
        public Button NextBtn;
        public Text DescriptionText;

        public Light DirectionalLight;
        public GameObject[] ShaderGOArry;
        private int currentID;

        private float durationTime=0.5f;
        private Camera cameraMain;

        // Use this for initialization
        void Start()
        {
            cameraMain = Camera.main;

            PointLightToggle.onValueChanged.AddListener(OnPointLight);
            DirectionalLightSlider.onValueChanged.AddListener(OnDirectionalLight);
            PreviewBtn.onClick.AddListener(OnPreview);
            NextBtn.onClick.AddListener(OnNext);

            ShowDescription();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnPointLight(bool check)
        {
            ShaderGOArry[currentID].GetComponentInChildren<Light>().enabled=check;
        }

        void OnDirectionalLight(float value)
        {
            DirectionalLight.transform.rotation = Quaternion.Euler(50, value, 0);
        }

        #region 上一个下一个按钮事件
        void OnPreview()
        {
            currentID--;
            if (currentID < 0)
                currentID = ShaderGOArry.Length-1;
            StartCoroutine(WaitMoveCameraPos(new Vector3(cameraMain.transform.position.x, cameraMain.transform.position.y, -20*currentID)));
        }

        void OnNext()
        {
            currentID++;
            if(currentID>=ShaderGOArry.Length)
                currentID = 0;
            StartCoroutine(WaitMoveCameraPos(new Vector3(cameraMain.transform.position.x,cameraMain.transform.position.y,-20*currentID)));
        }

        IEnumerator WaitMoveCameraPos(Vector3 pos)
        {
            float _time=0;
            Vector3 _currentPos=cameraMain.transform.position;
            while(true)
            {
                _time += Time.deltaTime;
                cameraMain.transform.position = Vector3.Lerp(_currentPos, pos, _time / durationTime);
                if (_time >= durationTime)
                    break;
                
                yield return new WaitForEndOfFrame(); 
            }

            ShowDescription();
        }

        void ShowDescription()
        {
            switch(currentID)
            {
                case 0:
                    DescriptionText.text ="ShaderName: <color=magenta>HairPBR</color>" +
                        "\nLightModel: <color=magenta>Physicallly-BasedRendering</color>" +
                        "\n\nDescription: \nTwo common ways to support PBR, <color=magenta>Spcular</color> and <color=magenta>Metallic</color>, are best but consume more.";
                    break;
                case 1:
                    DescriptionText.text = "ShaderName: <color=magenta>HairBasic</color>" +
                        "\nLightModel: <color=magenta>Blinn-Phong</color>" +
                        "\n\nDescription: \nSupport the <color=magenta>Diffuse、Normal、Specular</color> three maps, if you have high requirements for the effect and performance, you can consider this.";
                    break;
                case 2:
                    DescriptionText.text = "ShaderName: <color=magenta>HairUnlit</color>" +
                        "\nLightModel: <color=magenta>No Lighting</color>" +
                        "\n\nDescription: \nNot affected by the light, only a Diffuse map is supported, the effect is general, but the efficiency is very high.";
                    break;
            }
        }
        #endregion
    }
}
