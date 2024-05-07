using UnityEngine;

namespace Maroon.Physics.Motion
{
    public class Tests
    {
        public static void Test()
        {
            var entity = new SimulatedEntity();
            entity.SetInitialState(new State(new Vector3d(0, 0, 0), new Vector3d(0, 0, 0)));
            entity.AddExpression("fx", "1.6022e-19*(5000/0.05)*H(0.05-x)");
            entity.AddExpression("fy", "1.6022e-19*(60/0.02)*H(0.1-abs(x-0.25))");
            entity.AddExpression("fz", "0");
            entity.AddExpression("m", "9.11e-31");
            entity.AddExpression("h(x)", "if(x = 0, 0.5, (1+x/abs(x))/2)");

            var sim = new Simulation();
            sim.dt = 2.9e-11;
            sim.t0 = 0;
            sim.steps = 500;
            sim.AddEntity(entity);
            sim.Run();

            entity.PrintData();
        }
    }
}