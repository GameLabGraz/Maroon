using System.Collections.Generic;
using Maroon.Physics.Optics.Camera;
using Maroon.Physics.Optics.TableObject;
using Maroon.Physics.Optics.TableObject.LightComponent;
using Maroon.Physics.Optics.TableObject.OpticalComponent;
using Maroon.Physics.Optics.Util;
using Maroon.ReusableScripts.ExperimentParameters;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

namespace Maroon.Physics.Optics.Manager
{
    public class PresetManager : MonoBehaviour
    {
        public static PresetManager Instance;

        [Header("ParameterLoader")]
        [SerializeField] private ParameterLoader parameterLoader;

        [Header("Prefabs: Light Sources")]
        [SerializeField] private LaserPointer laserPointer;
        [SerializeField] private ParallelSource parallelSource;
        [SerializeField] private PointSource pointSource;
        
        [Header("Prefabs: Optical Components")]
        [SerializeField] private Aperture aperture;
        [SerializeField] private Eye eye;
        [SerializeField] private Lens lens;
        [SerializeField] private TableObject.OpticalComponent.Mirror mirror;

        [Header("Camera")] 
        [SerializeField] private GameObject mainCamera;
        private CameraControls _camControls;

        private OpticalComponentManager _ocm;
        private LightComponentManager _lcm;
        private UIManager _uim;
        private ExperimentManager _em;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Debug.LogError("SHOULD NOT OCCUR - Destroyed PresetManager");
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            _ocm = OpticalComponentManager.Instance;
            _lcm = LightComponentManager.Instance;
            _uim = UIManager.Instance;
            _em = ExperimentManager.Instance;
            _camControls = mainCamera.GetComponent<CameraControls>();
            
            parameterLoader.LoadJsonFromFileIndex(0);
        }

        public void TablePresets(int nr)
        {
            if (nr > 0) // First index should not load anything
            {
                parameterLoader.LoadJsonFromFileIndex(nr - 1);
            }
        }

        #region LoadingPresets
        public void OnLoadedExperimentParameters(ExperimentParameters experimentParameters)
        {
            if (experimentParameters is OpticsParameters)
            {
                LoadExperimentParameters((OpticsParameters) experimentParameters);
            }
            else
            {
                Debug.LogError("OnLoadedExperimentParameters requires OpticsParameters");
            }
        }

        private void LoadExperimentParameters(OpticsParameters experimentParameters)
        {
            _em.ClearTable();
            _uim.rayThickness.Value = experimentParameters.rayThickness;

            foreach (TableObjectParameters componentParameters in experimentParameters.tableObjectParameters)
            {
                TableObject.TableObject prefab = GetTableObjectPrefabForParameters(componentParameters);

                if (componentParameters is LightComponentParameters lightComponentParameters)
                {
                    LightComponent lightComp = _lcm.AddLightComponent((LightComponent)prefab, lightComponentParameters.position, lightComponentParameters.rotation, lightComponentParameters.waveLengths);

                    Type lightType = lightComponentParameters.GetType();
                    // Cannot use switch here, as switch cases require a constant expression
                    if (lightType == typeof(LaserPointerParameters))
                    {
                        ((LaserPointer)lightComp).SetParameters((LaserPointerParameters)lightComponentParameters);
                    }
                    else if (lightType == typeof(ParallelSourceParameters))
                    {
                        ((ParallelSource)lightComp).SetParameters((ParallelSourceParameters)lightComponentParameters);
                    }
                    else if (lightType == typeof(PointSourceParameters))
                    {
                        ((PointSource)lightComp).SetParameters((PointSourceParameters)lightComponentParameters);
                    }
                    else
                    {
                        Debug.LogError("Unknown type of LightComponentParameters: " + lightType);
                    }
                }
                else if (componentParameters is OpticalComponentParameters opticalComponentParameters)
                {
                    OpticalComponent opticalComp = _ocm.AddOpticalComponent((OpticalComponent)prefab, opticalComponentParameters.position, opticalComponentParameters.rotation);

                    Type opticalType = opticalComponentParameters.GetType();
                    // Cannot use switch here, as switch cases require a constant expression
                    if (opticalType == typeof(LensParameters))
                    {
                        ((Lens)opticalComp).SetParameters((LensParameters)opticalComponentParameters);
                    }
                    else if (opticalType == typeof(EyeParameters))
                    {
                        ((Eye)opticalComp).SetParameters((EyeParameters)opticalComponentParameters);
                    }
                    else if (opticalType == typeof(ApertureParameters))
                    {
                        ((Aperture)opticalComp).SetParameters((ApertureParameters)opticalComponentParameters);
                    }
                    else if (opticalType == typeof(MirrorParameters))
                    {
                        ((Mirror)opticalComp).SetParameters((MirrorParameters)opticalComponentParameters);
                    }
                    else if (opticalType == typeof(WallParameters))
                    {
                        ((Wall)opticalComp).SetParameters((WallParameters)opticalComponentParameters);
                    }
                    else
                    {
                        Debug.LogError("Unknown type of OpticalComponentParameters: " + opticalType);
                    }
                }
                else
                {
                    Debug.LogError("Unknown TableObjectParameters class.");
                }
            }

            _camControls.SetPresetCameras(
                experimentParameters.cameraSettingBaseView,
                experimentParameters.cameraSettingTopView
            );
        }

        private TableObject.TableObject GetTableObjectPrefabForParameters(TableObjectParameters parameters)
        {
            Dictionary<Type, TableObject.TableObject> prefabsPerType = new()
            {
                { typeof(LaserPointerParameters), laserPointer },
                { typeof(ParallelSourceParameters), parallelSource },
                { typeof(PointSourceParameters), pointSource },
                { typeof(MirrorParameters), mirror },
                { typeof(EyeParameters), eye },
                { typeof(LensParameters), lens },
                { typeof(ApertureParameters), aperture },
            };

            if (prefabsPerType.ContainsKey(parameters.GetType()))
                return prefabsPerType[parameters.GetType()];
            else
            {
                Debug.LogError("Unknown TableObjectParameters type: " + parameters.GetType());
                return null;
            }
        }
        #endregion

        #region Export to JSON
        /// <summary>
        /// Exports OpticsParameters to a file in the Documents folder
        /// </summary>
        /// <param name="parameters">The OpticsParameters to be stored</param>
        /// <param name="filename">The relative filename including .json extension</param>
        private void ExportToJSON(OpticsParameters parameters, string filename)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All,
            };

            string jsonText = JsonConvert.SerializeObject(parameters, settings);

            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            File.WriteAllText(Path.Combine(docPath, filename), jsonText);
            Debug.Log("Exported to file " + Path.Combine(docPath, filename));
        }

        /// <summary>
        /// Example method, how one can create and export an Optics Scenario (OpticsParameters) to a JSON file
        /// </summary>
        private void ExampleExportToJSON()
        {
            OpticsParameters parameters = new OpticsParameters()
            {
                presetNameTranslationKey = "Focal Length",
                rayThickness = Constants.BaseRayThicknessInMM,
                cameraSettingBaseView = new CameraControls.CameraSetting(new Vector3(-0.065f, 2.6f, 1.0f), Constants.BaseCamRot, 36),
                cameraSettingTopView = new CameraControls.CameraSetting(new Vector3(-0.06f, 3, 2.1f), Constants.TopCamRot, 36),

                tableObjectParameters = new List<TableObjectParameters>()
                {
                    new ParallelSourceParameters()
                    {
                        position = new Vector3(1.2f, 0, 0.62f),
                        distanceBetweenRays = 0.0038f,
                    },
                    new LensParameters()
                    {
                        position = new Vector3(1.70f, 0, 0.62f),
                        R1 = Constants.Biconvex.Item1,
                        R2 = Constants.Biconvex.Item2,
                        d1 = Constants.Biconvex.Item3,
                        d2 = Constants.Biconvex.Item4,
                    },
                },
            };

            ExportToJSON(parameters, "FocalLength.json");
        }
        #endregion


        // TODO instead of this enum, use the JSON Files provided to ParameterLoader?
        private enum TablePreset
        {
            Undefined,
            LensAndMirror,
            FocalLength,
            StandardEye,
            NearsightedEye,
            FarsightedEye,
            MagnifyingGlass,
            KeplerianTelescope,
            GalileanTelescope,
            NewtonianTelescope,
            Microscope,
            LightEmittingDiode,
            OpticalFiber
        }
    }
}