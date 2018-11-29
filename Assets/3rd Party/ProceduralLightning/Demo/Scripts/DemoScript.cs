//
// Procedural Lightning for Unity
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;

namespace DigitalRuby.ThunderAndLightning
{
    public class DemoScript : MonoBehaviour
    {
        public ThunderAndLightningScript ThunderAndLightningScript;
        public LightningBoltScript LightningBoltScript;
        public ParticleSystem CloudParticleSystem;
        public float MoveSpeed = 250.0f;

        private const float fastCloudSpeed = 50.0f;

        private float deltaTime;
        private float fpsIncrement;
        private string fpsText;

        private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        private RotationAxes axes = RotationAxes.MouseXAndY;
        private float sensitivityX = 15F;
        private float sensitivityY = 15F;
        private float minimumX = -360F;
        private float maximumX = 360F;
        private float minimumY = -60F;
        private float maximumY = 60F;
        private float rotationX = 0F;
        private float rotationY = 0F;
        private Quaternion originalRotation;

        private void UpdateThunder()
        {
            if (ThunderAndLightningScript != null)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ThunderAndLightningScript.CallNormalLightning();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ThunderAndLightningScript.CallIntenseLightning();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    if (CloudParticleSystem != null)
                    {
                        CloudParticleSystem.playbackSpeed = (CloudParticleSystem.playbackSpeed == 1.0f ? fastCloudSpeed : 1.0f);
                    }
                }
            }
        }

        private void UpdateMovement()
        {
            float speed = MoveSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.W))
            {
                Camera.main.transform.Translate(0.0f, 0.0f, speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                Camera.main.transform.Translate(0.0f, 0.0f, -speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                Camera.main.transform.Translate(-speed, 0.0f, 0.0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                Camera.main.transform.Translate(speed, 0.0f, 0.0f);
            }
        }

        private void UpdateMouseLook()
        {
            if (axes == RotationAxes.MouseXAndY)
            {
                // Read the mouse input axis
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

                rotationX = ClampAngle(rotationX, minimumX, maximumX);
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

                transform.localRotation = originalRotation * xQuaternion * yQuaternion;
            }
            else if (axes == RotationAxes.MouseX)
            {
                rotationX += Input.GetAxis("Mouse X") * sensitivityX;
                rotationX = ClampAngle(rotationX, minimumX, maximumX);

                Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
                transform.localRotation = originalRotation * xQuaternion;
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = ClampAngle(rotationY, minimumY, maximumY);

                Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
                transform.localRotation = originalRotation * yQuaternion;
            }
        }

        private void UpdateQuality()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                QualitySettings.SetQualityLevel(0);
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                QualitySettings.SetQualityLevel(1);
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                QualitySettings.SetQualityLevel(2);
            }
            else if (Input.GetKeyDown(KeyCode.F4))
            {
                QualitySettings.SetQualityLevel(3);
            }
            else if (Input.GetKeyDown(KeyCode.F5))
            {
                QualitySettings.SetQualityLevel(4);
            }
            else if (Input.GetKeyDown(KeyCode.F6))
            {
                QualitySettings.SetQualityLevel(5);
            }
        }

        private void UpdateOther()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.LoadLevel(0);
            }
        }

        private void OnGUI()
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();
            Rect rect = new Rect(10, h - 50, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 50;
            style.normal.textColor = Color.white;

            if ((fpsIncrement += Time.deltaTime) > 1.0f)
            {
                fpsIncrement -= 1.0f;
                float msec = deltaTime * 1000.0f;
                float fps = 1.0f / deltaTime;
                fpsText = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            }

            GUI.Label(rect, fpsText, style);
        }

        private void Update()
        {
            UpdateThunder();
            UpdateMovement();
            UpdateMouseLook();
            UpdateQuality();
            UpdateOther();
        }

        private void Start()
        {
            originalRotation = transform.localRotation;

            if (CloudParticleSystem != null)
            {
                CloudParticleSystem.playbackSpeed = fastCloudSpeed;
            }
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }

            return Mathf.Clamp(angle, min, max);
        }
    }
}