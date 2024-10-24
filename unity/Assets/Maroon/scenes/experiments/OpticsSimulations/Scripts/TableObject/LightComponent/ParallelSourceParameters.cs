namespace Maroon.Physics.Optics.TableObject.LightComponent
{
    [System.Serializable]
    public class ParallelSourceParameters : LightComponentParameters
    {
        public int numberOfRays = 16;
        public float distanceBetweenRays = 0.38f / 1000.0f;
    }
}