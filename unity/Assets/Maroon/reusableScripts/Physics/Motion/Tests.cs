using UnityEngine;

namespace Maroon.Physics.Motion
{
    public class Tests
    {
        public static void Test()
        {
            var entity = new SimulatedEntity();
            entity.SetInitialState(new State(new Vector3d(1, 0, 0), new Vector3d(0, 1, 0)));
            entity.AddExpression("fx", "-x");
            entity.AddExpression("fy", "-y");

            var sim = new Simulation();
            sim.dt = 0.1;
            sim.t0 = 0;
            sim.steps = 630;
            sim.AddEntity(entity);
            sim.Run();

            entity.PrintDataCsv("");
        }
    }
}