//
//Author: Tobias Stöckl
//
using UnityEngine;
using Maroon.Physics;
using Maroon.UI;
using GEAR.Localization;
using System.Collections.Generic;

namespace Maroon.PlatformControls.PC
{

    public class LaserSelectionHandler : MonoBehaviour
    {
        public GameObject CurrActiveLaser { get; private set; }
        public GameObject LaserControlPanel;
        public GameObject LaserPointerPrefab;

        [SerializeField]
        private GameObject LaserArrayStart;
        [SerializeField]
        private GameObject LaserArrayEnd;

        public QuantityFloat ActiveLaserIntensity;
        public QuantityFloat ActiveLaserWavelength;
        public QuantityFloat ActiveLaserRefractiveIndex;

        private DialogueManager _diagMan;

        private int _numLaserPointers = 0;
        public int MaxLasers = 100;


        public bool OnUIpanel { get; set; } = false;


        private Vector3 _laserArrayStartPoint;
        private Vector3 _laserArrayEndPoint;

        private Vector3 _lensPos;


        private void Start()
        {
            CurrActiveLaser = null;

            _laserArrayStartPoint = LaserArrayStart.transform.position;
            _laserArrayEndPoint = LaserArrayEnd.transform.position;
            _lensPos = GameObject.Find("LensObject").transform.position;
            _diagMan = FindObjectOfType<DialogueManager>();


            AddLaserArray();
        }
        // returns true for can add, false for cannot add
        private bool CheckLaserLimit(int toAdd)
        {
            if (_numLaserPointers + toAdd < MaxLasers)
            {
                _numLaserPointers += toAdd;
                return true;
            }
            // show dialogue for max number of lasers

            string message = LanguageManager.Instance.GetString("maxnumberoflasers");
            _diagMan.ShowMessage(string.Format(message, (_numLaserPointers + 1)));

            return false;
        }


        public void SetActiveIntensityAndWavelength(float intensity, float wavelength)
        {
            ActiveLaserIntensity.Value = intensity;
            ActiveLaserWavelength.Value = wavelength;
        }


        public void SetActiveIntensity(float intensity)
        {

            if (CurrActiveLaser != null)
            {
                CurrActiveLaser.GetComponent<Physics.LaserPointer>().LaserIntensity = intensity;
            }
        }
        public void SetActiveWavelength(float wavelength)
        {

            if (CurrActiveLaser != null)
            {
                CurrActiveLaser.GetComponent<Physics.LaserPointer>().LaserWavelength = wavelength;
            }
        }
        public GameObject[] GetAllLaserPointers()
        {
            var lps = GameObject.FindObjectsOfType<Maroon.Physics.LaserPointer>();

            List<GameObject> laserPointers = new List<GameObject>();

            foreach(var lp in lps)
            {
                laserPointers.Add(lp.gameObject);
            }

            return laserPointers.ToArray();
        }

        public void RemoveAllLaserPointers()
        {
            var lps = GetAllLaserPointers();

            foreach (var lp in lps)
            {
                Destroy(lp);
            }
            _numLaserPointers = 0;
        }

        public void AddLaserArray()
        {
            if (!CheckLaserLimit(1)) return;

            const int lasersInArray = 7;

            Vector3 offset = (_laserArrayEndPoint - _laserArrayStartPoint) / lasersInArray;

            for (int i = 1; i < lasersInArray; i++)
            {
                Instantiate(LaserPointerPrefab, _laserArrayStartPoint + i * offset, LaserPointerPrefab.transform.rotation);
            }

        }

        private void AddLaserArrayWithOptions(int numlasers, Vector3 startpoint, Vector3 endpoint, Vector3 focalpoint, float intensity, float wavelength)
        {
            if (!CheckLaserLimit(numlasers)) return;
            Vector3 offset = (endpoint - startpoint) / numlasers;

            for (int i = 1; i < numlasers; i++)
            {
                var lpprefab = Instantiate(LaserPointerPrefab, startpoint + i * offset, LaserPointerPrefab.transform.rotation);
                var lpprops = lpprefab.GetComponent<Physics.LaserPointer>();
                lpprops.LaserIntensity = intensity;
                lpprops.LaserWavelength = wavelength;
                lpprops.SetLaserColor();

                Vector3 toFocalPoint = focalpoint - lpprefab.transform.position;

                var angle = Vector3.SignedAngle(toFocalPoint, Vector3.right, Vector3.up);
                lpprefab.transform.rotation = Quaternion.Euler(0.0f, -(angle), -90.0f);
            }


        }

        public void AddFocusedLaserArray()
        {
            AddLaserArrayWithOptions(7, _laserArrayStartPoint, _laserArrayEndPoint, (_laserArrayStartPoint + _laserArrayEndPoint) / 2 + Vector3.right * 0.5f, 1.0f, 680f);
        }

        public void AddFocusedLaserArrayWithOffset()
        {
            Vector3 offsetVec = -Vector3.right * 0.3f;
            AddLaserArrayWithOptions(7, _laserArrayStartPoint + offsetVec, _laserArrayEndPoint + offsetVec, (_laserArrayStartPoint + _laserArrayEndPoint) / 2 + Vector3.right * 0.5f + offsetVec, 1.0f, 680f);
        }

        public void AddRGBLaserArray()
        {
            AddLaserArrayWithOptions(7, _laserArrayStartPoint, _laserArrayEndPoint, _laserArrayStartPoint + Vector3.right * 10000.0f, 1.0f, 450f);
            AddLaserArrayWithOptions(7, _laserArrayStartPoint - Vector3.right * 0.2f, _laserArrayEndPoint - Vector3.right * 0.2f, _laserArrayStartPoint + Vector3.right * 10000.0f, 1.0f, 500f);
            AddLaserArrayWithOptions(7, _laserArrayStartPoint - Vector3.right * 0.4f, _laserArrayEndPoint - Vector3.right * 0.4f, _laserArrayStartPoint + Vector3.right * 10000.0f, 1.0f, 700f);
        }

        public void AddLaserPointer()
        {
            if (!CheckLaserLimit(1)) return;
            Instantiate(LaserPointerPrefab, new Vector3(-1.0f, _lensPos.y, 2.5f), LaserPointerPrefab.transform.rotation);
            _numLaserPointers++;
        }
        public void RemoveLaserPointer()
        {
            if (CurrActiveLaser != null)
            {
                Destroy(CurrActiveLaser);
                _numLaserPointers--;
            }

        }

        private enum LaserArrayType
        {
            removeLaserPointers,
            addArray,
            addRGBArray,
            addFocusedArray,
            addFocusedArrayWithOffset
        }

        public void SetLaserArrangements(int arr)
        {

            if (arr != 0)
            {
                RemoveAllLaserPointers();
            }
            switch (arr)
            {
                case (int)LaserArrayType.removeLaserPointers: break;
                case (int)LaserArrayType.addArray: AddLaserArray(); break;
                case (int)LaserArrayType.addRGBArray: AddRGBLaserArray(); break;
                case (int)LaserArrayType.addFocusedArray: AddFocusedLaserArray(); break;
                case (int)LaserArrayType.addFocusedArrayWithOffset: AddFocusedLaserArrayWithOffset(); break;
                default: break;
            }
        }


        // Update is called once per frame
        private void Update()
        {

            //if current active laser is null disable lasercontrolpanel
            if (CurrActiveLaser == null)
            {
                LaserControlPanel.SetActive(false);
            }
            else
            {
                LaserControlPanel.SetActive(true);
                CurrActiveLaser.GetComponent<Physics.LaserPointer>().SetLaserColor();
            }

            if (Input.GetMouseButtonDown(0))
            {
                //shoot ray and collide with scene
                Ray testray = Camera.main.ScreenPointToRay(Input.mousePosition);

                UnityEngine.Physics.Raycast(testray.origin, testray.direction, out var rhit, Mathf.Infinity, UnityEngine.Physics.AllLayers); // todo layermask?

                if (rhit.collider.gameObject.GetComponent<Maroon.Physics.LaserPointer>() != null)
                {
                    if (CurrActiveLaser != null) CurrActiveLaser.GetComponent<DragLaserObject>().MakeInactive();
                    CurrActiveLaser = rhit.collider.gameObject;
                    CurrActiveLaser.GetComponent<DragLaserObject>().MakeActive();
                }
                else
                {
                    if (rhit.collider.gameObject.GetComponent<HandleController>() == null && CurrActiveLaser != null) //todo make klick on table deselect of laser.
                    {
                        if (rhit.collider.gameObject.name == "OpticsTable" || rhit.collider.gameObject.name == "OpticsTable2") //todo change to optical tables
                        {
                            if (!OnUIpanel)
                            {
                                CurrActiveLaser.GetComponent<DragLaserObject>().MakeInactive();
                                CurrActiveLaser = null;
                            }

                        }
                    }
                }
            }
            SetActiveIntensity(ActiveLaserIntensity);
            SetActiveWavelength(ActiveLaserWavelength);
            if (CurrActiveLaser != null)
            {
                ActiveLaserRefractiveIndex.Value = CurrActiveLaser.GetComponent<Physics.LaserPointer>().GetCauchyForCurrentWavelength();
            }
        }
    }
}