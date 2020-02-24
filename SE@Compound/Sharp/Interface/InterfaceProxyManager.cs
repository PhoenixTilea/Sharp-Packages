// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// Provides on-demand proxy creation for loose object to interface bindings
    /// </summary>
    public static class InterfaceProxyManager
    {
        private readonly static Dictionary<UInt32, Type> typeCache;
        private readonly static Type interfaceProxyType;
        private readonly static FieldInfo hostObject;

        static InterfaceProxyManager()
        {
            typeCache = new Dictionary<UInt32, Type>();

            interfaceProxyType = typeof(DynamicInterfaceProxy);
            hostObject = interfaceProxyType.GetField("host", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static void CreateConstructor(TypeBuilder type)
        {
            ConstructorInfo baseCtor = interfaceProxyType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(object) }, null);
            ConstructorBuilder ctor = type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { typeof(object) });
            ILGenerator generator = ctor.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);

            generator.Emit(OpCodes.Call, baseCtor);
            generator.Emit(OpCodes.Ret);
        }
        private static void CreateFunctionBindings(TypeBuilder type, Type hostType, Type @interface)
        {
            bool isExplicitBindingGlobal = !@interface.HasAttribute<ImplicitBindingAttribute>();

            BindingFlags flags = (BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo minf in @interface.GetMethods(flags))
            {
                MethodInfo methodBinding = null;
                ParameterInfo[] parameters = minf.GetParameters();

                bool isExplicitBindingLocal = (isExplicitBindingGlobal && !minf.HasAttribute<ImplicitBindingAttribute>()) | minf.IsSpecialName;
                foreach (MethodInfo target in hostType.GetMethods(flags))
                    if (methodBinding == null || target.Name == minf.Name)
                    {
                        ParameterInfo[] args = target.GetParameters();
                        if (args.Length != parameters.Length || (args.Length > 0 && Array.Equals(args, parameters)))
                            continue;

                        if (target.ReturnType == minf.ReturnType)
                        {
                            bool isExactMatch = (target.Name == minf.Name);
                            if (!isExplicitBindingLocal || (isExplicitBindingLocal && isExactMatch))
                                methodBinding = target;

                            if (isExactMatch)
                                break;
                        }
                    }

                if (methodBinding == null)
                    throw new InvalidCastException(string.Format("Type {0} doesn't implement {1} of interface {2}", hostType.FullName, minf.Name, @interface.FullName));

                Type[] parameterTypes = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                    parameterTypes[i] = parameters[i].ParameterType;

                MethodBuilder method = type.DefineMethod(minf.Name, MethodAttributes.Public | MethodAttributes.Virtual, minf.ReturnType, parameterTypes);
                ILGenerator generator = method.GetILGenerator();

                generator.Emit(OpCodes.Ldarg_0);
                if (hostObject.DeclaringType.IsValueType)
                    generator.Emit(OpCodes.Unbox, hostObject.DeclaringType);
                else
                    generator.Emit(OpCodes.Castclass, hostObject.DeclaringType);
                generator.Emit(OpCodes.Ldfld, hostObject);

                if (parameters.Length > 0)
                    generator.Emit(OpCodes.Ldarg_1);

                if (parameters.Length > 1)
                    generator.Emit(OpCodes.Ldarg_2);

                if (parameters.Length > 2)
                    generator.Emit(OpCodes.Ldarg_3);

                for (int i = 3; i < parameters.Length; i++)
                    generator.Emit(OpCodes.Ldarg, i + 1);

                generator.EmitCall(OpCodes.Call, methodBinding, null);
                generator.Emit(OpCodes.Ret);

                type.DefineMethodOverride(method, minf);
            }
        }

        private static Type CreateInstance(Type @interface, Type hostType)
        {
            if (!@interface.IsInterface)
                throw new ArgumentOutOfRangeException("T");

            AssemblyBuilder asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicProxies"), AssemblyBuilderAccess.Run);
            TypeBuilder type = asm.DefineDynamicModule("DynamicProxies").DefineType(string.Format("{0}_{1}.InterfaceProxy", hostType.Name, @interface.Name), TypeAttributes.Public | TypeAttributes.Class, interfaceProxyType);            
            type.AddInterfaceImplementation(@interface);

            CreateConstructor(type);
            CreateFunctionBindings(type, hostType, @interface);

            return type.CreateType();
        }

        /// <summary>
        /// Returns an instance of an object to interface binding proxy. If the proxy doesn't
        /// exists yet, it will be created
        /// </summary>
        /// <typeparam name="interface">The type of interface to be bound to the host</typeparam>
        /// <param name="host">An object instance the interface should lead to</param>
        /// <returns>The instance of the given interface type or zero</returns>
        public static object GetBindingProxy(Type @interface, object host)
        {
            UInt32 id = host.GetType().Name.Fnv32();
            id = @interface.Name.Fnv32(id);

            Type type;
            lock (typeCache)
            {
                if (!typeCache.TryGetValue(id, out type))
                {
                    type = CreateInstance(@interface, host.GetType());
                    typeCache.Add(id, type);
                }
            }

            return Activator.CreateInstance(type, host);
        }
        /// <summary>
        /// Returns an instance of an object to interface binding proxy. If the proxy doesn't
        /// exists yet, it will be created
        /// </summary>
        /// <typeparam name="T">The type of interface to be bound to the host</typeparam>
        /// <param name="host">An object instance the interface should lead to</param>
        /// <returns>The instance of the given interface type or zero</returns>
        public static T GetBindingProxy<T>(object host) where T : class
        {
            return (T)GetBindingProxy(typeof(T), host);
        }
    }
}
