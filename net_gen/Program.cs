using System;
// using static System.Console;

using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
//using SLCollections;
//using SLib.IO;
using System.Collections.Specialized;
using System.Reflection;
using System.Reflection.Emit;

namespace net_gen {
    public struct ValueData {
        public int x;
    }

    class L {
        public int i;
        public L (int i) {
            this.i = i;
        }

        public virtual void foo (int i, int j) {
            this.i = i;
            this.i += j;
        }
    }
    class B : L {
        public B (int i) : base (i) {
            this.i = i + 4;
        }

    }
    class Program {
        static void Main (string[] args) {
            String path = "ir.json";
            Generator generator = new Generator(path);
        }

        //public static void generate()

        // Loading Units from external assemblies
        public static List<Type> LoadTypes (Assembly assembly, String[] typeNames) {
            List<Type> types = new List<Type> ();
            foreach (String typeName in typeNames) {
                try {
                    types.Add (assembly.GetType (typeName, true, false));
                } catch (Exception) {
                    Console.WriteLine ("Library type" +
                        "loading error: Cannot load type: " + typeName +
                        " from library " + assembly.GetName ());
                }
            }
            return types;
        }

        //public static void loat()
        //{
        //    int x = 5;
        //    int localfunc(int i)
        //    {
        //        return 42 * i;
        //    }
        //    console.writeline(localfunc(x));
        //}
    }

}

namespace SlanGenerator {

}