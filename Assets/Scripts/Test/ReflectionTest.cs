using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Assets.Scripts.Test
{
    public class ReflectionTest
    {

        public void Test() {

            Assembly assembly = Assembly.Load("GameFW.Nav");
            foreach (Type type in assembly.GetTypes()) {
                string t = type.Name;
            }

            Type wType = assembly.GetType("GameFW.Nav.xxx");
            assembly.CreateInstance("name");
            Object obj = Activator.CreateInstance(wType);
            obj.GetType().GetMethod("name").Invoke(null, null);
            MethodInfo mInfo = wType.GetMethod("methodName");
            mInfo.Invoke(null, null);
        }
    }
}
