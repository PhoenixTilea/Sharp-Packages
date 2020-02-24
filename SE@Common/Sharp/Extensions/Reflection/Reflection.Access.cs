// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SE
{
    public static partial class ReflectionExtension
    {
        /// <summary>
        /// Creates a function delegate accessor to a field
        /// </summary>
        /// <returns>A delegate to a field</returns>
        public static Delegate CreateGetter(this FieldInfo field)
        {
            DynamicMethod func = new DynamicMethod(string.Format("{0}.Get{1}", field.DeclaringType.FullName, field.Name), typeof(object), new Type[] { typeof(object) }, field.DeclaringType, true);
            ILGenerator generator = func.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            if (field.DeclaringType.IsValueType)
                generator.Emit(OpCodes.Unbox, field.DeclaringType);
            else
                generator.Emit(OpCodes.Castclass, field.DeclaringType);
            generator.Emit(OpCodes.Ldfld, field);

            if (field.FieldType.IsValueType)
                generator.Emit(OpCodes.Box, field.FieldType);

            generator.Emit(OpCodes.Ret);
            return (Func<object, object>)func.CreateDelegate(typeof(Func<object, object>));
        }
        /// <summary>
        /// Creates a function delegate accessor to a property
        /// </summary>
        /// <returns>A delegate to a property</returns>
        public static Delegate CreateGetter(this PropertyInfo property)
        {
            DynamicMethod func = new DynamicMethod(string.Format("{0}.Get{1}", property.DeclaringType.FullName, property.Name), typeof(object), new Type[] { typeof(object) }, property.DeclaringType);
            ILGenerator generator = func.GetILGenerator();

            generator.DeclareLocal(typeof(object));
            generator.Emit(OpCodes.Ldarg_0);
            if (property.DeclaringType.IsValueType)
                generator.Emit(OpCodes.Unbox, property.DeclaringType);
            else
                generator.Emit(OpCodes.Castclass, property.DeclaringType);

            if (property.DeclaringType.IsValueType)
                generator.EmitCall(OpCodes.Call, property.GetGetMethod(true), null);
            else
                generator.EmitCall(OpCodes.Callvirt, property.GetGetMethod(true), null);

            if (!property.PropertyType.IsClass)
                generator.Emit(OpCodes.Box, property.PropertyType);

            generator.Emit(OpCodes.Ret);
            return func.CreateDelegate(typeof(Func<object, object>));
        }

        /// <summary>
        /// Creates a function delegate accessor to a field
        /// </summary>
        /// <returns>A delegate to a field</returns>
        public static Delegate CreateSetter(this FieldInfo field)
        {
            DynamicMethod dynamicMethod = new DynamicMethod(string.Format("{0}.Set{1}", field.DeclaringType.FullName, field.Name), typeof(void), new Type[] { typeof(object), typeof(object) }, field.DeclaringType, true);
            ILGenerator generator = dynamicMethod.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            if (field.DeclaringType.IsValueType)
                generator.Emit(OpCodes.Unbox, field.DeclaringType);
            else
                generator.Emit(OpCodes.Castclass, field.DeclaringType);

            generator.Emit(OpCodes.Ldarg_1);
            if (field.FieldType.IsValueType)
                generator.Emit(OpCodes.Unbox_Any, field.FieldType);

            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);

            return (Action<object, object>)dynamicMethod.CreateDelegate(typeof(Action<object, object>));
        }
        /// <summary>
        /// Creates a function delegate accessor to a property
        /// </summary>
        /// <returns>A delegate to a property</returns>
        public static Delegate CreateSetter(this PropertyInfo property)
        {
            DynamicMethod func = new DynamicMethod(string.Format("{0}.Set{1}", property.DeclaringType.FullName, property.Name), typeof(void), new Type[] { typeof(object), typeof(object) }, property.DeclaringType, true);
            ILGenerator generator = func.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0);
            if (property.DeclaringType.IsValueType)
                generator.Emit(OpCodes.Unbox, property.DeclaringType);
            else
                generator.Emit(OpCodes.Castclass, property.DeclaringType);

            generator.Emit(OpCodes.Ldarg_1);
            if (property.PropertyType.IsClass)
                generator.Emit(OpCodes.Castclass, property.PropertyType);
            else
                generator.Emit(OpCodes.Unbox_Any, property.PropertyType);

            generator.EmitCall(OpCodes.Callvirt, property.GetSetMethod(true), null);
            generator.Emit(OpCodes.Ret);

            return func.CreateDelegate(typeof(Action<object, object>));
        }
    }
}
