using System;

using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
//using SLCollections;
//using SLib.IO;
using System.Collections.Specialized;
using System.Reflection;
using System.Reflection.Emit;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics.Contracts;

namespace net_gen {
    public class Node
    {
        public int id;
        public int parent;
        public string node;
        public Structure structure;

    }

    public abstract class Structure
    {
        
    }

    public class Literal : Structure
    {
        public string data;
    }

    public class Identifier: Structure {
        public string data { set; get; }
    }

    public class Declaration: Structure
    {
        public Identifier name{ set; get; }
        public bool isFinal{ set; get; }
        public bool isHidden{ set; get; }
        public Declaration(){

        }
        public Declaration(Declaration declaration) 
        {
            Initialize(declaration);
        }

        public void Initialize(Declaration declaration) {
            this.name = declaration.name;
            this.isFinal = declaration.isFinal;
            this.isHidden = declaration.isHidden;
        }
    }

    public class Constant: Declaration {
        public List<Expression> constants{ set; get; }
    }

    public class Parent : Declaration {
        public bool conformance{ set; get; }
        public Unit unit_ref{ set; get; }
    }

    public class Use : Declaration
    {
        public bool useConst{ set; get; }
        public Unit unit_ref{ set; get; }
    }

    public class Variable: Declaration {
        public int type{ set; get; }
        public Expression initializer{ set; get; }
        public bool isConst{ set; get; }
        public bool isRef{ set; get; }
        public bool isOverride{ set; get; }
        public bool isVal{ set; get; }
        public bool isConcurrent{ set; get; }
    }

    public class Unit: Declaration {
        public bool isReference{ set; get; }
        public bool isAbstract{ set; get; }
        public bool isConcurrent{ set; get; }
        public Identifier alias{ set; get; }
        public List<Unit> parent_units{ set; get; }
        public List<Use> uses{ set; get; }
        public  List<Declaration> declarations{ set; get; }
        public List<Expression> invariants{ set; get; }
    }

    public class Routine: Declaration {
        public Identifier alias{ set; get; }
        public string type{ set; get; }
        public int pureSafe{ set; get; }
        public bool isPure{ set; get; }
        public bool isSafe{ set; get; }
        public bool isAbstract{ set; get; }
        public bool isForeign{ set; get; }
        public bool isOverride{ set; get; }
        public bool requireElse{ set; get; }
        public bool ensureThen{ set; get; }
        public List<Declaration> parameters{ set; get; }
        public Body body{ set; get; }
        public List<Expression> preConditions{ set; get; }
        public List<Expression> postConditions{ set; get; }
    }

    public class Initializer : Routine {
        
    }

    public class Expression: Structure {
        public int type{ set; get; }
    }

    public class Unary: Expression {
        public Expression primary{ set; get; }
        public string unaryOp{ set; get; }
    }

    public class In_Expression : Unary
    {
        public  string range{ set; get; }
    }

    public class New : Unary
    {
        public  string unit_ref{ set; get; }
    }

    public class Branch : Expression 
    {
        public Expression condition{ set; get; }
        public Expression then_part{ set; get; }
    }

    public abstract class Primary: Expression {
        
    }

    public class Reference: Primary {
        public Declaration declaration{ set; get; }
    }

    public class ReturnExpression : Primary
    {
        public Routine routine{ set; get; }
    }

    public class TupleExpression : Primary
    {
        public  List<Expression> expressions{ set; get; }
    }

    public class ThisExpression : Primary {
        public  Unit unit_ref{ set; get; }
    }

    public class Old : Primary {
        public Expression old{ set; get; }
    }

    public class Binary : Structure
    {
        public Expression left{ set; get; }
        public Expression right{ set; get; }
    }

    public class Multiplicative : Binary {
        public string multOp{ set; get; }
    }

    public class Additive : Binary
    {
        public string addOp{ set; get; }
    }

    public class Logical : Binary
    {
        public string logOp{ set; get; }
    }

    public class Relational : Binary
    {
        public string relOp{ set; get; }
    }

    public class Statement: Structure
    {

    }

    public class Check : Statement
    {
        public  List<Expression> predicates{ set; get; }
    }

    public class Body : Statement
    {
        public List<Node> body{ set; get; }
    }

    public class Break : Statement
    {
        public string label{ set; get; }
        public Statement labeled_stmt{ set; get; }
    }

    public class StatementIf : Statement
    {
        public Expression condition{ set; get; }
        public Body then_part{ set; get; }
    }

    public class Catch : Statement
    {
        public Variable catch_var{ set; get; }
        public Unit unit_ref{ set; get; }
        public Body body{ set; get; }
    }

    public class TryStatement : Statement 
    {
        public  List<Node> body{ set; get; }
        public List<Catch> handlers{ set; get; }
        public List<Node> else_part{ set; get; }
    }


    public class Generator {
        public dynamic nodes;

        public Generator(String path) {
            // JsonObjectAttribute jsonObject = new JsonConverter
            // var jsonTree = JsonConvert.DeserializeObject()
            using (StreamReader sr = new StreamReader(path))
            {
                String IR = sr.ReadToEnd();

                this.nodes = JsonConvert.DeserializeObject(IR);
            }

        }

        public void dynamicDeserialize() {
            foreach (var element in this.nodes.data)
            {
                Node node = deserializeNode(element);
            }
        }

        public Node deserializeNode(dynamic element)
        {
            Contract.Ensures(Contract.Result<Node>() != null);
            dynamic node = element.node;
            dynamic structure = element.structure;
            Node newNode = new Node() {
                id = Int32.Parse(element.id),
                parent = Int32.Parse(element.parent),
                node = new String(node)
            };
            Structure nodeStructure = null;
            switch (node)
            {   
                case "COMPILATION": 
                    {
                        nodeStructure = initializeRoutine(structure);
                        break;
                    }
                case "UNIT":
                    {
                        nodeStructure = initializeUnit(structure);
                        break;
                    };
                case "ROUTINE":
                    {
                        nodeStructure = initializeRoutine(structure);
                        break;  
                    };
                case "VARIABLE":
                    {
                        nodeStructure = initializeVariable(structure);
                    }

            }
            newNode.structure = nodeStructure;
            return newNode;
        }

        private Structure initializeVariable(dynamic structure)
        {
            Contract.Ensures(Contract.Result<Structure>() != null);
            dynamic modifiers = structure.modifiers;
            return new Variable()
            {

            };
        }

        private Structure initializeRoutine(dynamic structure)
        {
            Contract.Ensures(Contract.Result<Structure>() != null);
            dynamic modifiers = structure.modifiers;
            return new Routine()
            {
                name = parseIdentifier(structure.name),
                isHidden = Boolean.Parse(structure.isHidden),
                isFinal = Boolean.Parse(structure.isFinal),
                alias = parseIdentifier(structure.identifier),
                type = new string(structure.type),
                pureSafe = Boolean.Parse(modifiers.isPure),
                isSafe = Boolean.Parse(modifiers.isSafe),
                isAbstract = Boolean.Parse(modifiers.isAbstract),
                isForeign = Boolean.Parse(modifiers.isForeign),
                isOverride = Boolean.Parse(modifiers.isOverride),
                requireElse = Boolean.Parse(modifiers.requireElse),
                ensureThen = Boolean.Parse(modifiers.ensureThen),
                parameters = parseDeclarations(structure.parameters),
                body = parseBody(structure.body),
                preConditions = parseExpressions(structure.preConditions),
                postConditions = parseExpressions(structure.postConditions)
            };
        }

        private Structure initializeUnit(dynamic structure)
        {
            Contract.Ensures(Contract.Result<Structure>() != null);
            dynamic modifiers = structure.modifiers;

            return new Unit()
            {
                name = parseIdentifier(structure.name),
                isHidden = Boolean.Parse(modifiers.isHidden),
                isFinal = Boolean.Parse(modifiers.isFinal),
                isReference = Boolean.Parse(modifiers.isReference),
                isAbstract = Boolean.Parse(modifiers.isAbstract),
                isConcurrent = Boolean.Parse(modifiers.isConcurrent),
                alias = parseIdentifier(structure.identifier),
                parent_units = parseBaseUnits(structure.inherits),
                uses = parseUses(structure.uses),
                declarations = parseDeclarations(structure.declarations),
                invariants = parseExpressions(structure.invariants)
            };
        }

        private Body parseBody(dynamic body)
        {
            throw new NotImplementedException();
        }

        private List<Expression> parseExpressions(dynamic invariants)
        {
            throw new NotImplementedException();
        }

        private List<Declaration> parseDeclarations(dynamic declarations)
        {
            throw new NotImplementedException();
        }

        private List<Use> parseUses(dynamic uses)
        {
            throw new NotImplementedException();
        }

        public List<Unit> parseBaseUnits(dynamic inherits) 
        {
            return new List<Unit>();
        }

        public Identifier parseIdentifier(dynamic identifier)
        {
            return new Identifier() { data = new String(identifier.data) };
        }

        //public virtual Structure generate() 
        //{

        //    return new Structure();
        //}


        public void build() {
            string moduleName = "test.exe";

            string path = "";
            try {
                AssemblyName name = new AssemblyName (System.IO.Path.GetFileNameWithoutExtension (moduleName));
                // AssemblyBuilder asmb = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
                AssemblyBuilder asmb = AssemblyBuilder.DefineDynamicAssembly (new AssemblyName (Guid.NewGuid ().ToString ()), AssemblyBuilderAccess.RunAndCollect);
                ModuleBuilder modb = asmb.DefineDynamicModule (moduleName);
                // AssemblyBuilder asmbincstance = asmb.CreateInstance("mymodule");

                TypeBuilder typeBuilder = modb.DefineType ("Foo");

                MethodBuilder methb = typeBuilder.DefineMethod ("Main",
                    MethodAttributes.Static, typeof (void),
                    Type.EmptyTypes);

                ILGenerator il = methb.GetILGenerator ();

                //LocalBuilder aObject = il.DeclareLocal(aType);
                //il.Emit(OpCodes.Newobj, aType.GetConstructor(new Type[0]));
                //il.Emit(OpCodes.Stloc, aObject);

                // Init of _base_L field attribute
                //il.Emit(OpCodes.Ldloc, aObject);
                //il.Emit(OpCodes.Newobj, lType);
                //il.Emit(OpCodes.Stfld, aTypeBaseL);

                // Call of foo function from base class L
                //il.Emit(OpCodes.Ldfld, aTypeBaseL);
                //il.Emit(OpCodes.Call, lTypeMethodFoo);

                Assembly slib = Assembly.LoadFrom ("SLib.dll");
                Type consoleType = slib.GetType ("SLib.IO.Console");
                //BindingFlags flags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static;
                MethodInfo consolePut = consoleType.GetMethod ("put", new [] { typeof (string) });
                MethodInfo readKey = consoleType.GetMethod ("read_key", new Type[0]);
                il.Emit (OpCodes.Ldstr, "HelloWorld!\n");
                il.Emit (OpCodes.Call, consolePut);
                Type file = slib.GetType ("SLib.IO.File");

                ConstructorInfo fileConstructor = file.GetConstructor (new Type[] { typeof (string), typeof (bool) });
                LocalBuilder fileBuilder = il.DeclareLocal (file);
                string fileName = @"test.txt";

                il.Emit (OpCodes.Ldstr, "File will be created after key press\n");
                il.Emit (OpCodes.Call, consolePut);
                il.Emit (OpCodes.Call, readKey);
                il.Emit (OpCodes.Ldstr, fileName);
                il.Emit (OpCodes.Ldc_I4_1);
                il.Emit (OpCodes.Newobj, fileConstructor);
                il.Emit (OpCodes.Stloc, fileBuilder);
                il.Emit (OpCodes.Ldloc, fileBuilder);

                il.Emit (OpCodes.Ldstr, "File will be removed after key press\n");
                il.Emit (OpCodes.Call, consolePut);

                il.Emit (OpCodes.Call, readKey);

                MethodInfo remove = file.GetMethod ("remove", new Type[0]);
                il.Emit (OpCodes.Callvirt, remove);

                il.Emit (OpCodes.Ldstr, "File removed\n");
                il.Emit (OpCodes.Call, consolePut);
                il.Emit (OpCodes.Call, readKey);

                //TypeBuilder lBuilder = modb.DefineType("L");
                //FieldBuilder iBuilder = lBuilder.DefineField("i", typeof(int), FieldAttributes.Public);
                //MethodBuilder newMethod = lBuilder.DefineMethod();

                //If(4 + 5) > (9 - 4 / 2) then
                // StandartIO.Print(“True\n”)
                //Else
                // StandartIO.Print(“False\n”)
                //End
                //StandartIO.Print(“end if\n”)

                //il.Emit(OpCodes.Ldstr, "(4+5) > (9-4/2) ?\n");
                //il.Emit(OpCodes.Call, consolePut);
                //il.Emit(OpCodes.Call, readKey);

                //Label @else = il.DefineLabel();
                //il.Emit(OpCodes.Ldc_I4, 4);
                //il.Emit(OpCodes.Ldc_I4, 5);
                //il.Emit(OpCodes.Add);

                //il.Emit(OpCodes.Ldc_I4, 9);
                //il.Emit(OpCodes.Ldc_I4, 4);
                //il.Emit(OpCodes.Ldc_I4, 2);
                //il.Emit(OpCodes.Div);
                //il.Emit(OpCodes.Sub);

                //il.Emit(OpCodes.Ble, @else);

                //il.Emit(OpCodes.Ldstr, "True\n");
                //il.Emit(OpCodes.Call, consolePut);
                //Label @end = il.DefineLabel();
                //il.Emit(OpCodes.Br, @end);

                //il.MarkLabel(@else);
                //il.Emit(OpCodes.Ldstr, "False\n");
                //il.Emit(OpCodes.Call, consolePut);
                //il.MarkLabel(@end);
                //il.Emit(OpCodes.Call, readKey);
                //il.Emit(OpCodes.Ldstr, "end if\n");
                //il.Emit(OpCodes.Call, consolePut);
                //il.Emit(OpCodes.Call, readKey);

                // x is Integer(5)

                //LocalBuilder x = il.DeclareLocal(typeof(int));
                //il.Emit(OpCodes.Ldc_I4_5);
                //il.Emit(OpCodes.Stloc, x);

                // while x > 1

                //Label @loop = il.DefineLabel();
                //Label @end = il.DefineLabel();
                //il.MarkLabel(@loop);
                //il.Emit(OpCodes.Ldc_I4, x);
                //il.Emit(OpCodes.Ldc_I4_1);
                //il.Emit(OpCodes.Sub);
                //il.Emit(OpCodes.Stloc, x);
                //il.Emit(OpCodes.Ldc_I4_1);
                //il.Emit(OpCodes.Ble, @end);

                // loop

                //il.Emit(OpCodes.Ldstr, "loop \n");
                //il.Emit(OpCodes.Call, consolePut);
                //il.Emit(OpCodes.Br, @loop);

                // end
                //il.MarkLabel(@end);
                //il.Emit(OpCodes.Ldstr, "end loop\n");

                //il.Emit(OpCodes.Call, consolePut);
                //il.Emit(OpCodes.Call, readKey);
                //il.Emit(OpCodes.Pop);

                il.Emit (OpCodes.Ret);
                typeBuilder.CreateType ();

                // unit L
                TypeBuilder lType = modb.DefineType ("L");

                // define constructor
                ConstructorBuilder lTypeCtor = lType.DefineConstructor (
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.Standard,
                    new Type[0]);
                ILGenerator IllType = lTypeCtor.GetILGenerator ();
                IllType.Emit (OpCodes.Ldarg_0);
                ConstructorInfo ctor = typeof (Object).GetConstructor (new Type[0]);
                IllType.Emit (OpCodes.Call, ctor);
                IllType.Emit (OpCodes.Ret);

                // type creation 
                lType.CreateType ();

                TypeBuilder aType = modb.DefineType ("A");
                FieldBuilder aTypeBaseField = aType.DefineField ("__base_L", lType, FieldAttributes.Public);
                ConstructorBuilder aTypeCtor = aType.DefineConstructor (
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.Standard,
                    new Type[0]);
                ILGenerator IlaType = aTypeCtor.GetILGenerator ();
                IlaType.Emit (OpCodes.Ldarg_0);
                ConstructorInfo aCtor = typeof (Object).GetConstructor (new Type[0]);
                IlaType.Emit (OpCodes.Call, aCtor);
                IlaType.Emit (OpCodes.Newobj, lType);
                IlaType.Emit (OpCodes.Stloc, aTypeBaseField);
                IlaType.Emit (OpCodes.Ret);

                aType.CreateType ();

                TypeAttributes valueAttrTypes = TypeAttributes.AnsiClass | TypeAttributes.SequentialLayout | TypeAttributes.Sealed | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;
                TypeBuilder valueType = modb.DefineType ("ValueData", valueAttrTypes, typeof (ValueType));
                //ConstructorInfo customGenCtor = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute).GetConstructor(new Type[0]);
                //valueType.SetCustomAttribute(customGenCtor, new byte[] { 01, 00, 00, 00 });
                FieldBuilder xValueType = valueType.DefineField ("x", typeof (int), FieldAttributes.Public);
                valueType.CreateType ();

                modb.CreateGlobalFunctions ();

                // asmb.Save("my_assembly");
                // asmb.EntryPoint = methb;
            } catch (Exception e) {
                Console.WriteLine (e.StackTrace);
            }

            //Loat();

        }
    }





}
