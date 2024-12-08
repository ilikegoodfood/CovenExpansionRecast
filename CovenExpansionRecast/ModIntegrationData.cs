using Assets.Code.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CovenExpansionRecast
{
    public class ModIntegrationData
    {
        public readonly ModKernel Kernel;

        public readonly Assembly Assembly;

        public readonly Dictionary<string, Type> TypeDict;

        public readonly Dictionary<string, MethodInfo> MethodInfoDict;

        public readonly Dictionary<string, FieldInfo> FieldInfoDict;

        public ModIntegrationData(ModKernel kernel)
        {
            Kernel = kernel;
            Assembly = kernel.GetType().Assembly;

            TypeDict = new Dictionary<string, Type> { { "Kernel", kernel.GetType() } };
            MethodInfoDict = new Dictionary<string, MethodInfo>();
            FieldInfoDict = new Dictionary<string, FieldInfo>();
        }
    }
}
