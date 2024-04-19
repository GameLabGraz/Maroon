using UnityEngine;

namespace Maroon.Physics.Motion
{
    public class Tests
    {
        public static void TestSolver()
        {
            var model = new Model("-4*vx", "-4*vy", "-4*vz - m*9.807", "1");
            var state = new State(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 10.0f), model);

            var solver = new Solver(0.01f, 200);

            solver.AddObject(state);
            solver.Solve();
            solver.PrintLog();
        }
    }
}