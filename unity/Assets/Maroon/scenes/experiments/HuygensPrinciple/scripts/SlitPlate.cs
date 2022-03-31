using System;
using UnityEngine;
using GEAR.Serialize;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Maroon.Physics.HuygensPrinciple
{
    [System.Serializable]
    public class SlitWidthChangeEvent : UnityEvent<float>
    {
    }
    
    //[ExecuteInEditMode]
    public class SlitPlate : MonoBehaviour, IResetObject
    {
        [Header("Properties")]

        [SerializeField]
        [Range(1, 5, order = 0)]
        [SerializeProperty("NumberOfSlits", order = 1)]
        private int numberOfSlits = 1;
        
        [SerializeField]
        [Range(0,5, order = 0)]
        [SerializeProperty("SlitWidth", order = 1)]
        private float slitWidth = 0.1f;

        [Header("Plate Objects")]
        
        [SerializeField]
        private GameObject top;

        [SerializeField]
        private GameObject bottom;

        [SerializeField]
        private GameObject right;

        [SerializeField]
        private GameObject left;

        [SerializeField]
        private Material plateMaterial;

        [SerializeField] private bool ignorePositionReset = false;
        [SerializeField] private bool ignoreScaleReset = false;

        public SlitWidthChangeEvent onSlitWithAdapted;

        private Vector3 TopSize => top.GetComponentInChildren<MeshRenderer>().bounds.size;
        private Vector3 BottomSize => bottom.GetComponentInChildren<MeshRenderer>().bounds.size;
        private Vector3 RightSize => right.GetComponentInChildren<MeshRenderer>().bounds.size;
        private Vector3 LeftSize => left.GetComponentInChildren<MeshRenderer>().bounds.size;
        
        public float PlateWidth => top.transform.localScale.x;    
        public float PlateHeight => TopSize.y + RightSize.y + BottomSize.y;

        private int generatorCountPerSlit;
        private float slitCenterDistance; 

        private Vector3 previousPlateScale;
        private Vector3 previousPlatePosition;

        private List<GameObject> slitCenters = new List<GameObject>();
        private List<GameObject> midSections = new List<GameObject>();
        private List<WaveGenerator> waveGeneratorList = new List<WaveGenerator>();

        public int NumberOfSlits
        {
            get => numberOfSlits;
            set
            {
                numberOfSlits = value;
                generatorCountPerSlit = CalculateGeneratorsPerSlit();
                ResetCubes();
                SetupPlateSlits(true);
            }
        }

        public float SlitWidth
        {
            get => slitWidth;
            set
            {
                slitWidth = value;
                generatorCountPerSlit = CalculateGeneratorsPerSlit();
                ResetWaveGenerators();
                SetupPlateSlits(false);
            }
        }

        public void SetNumberOfSlits(float value)
        {
            var prevWidth = slitWidth;
            numberOfSlits = (int)value;
            generatorCountPerSlit = CalculateGeneratorsPerSlit();
            ResetCubes();
            SetupPlateSlits(true);
        }

        public void SetSlitWidth(float value)
        {
            slitWidth = value;
            generatorCountPerSlit = CalculateGeneratorsPerSlit();
            ResetWaveGenerators();
            SetupPlateSlits(false);
        }
        
        private void Start()
        {
            if(top == null)
                Debug.LogError("SlitPlate::Start: Top object cannot be null.");
            if (bottom == null)
                Debug.LogError("SlitPlate::Start: Bottom object cannot be null.");
            if (right == null)
                Debug.LogError("SlitPlate::Start: Right object cannot be null.");
            if (left == null)
                Debug.LogError("SlitPlate::Start: Left object cannot be null.");
            if (plateMaterial == null)
                plateMaterial = top.GetComponent<MeshRenderer>().sharedMaterial;
            if (!Mathf.Approximately(TopSize.z, BottomSize.z))
                Debug.LogError("SlitPlate::Start: Top and Bottom object width must be equal.");

            if (!Mathf.Approximately(RightSize.y, LeftSize.y))
                Debug.LogError("SlitPlate::Start: Right and Left object height must be equal.");

            generatorCountPerSlit = CalculateGeneratorsPerSlit();
            SetupPlateSlits(false);
            StorePreviousState();
        }

        private void SetupPlateSlits(bool numberOfSlitsChanged)
        {
            var cubeCount = numberOfSlits + 1;
            var scale = right.transform.localScale;
            var scaleInBounds = PlateWidth - SlitWidth * numberOfSlits >= 0 ; 
            if (!scaleInBounds)
            {
                SlitWidth = (PlateWidth - 0.01f) / numberOfSlits;
                scaleInBounds = PlateWidth - SlitWidth * numberOfSlits >= 0;
                Debug.Assert(scaleInBounds);

                onSlitWithAdapted.Invoke(SlitWidth);
            }
            

            scale.x = scaleInBounds ? (PlateWidth - (slitWidth * numberOfSlits)) / cubeCount : 0.0f ;

            right.transform.localScale = left.transform.localScale = scale;

            if (numberOfSlitsChanged)
            {               
                for (var count = 0; count < cubeCount - 2; count++)
                {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.parent = gameObject.transform;
                    cube.transform.rotation = gameObject.transform.rotation;
                    cube.transform.localPosition = left.transform.localPosition;
                    cube.GetComponent<Renderer>().material = plateMaterial;
                    midSections.Add(cube);                 
                }
            }

            AddAllWaveGenerators();
            ScaleAndPositionPlates(scale, scaleInBounds);
            ScaleAndPositionWaveGenerators(scale, scaleInBounds); 
        }

        private void ScaleAndPositionPlates(Vector3 scale, bool scaleInBounds)
        {
            if (midSections.Count > 0)
                for (var index = 0; index < midSections.Count; index++)
                {
                    midSections[index].transform.localScale = scale;
                    if(scaleInBounds)
                    {
                        var transformCubeSection = left.transform.localPosition.x + ((scale.x + slitWidth) * (float)(index + 1)) + scale.x / 2.0f;                                
                        midSections[index].transform.localPosition = new Vector3(transformCubeSection, left.transform.localPosition.y, left.transform.localPosition.z);
                    }
                }
        }

        private void ScaleAndPositionWaveGenerators(Vector3 scale, bool scaleInBounds)
        {
            var transition = scale.x + slitWidth;        
            var initialPositionLeft = left.transform.localPosition.x + scale.x;
            var generatorPlacementTransistion = (slitWidth / (float)(generatorCountPerSlit + 1));

            slitCenterDistance = transition;

            if (slitWidth > 0.0f)
            {
                ActivatePlateWaveGenerators();
                for (var slitIndex = 0; slitIndex < numberOfSlits; slitIndex++)
                {
                    if (scaleInBounds)
                    {
                        var generatorGroupTransition = initialPositionLeft + (transition * (float)(slitIndex));
                        for (var count = 0; count < generatorCountPerSlit; count++)
                        {
                            waveGeneratorList[count + (slitIndex * (generatorCountPerSlit))].transform.localPosition = new Vector3(
                                generatorGroupTransition + (generatorPlacementTransistion * (count + 1)), 
                                left.transform.localPosition.y, 
                                left.transform.localPosition.z + 0.02f);                 
                        }
                    }                             
                }

                for (var slitCount = 0; slitCount < numberOfSlits; slitCount++)
                {
                    GameObject slitC = null;
                    if (slitCount < slitCenters.Count)
                    {
                        slitC = slitCenters[slitCount];
                    }
                    else
                    {
                        slitC = new GameObject("SlitCenter");
                        slitCenters.Add(slitC);
                    }

                    slitC.transform.parent = gameObject.transform;
                    slitC.transform.localPosition= new Vector4(
                        (initialPositionLeft + slitWidth / 2) + (transition * slitCount), 
                        left.transform.localPosition.y, 
                        left.transform.localPosition.z, 
                        0);
                }

                //just to be sure that there are only numberOfSlits elements in slitCenters
                while (slitCenters.Count > numberOfSlits)
                {
                    Destroy(slitCenters[slitCenters.Count - 1]);
                    slitCenters.RemoveAt(slitCenters.Count - 1);
                }
            }
            else
            {
                DeactivatePlateWaveGenerators();
            }
        }

        private int CalculateGeneratorsPerSlit()
        {        
            var numberOfGeneratorsPerSlit = 1 + (int)((slitWidth * 10.0f)/0.8f);
            return slitWidth == 0 ? 0 : numberOfGeneratorsPerSlit;
        }

        private void AddWaveGenerator()
        {
            var waveGenerator = WaveGeneratorPoolHandler.Instance.
                CreateWaveGenerator(WaveGenerator.GeneratorMembership.SlitPlate1);

            waveGenerator.transform.parent = gameObject.transform;
            waveGeneratorList.Add(waveGenerator);
        }

        private void AddAllWaveGenerators() 
        {
            var totalNumberOfGenerators = generatorCountPerSlit * numberOfSlits; 
            for (var count = 0; count < totalNumberOfGenerators; count++)
            {
                AddWaveGenerator();
            }
        }

        private void ActivatePlateWaveGenerators()
        {
            foreach (var generator in waveGeneratorList)
                generator.SetGeneratorActive(true);
        }

        private void DeactivatePlateWaveGenerators()
        {
            foreach (var generator in waveGeneratorList)
                generator.SetGeneratorActive(false);
        }

        private void ResetCubes()
        {
            foreach (var section in midSections)
            {
                Destroy(section);
            }

            ResetWaveGenerators();
            midSections.Clear();
        }

        public void ResetSlitCenters()
        {
            foreach(var center in slitCenters)
            {
                Destroy(center); 
            }
            slitCenters.Clear();
        }

        public void ResetWaveGenerators()
        {
            foreach (var generator in waveGeneratorList)
            {
                WaveGeneratorPoolHandler.Instance.RemoveWaveGenerator(generator);
                Destroy(generator.gameObject);
            }

            waveGeneratorList.Clear();
            ResetSlitCenters();
        }

        private void StorePreviousState()
        {
            previousPlateScale = gameObject.transform.localScale;
            previousPlatePosition = gameObject.transform.position;
        }

        private void LoadPreviousState()
        {
            if(!ignoreScaleReset)
                gameObject.transform.localScale = previousPlateScale;
            if(!ignorePositionReset)
                gameObject.transform.position = previousPlatePosition;
        }

        public void ResetObject()
        {
            ResetCubes();
            SetupPlateSlits(true);
            LoadPreviousState();
        }

        public List<GameObject> GetSlitCenters()
        {
            return slitCenters; 
        }

        public float GetDistanceBetweenSlitCenters()
        {
            return slitCenterDistance; 
        }

        public void ChangeMidSectionColors(Color col)
        {
            plateMaterial.color = col;
            
            foreach (var section in midSections)
            {
                var mr = section.GetComponent<MeshRenderer>();
                if(!mr) continue;

                mr.material.color = col;
            }
        }
    }
}
