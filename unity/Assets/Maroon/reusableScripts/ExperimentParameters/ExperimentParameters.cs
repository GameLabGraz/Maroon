namespace Maroon.ReusableScripts.ExperimentParameters
{
    /// <summary>
    /// Base class for storing experiment parameters in a JSON-File
    /// Hint: Ensure each derived class also has the [System.Serializable] attribute; the attribute is not inherited by default.
    /// 
    /// Example javascript code and JSON format (ThreeDimensionalMotionParameters:
    /// var data = JSON.stringify([{ 
    /// Background: "Grass", 
    /// Particle: "Ball", 
    /// FunctionX: "(-0.01*(vx-(1))-0.03*(vx-(1))*sqrt((vx-(1))*(vx-(1))+(vy-(7*Exp(-x*x)))*(vy-(7*Exp(-x*x)))+(vz-(-3*Exp(-t*t)))*(vz-(-3*Exp(-t*t)))))", 
    /// FunctionY: "(-0.01*(vy-(7*Exp(-x*x)))-0.03*(vy-(7*Exp(-x*x)))*Sqrt((vx-(1))*(vx-(1))+(vy-(7*Exp(-x*x)))*(vy-(7*Exp(-x*x)))+(vz-(-3*Exp(-t*t)))*(vz-(-3*Exp(-t*t)))))", 
    /// FunctionZ: "(-0.01*(vz-(-3*Exp(-t*t)))-0.03*(vz-(-3*Exp(-t*t)))*Sqrt((vx-(1))*(vx-(1))+(vy-(7*Exp(-x*x)))*(vy-(7*Exp(-x*x)))+(vz-(-3*Exp(-t*t)))*(vz-(-3*Exp(-t*t)))))-9.81*0.1", 
    /// Mass: "0.1", 
    /// T0: "0", 
    /// DeltaT: "0.01", 
    /// Steps: "100", 
    /// X: "0", Y: "0", Z: "0", 
    /// Vx: "-7", Vy: "5", Vz: "10"
    /// }]);
    ///
    /// gameInstance.SendMessage('WebGL Receiver', 'GetDataFromJavaScript', data);
    /// </summary>
    [System.Serializable]
    public abstract class ExperimentParameters
    {
    }
}
