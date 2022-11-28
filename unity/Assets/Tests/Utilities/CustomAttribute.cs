using System;
using System.Linq;

namespace Tests.Utilities {
    [AttributeUsage(AttributeTargets.Method)]
    public class SkipTestFor : Attribute
    {
        public string[] ExperimentNames { get; }

        public SkipTestFor(params string[] experimentNames) {
            ExperimentNames = experimentNames;
        }

        public static SkipTestFor GetAttributeCustom<T>(string methodName) where T : class
        {
            try
            {
                return (SkipTestFor)typeof(T).GetMethod(methodName).GetCustomAttributes(typeof(SkipTestFor), false).FirstOrDefault();
            }
            catch(SystemException)
            {
                return null;
            }
        }
    }
}