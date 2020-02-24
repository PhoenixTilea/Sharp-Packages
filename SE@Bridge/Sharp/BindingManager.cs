// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using SE;

namespace SE.Reflection.Bridge
{
    /// <summary>
    /// Provides cross-language runtime native binding access
    /// </summary>
    public static class BindingManager
    {
        private readonly static Dictionary<Type, object> instanceCache;

        static BindingManager()
        {
            instanceCache = new Dictionary<Type, object>();
        }

        private static T CreateInstance<T>(string assemblyName = null) where T : class
        {
            Type @interface = typeof(T);

            if (!@interface.IsInterface) throw new ArgumentOutOfRangeException("T");
            if (string.IsNullOrWhiteSpace(assemblyName))
            {
                foreach (NativeBindingAttribute config in @interface.GetAttributes<NativeBindingAttribute>())
                {
                    switch (config.TargetPlatform)
                    {
                        case PlatformSwitch.x86: if (IntPtr.Size == 4 && !string.IsNullOrWhiteSpace(config.AssemblyName)) assemblyName = config.AssemblyName; break;
                        case PlatformSwitch.x64: if (IntPtr.Size == 8 && !string.IsNullOrWhiteSpace(config.AssemblyName)) assemblyName = config.AssemblyName; break;
                        case PlatformSwitch.Any:
                        default: if (!string.IsNullOrWhiteSpace(config.AssemblyName)) assemblyName = config.AssemblyName; break;
                    }

                    if (!string.IsNullOrWhiteSpace(assemblyName))
                        break;
                }
                if (string.IsNullOrWhiteSpace(assemblyName)) assemblyName = @interface.Name;
            }

            AssemblyBuilder asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicInteropt"), AssemblyBuilderAccess.Run);
            TypeBuilder type = asm.DefineDynamicModule("DynamicInteropt").DefineType(string.Format("{0}.NativeProxy", @interface.Name));
            type.AddInterfaceImplementation(@interface);

            foreach (MethodInfo minf in @interface.GetMethods<MethodBindingAttribute>(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
            {
                MethodBindingAttribute binding = minf.GetAttribute<MethodBindingAttribute>();

                if (binding.EntryPoint == null)
                    binding.EntryPoint = minf.Name;

                ParameterInfo[] parameters = minf.GetParameters();
                Type[] parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    parameterTypes[i] = parameters[i].ParameterType;

                MethodBuilder pinvoke = type.DefineMethod(minf.Name, MethodAttributes.Static, minf.ReturnType, parameterTypes);
                Type dllImportType = typeof(System.Runtime.InteropServices.DllImportAttribute);
                pinvoke.SetCustomAttribute(new CustomAttributeBuilder(dllImportType.GetConstructor(new Type[] { typeof(string) }),
                new object[] 
                { 
                    assemblyName 
                },
                new FieldInfo[] 
                {
                    dllImportType.GetField("EntryPoint"),
                    dllImportType.GetField("ExactSpelling"),
                    dllImportType.GetField("PreserveSig"),
                    dllImportType.GetField("SetLastError"),
                    dllImportType.GetField("CallingConvention"),
                    dllImportType.GetField("CharSet"),
                    dllImportType.GetField("BestFitMapping"),
                    dllImportType.GetField("ThrowOnUnmappableChar")
                },
                new object[] 
                {
                    binding.EntryPoint,
                    binding.ExactSpelling,
                    binding.PreserveSig,
                    binding.SetLastError,
                    binding.CallingConvention,
                    binding.CharSet,
                    binding.BestFitMapping,
                    binding.ThrowOnUnmappableChar
                }));

                ParameterBuilder[] pinvokeParameters = new ParameterBuilder[parameters.Length];
                object[] marshalAsAttributes;
                FieldInfo[] marshalFields;
                object[] marshalFieldsValues;
                for (int i = -1; i < parameters.Length; i++)
                {
                    ParameterBuilder parameter;
                    if (i == -1)
                    {
                        parameter = pinvoke.DefineParameter(0, minf.ReturnParameter.Attributes, null);
                        marshalAsAttributes = minf.ReturnParameter.GetCustomAttributes(typeof(MarshalAsAttribute), false);
                    }
                    else
                    {
                        parameter = pinvoke.DefineParameter(i + 1, parameters[i].Attributes, null);
                        marshalAsAttributes = parameters[i].GetCustomAttributes(typeof(MarshalAsAttribute), false);
                    }
                    if (marshalAsAttributes.Length > 0)
                    {
                        MarshalAsAttribute marshalAs = (MarshalAsAttribute)marshalAsAttributes[0];
                        marshalFields = typeof(MarshalAsAttribute).GetFields();
                        marshalFieldsValues = new object[marshalFields.Length];

                        for (int j = 0; j < marshalFields.Length; j++)
                            marshalFieldsValues[j] = marshalFields[j].GetValue(marshalAs);

                        parameter.SetCustomAttribute(new CustomAttributeBuilder(typeof(MarshalAsAttribute).GetConstructor(new[] { typeof(UnmanagedType) }), new object[] { marshalAs.Value }, marshalFields, marshalFieldsValues));
                    }
                }

                MethodBuilder method = type.DefineMethod(minf.Name, MethodAttributes.Public | MethodAttributes.Virtual, minf.ReturnType, parameterTypes);
                ILGenerator gen = method.GetILGenerator();

                if (parameters.Length > 0)
                    gen.Emit(OpCodes.Ldarg_1);

                if (parameters.Length > 1)
                    gen.Emit(OpCodes.Ldarg_2);

                if (parameters.Length > 2)
                    gen.Emit(OpCodes.Ldarg_3);

                for (int i = 3; i < parameters.Length; i++)
                    gen.Emit(OpCodes.Ldarg, i + 1);

                gen.EmitCall(OpCodes.Call, pinvoke, null);
                gen.Emit(OpCodes.Ret);

                type.DefineMethodOverride(method, minf);
            }

            return (T)Activator.CreateInstance(type.CreateType());
        }

        /// <summary>
        /// Returns an instance of an interface to a static native API binding. If the interface doesn't
        /// exists yet, it will be created
        /// </summary>
        /// <typeparam name="T">The type of class to be instanciated</typeparam>
        /// <param name="assemblyName">An optional name and path to certain library for creating the native bindings</param>
        /// <returns>The instance of the given interface type or zero</returns>
        public static T GetBindingProxy<T>(string assemblyName = null) where T : class
        {
            object instance;
            lock (instanceCache)
            {
                if (!instanceCache.TryGetValue(typeof(T), out instance))
                {
                    instance = CreateInstance<T>(assemblyName);
                    instanceCache.Add(typeof(T), instance);
                }
            }
            return (T)instance;
        }
    }
}
