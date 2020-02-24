// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Dynamic;
using SE;

namespace SE.Reflection.Compound
{
    /// <summary>
    /// A dynamic binder object handling DLR requests based on its policy
    /// </summary>
    public class CompoundBinder : DynamicMetaObject
    {
        delegate bool TryGetPropertyByNameDelegate(InstanceId objectId, string name, out PropertyInstance result);
        delegate bool TryGetPropertyByIdDelegate(InstanceId propertyId, out PropertyInstance result);

        delegate bool TryGetMethodByNameDelegate(InstanceId objectId, string name, out Delegate result);
        delegate bool TryGetMethodByIdDelegate(InstanceId methodId, out Delegate result);

        private readonly static MethodInfo createIdFromString;

        private readonly static MethodInfo addProperty;
        private readonly static MethodInfo tryGetPropertyByName;
        private readonly static MethodInfo tryGetPropertyById;
        private readonly static MethodInfo removeProperty;

        private readonly static MethodInfo convert;

        private readonly static MethodInfo addMethod;
        private readonly static MethodInfo tryGetMethodByName;
        private readonly static MethodInfo tryGetMethodById;
        private readonly static MethodInfo removeMethod;

        private readonly static MethodInfo invoke;
        private readonly static MethodInfo createInterfaceProxy;

        private readonly static PropertyInfo id;
        private readonly static PropertyInfo componentId;

        private readonly static PropertyInfo propertyType;
        private readonly static PropertyInfo propertyValue;

        private readonly static PropertyInfo method;

        CompoundPolicy policy;

        static CompoundBinder()
        {
            createIdFromString = ((Func<string, UInt32, UInt32>)Fnv.Fnv32).Method;

            addProperty = ((Func<InstanceId, string, Type, bool>)CompoundPropertyManager.AddProperty).Method;
            tryGetPropertyByName = ((TryGetPropertyByNameDelegate)CompoundPropertyManager.TryGetProperty).Method;
            tryGetPropertyById = ((TryGetPropertyByIdDelegate)CompoundPropertyManager.TryGetProperty).Method;
            removeProperty = ((Func<InstanceId, string, bool>)CompoundPropertyManager.RemoveProperty).Method;

            convert = ((Func<object, Type, object>)Convert.ChangeType).Method;

            addMethod = ((Func<InstanceId, string, Delegate, bool>)CompoundMethodManager.AddMethod).Method;
            tryGetMethodByName = ((TryGetMethodByNameDelegate)CompoundMethodManager.TryGetMethod).Method;
            tryGetMethodById = ((TryGetMethodByIdDelegate)CompoundMethodManager.TryGetMethod).Method;
            removeMethod = ((Func<InstanceId, string, bool>)CompoundMethodManager.RemoveMethod).Method;

            invoke = typeof(MethodInfo).GetMethod("Invoke", new Type[]{ typeof(object), typeof(object[]) });
            createInterfaceProxy = ((Func<Type, object, object>)InterfaceProxyManager.GetBindingProxy).Method;

            id = typeof(ICompoundObject).GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);
            componentId = typeof(InstanceId).GetProperty("ComponentId", BindingFlags.Instance | BindingFlags.Public);

            propertyType = typeof(PropertyInstance).GetProperty("PropertyType", BindingFlags.Public | BindingFlags.Instance);
            propertyValue = typeof(PropertyInstance).GetProperty("Value", BindingFlags.Public | BindingFlags.Instance);

            method = typeof(Delegate).GetProperty("Method", BindingFlags.Public | BindingFlags.Instance);
        }
        /// <summary>
        /// Creates a new instance of this binder object
        /// </summary>
        public CompoundBinder(object value, CompoundPolicy policy, Expression expression)
            : base(expression, BindingRestrictions.Empty, value)
        {
            this.policy = policy;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            if (policy.Flags.HasFlag(CompoundPolicyFlags.AllowDynamicProperties))
            {
                DynamicMetaObject getDefault = binder.FallbackGetMember(this);
                ParameterExpression id = Expression.Variable(typeof(InstanceId), "id");
                ParameterExpression tmp = Expression.Variable(typeof(PropertyInstance), "property");

                Expression propertyDefault; if (policy.Flags.HasFlag(CompoundPolicyFlags.AllowPropertyDefaults))
                    propertyDefault = Expression.Default(binder.ReturnType);
                else
                    propertyDefault = getDefault.Expression;

                Expression expression = Expression.Block
                (
                    new ParameterExpression[] { id, tmp },
                    Expression.Assign
                    (
                        id,
                        CreatePropertyIdExpression(binder)
                    ),
                    Expression.Condition
                    (
                        Expression.Call
                        (
                            tryGetPropertyById,
                            id,
                            tmp
                        ),
                        Expression.Property
                        (
                            tmp,
                            propertyValue
                        ),
                        propertyDefault,
                        binder.ReturnType
                    )
                );

                BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
                return binder.FallbackGetMember(this, dynamicSuggestion);
            }
            else return binder.FallbackGetMember(this);
        }
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            if (policy.Flags.HasFlag(CompoundPolicyFlags.AllowDynamicProperties))
            {
                DynamicMetaObject getDefault = binder.FallbackSetMember(this, value);
                ParameterExpression id = Expression.Variable(typeof(InstanceId), "id");
                ParameterExpression tmp = Expression.Variable(typeof(PropertyInstance), "property");
                Expression expression = Expression.Block
                (
                    new ParameterExpression[] { id, tmp },
                    Expression.Assign
                    (
                        id,
                        CreatePropertyIdExpression(binder)
                    ),
                    Expression.Condition
                    (
                        Expression.Call
                        (
                            tryGetPropertyById,
                            id,
                            tmp
                        ),
                        Expression.Assign
                        (
                            Expression.Property
                            (
                                tmp,
                                propertyValue
                            ),
                            Expression.Call
                            (
                                convert,
                                Expression.Convert
                                (
                                    value.Expression,
                                    typeof(object)
                                ),
                                Expression.Property
                                (
                                    tmp,
                                    propertyType
                                )
                            )
                        ),
                        getDefault.Expression,
                        binder.ReturnType
                    )
                );

                BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
                return binder.FallbackSetMember(this, value, dynamicSuggestion);
            }
            else return binder.FallbackSetMember(this, value);
        }

        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            Expression e = this.Expression;
            DynamicMetaObject getDefault = binder.FallbackInvokeMember(this, args);
            ParameterExpression tmp = Expression.Variable(typeof(InstanceId), "id");
            Expression expression = Expression.Block
            (
                new ParameterExpression[]{ tmp },
                Expression.Assign
                (
                    tmp,
                    CreatePropertyIdExpression(binder)
                ),
                Expression.Property
                (
                    tmp,
                    componentId
                ),
                CreateInvokeMemberFunctionExpression(binder, args, getDefault.Expression, tmp)
            );

            BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
            DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
            return binder.FallbackInvokeMember(this, args, dynamicSuggestion);
        }

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            DynamicMetaObject getDefault = binder.FallbackConvert(this);
            ParameterExpression tmp = Expression.Variable(typeof(object), "result");
            Expression expression = Expression.Block
            (
                binder.ReturnType,
                new ParameterExpression[] { tmp },
                CreateInterfaceProxyExpression(binder, tmp),
                Expression.Condition
                (
                    Expression.Equal
                    (
                        tmp,
                        Expression.Constant(null)
                    ),
                    getDefault.Expression,
                    Expression.Convert
                    (
                        tmp,
                        binder.ReturnType
                    )
                )
            );

            BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
            return new DynamicMetaObject(expression, restrictions);
        }

        Expression CreatePropertyIdExpression(DynamicMetaObjectBinder binder)
        {
            return Expression.MakeBinary
            (
                ExpressionType.Or,
                Expression.Property
                (
                    Expression.Convert
                    (
                        Expression.Convert(Expression, LimitType),
                        typeof(ICompoundObject)
                    ),
                    id
                ),
                Expression.Call
                (
                    createIdFromString,
                    Expression.Property
                    (
                        Expression.Constant(binder),
                        binder.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public)
                    ),
                    Expression.Constant(Fnv.FnvOffsetBias)
                )
            );
        }

        Expression CreateInvokeMemberFunctionExpression(InvokeMemberBinder binder, DynamicMetaObject[] args, Expression defaultAction, ParameterExpression variable)
        {
            switch(binder.Name)
            {
                /**
                 Properties
                */
                case "AddProperty": 
                    {
                        if (policy.Flags.HasFlag(CompoundPolicyFlags.AllowDynamicProperties))
                            return Expression.Convert
                            (
                                Expression.Call
                                (
                                    addProperty,
                                    args.ToExpressionList(variable)
                                ),
                                typeof(object)
                            );
                        else return CreateInvokeMemberDefaultExpression(args, defaultAction, variable);
                    }
                case "TryGetProperty":
                    {
                        if (policy.Flags.HasFlag(CompoundPolicyFlags.AllowDynamicProperties))
                            return Expression.Convert
                            (
                                Expression.Call
                                (
                                    tryGetPropertyByName,
                                    args.ToExpressionList(variable)
                                ),
                                typeof(object)
                            );
                        else return CreateInvokeMemberDefaultExpression(args, defaultAction, variable);
                    }
                case "RemoveProperty": 
                    {
                        if (policy.Flags.HasFlag(CompoundPolicyFlags.AllowDynamicProperties))
                            return Expression.Convert
                            (
                                Expression.Call
                                (
                                    removeProperty,
                                    args.ToExpressionList(variable)
                                ),
                                typeof(object)
                            );
                        else return CreateInvokeMemberDefaultExpression(args, defaultAction, variable);
                    }
                /**
                 Extension Methods
                */
                case "AddMethod": if (!policy.Flags.HasFlag(CompoundPolicyFlags.AllowExtensionMethods)) return defaultAction;
                    else return Expression.Convert
                    (
                        Expression.Call
                        (
                            addMethod,
                            args.ToExpressionList(variable)
                        ),
                        typeof(object)
                    );
                case "TryGetMethod": if (!policy.Flags.HasFlag(CompoundPolicyFlags.AllowExtensionMethods)) return defaultAction;
                    else return Expression.Convert
                    (
                        Expression.Call
                        (
                            tryGetMethodByName,
                            args.ToExpressionList(variable)
                        ),
                        typeof(object)
                    );
                case "RemoveMethod": if (!policy.Flags.HasFlag(CompoundPolicyFlags.AllowExtensionMethods)) return defaultAction;
                    else return Expression.Convert
                    (
                        Expression.Call
                        (
                            removeMethod,
                            args.ToExpressionList(variable)
                        ),
                        typeof(object)
                    );
                default: return CreateInvokeMemberDefaultExpression(args, defaultAction, variable);
            }
        }

        Expression CreateInvokeMemberDefaultExpression(DynamicMetaObject[] args, Expression defaultAction, ParameterExpression variable)
        {
            if (policy.Flags.HasFlag(CompoundPolicyFlags.AllowExtensionMethods))
            {
                ParameterExpression tmp = Expression.Variable(typeof(Delegate), "tmp0");
                return Expression.Block
                (
                    new ParameterExpression[] { tmp },
                    Expression.Condition
                    (
                        Expression.Call
                        (
                            tryGetMethodById,
                            variable,
                            tmp
                        ),
                        Expression.Call
                        (
                            Expression.Property
                            (
                                tmp,
                                method
                            ),
                            invoke,
                            Expression.Constant(null),
                            Expression.NewArrayInit
                            (
                                typeof(object),
                                args.ToExpressionList<object>(Expression.Constant(Value))
                            )
                        ),
                        defaultAction
                    )
                );
            }
            else return defaultAction;
        }

        Expression CreateInterfaceProxyExpression(ConvertBinder binder, ParameterExpression variable)
        {
            if (policy.Flags.HasFlag(CompoundPolicyFlags.AllowImplicitInterfaceMapping) && binder.Type.IsInterface)
            {
                return Expression.Assign
                (
                    variable,
                    Expression.Call
                    (
                        createInterfaceProxy,
                        Expression.Constant(binder.Type),
                        Expression.Convert
                        (
                            Expression.Constant(Value),
                            typeof(object)
                        )
                    )
                );
            }
            else return Expression.Empty();
        }

        BindingRestrictions GetRestrictions()
        {
            if (Value == null && HasValue) return BindingRestrictions.GetInstanceRestriction(Expression, null);
            else return BindingRestrictions.GetTypeRestriction(Expression, LimitType);
        }
    }
}
