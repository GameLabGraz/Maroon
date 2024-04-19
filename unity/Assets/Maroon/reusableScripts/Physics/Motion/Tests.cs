using UnityEngine;

namespace Maroon.Physics.Motion
{
    public class Tests
    {
        public static void TestSolver()
        {
            var model = new Model("-4*vx", "-4*vy", "-4*vz - m*9.807", "1");
            var state = new State(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 10.0f), model);

            var solver_rk4 = new Solver(0.01f, 200, new RungeKutta4());
            var solver_ee = new Solver(0.01f, 200, new ExplicitEuler());
            var solver_sie = new Solver(0.01f, 200, new SemiImplicitEuler());

            solver_rk4.AddObject(state);
            solver_rk4.Solve();
            solver_rk4.PrintLog();

            solver_ee.AddObject(state);
            solver_ee.Solve();
            solver_ee.PrintLog();

            solver_sie.AddObject(state);
            solver_sie.Solve();
            solver_sie.PrintLog();
        }
    }
}