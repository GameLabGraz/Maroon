using UnityEngine;

namespace Maroon.Physics.Motion
{
    public class Tests
    {
        public static void Test()
        {
            Circular();
            RocketStart();
            B_deflect();
        }

        public static void Circular()
        {
            Debug.Log("Running test Circular");

            var entity = new MotionEntity();
            entity.SetInitialState(new Vector3(1, 0, 0), new Vector3(0, 1, 0));
            entity.AddExpression("fx", "-x");
            entity.AddExpression("fy", "-y");
            entity.AddExpression("fz", "0");
            entity.AddExpression("m", "1");

            var sim = new Solver();
            sim.dt = 0.01;
            sim.t0 = 0;
            sim.steps = 630;
            sim.AddEntity(entity);
            sim.Solve();

            entity.PrintData();
        }

        public static void RocketStart()
        {
            Debug.Log("Running test RocketStart");

            var entity = new MotionEntity();
            entity.SetInitialState(new Vector3(0, 0, 0), Vector3.zero);
            entity.AddExpression("fx", "(-0.01*(vx-(0.2*exp(-t*t/50)))-0.03*(vx-(0.2*exp(-t*t/50)))*sqrt((vx-(0.2*exp(-t*t/50)))*(vx-(0.2*exp(-t*t/50)))+(vy-(-0.3*exp(-t*t/50)))*(vy-(-0.3*exp(-t*t/50)))+(vz-(0))*(vz-(0))))");
            entity.AddExpression("fy", "(-0.01*(vy-(-0.3*exp(-t*t/50)))-0.03*(vy-(-0.3*exp(-t*t/50)))*sqrt((vx-(0.2*exp(-t*t/50)))*(vx-(0.2*exp(-t*t/50)))+(vy-(-0.3*exp(-t*t/50)))*(vy-(-0.3*exp(-t*t/50)))+(vz-(0))*(vz-(0))))");
            entity.AddExpression("fz", "(-0.01*(vz-(0))-0.03*(vz-(0))*sqrt((vx-(0.2*exp(-t*t/50)))*(vx-(0.2*exp(-t*t/50)))+(vy-(-0.3*exp(-t*t/50)))*(vy-(-0.3*exp(-t*t/50)))+(vz-(0))*(vz-(0))))-9.81*0.1*(2-H(3-t)*t/3-H(t-3))+5*H(3-t)");
            entity.AddExpression("m", "0.1*(2-H(3-t)*t/3-H(t-3))");
            entity.AddExpression("h(x)", "if(x == 0, 0.5, (1+x/abs(x))/2)");

            var sim = new Solver();
            sim.dt = 0.02;
            sim.t0 = 0;
            sim.steps = 500;
            sim.AddEntity(entity);
            sim.Solve();

            entity.PrintData();
        }

        public static void B_deflect()
        {
            Debug.Log("Running test B_deflect");

            var entity = new MotionEntity();
            entity.SetInitialState(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            entity.AddExpression("fx", "1.6022e-19*100000*H(0.05-x)-1.6022e-19*vy*(0.0025)*H(0.03-sqrt((x-0.15)*(x-0.15)+y*y))");
            entity.AddExpression("fy", "1.6022e-19*vx*(0.0025)*H(0.03-sqrt((x-0.15)*(x-0.15)+y*y))");
            entity.AddExpression("fz", "0");
            entity.AddExpression("m", "9.11e-31");
            entity.AddExpression("h(x)", "if(x == 0, 0.5, (1+x/abs(x))/2)");

            var sim = new Solver();
            sim.dt = 2.4E-11;
            sim.t0 = 0;
            sim.steps = 500;
            sim.AddEntity(entity);
            sim.Solve();

            entity.PrintData();
        }

    }
}