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
    /// A dynamic binder object that translates any class, implementing the IDynamicMetaObjectProvider
    /// interface, into a DLR compliant object
    /// </summary>
    public class DynamicBinder : DynamicMetaObject
    {
        /// <summary>
        /// Creates a new instance of this binder object
        /// </summary>
        public DynamicBinder(object value, Expression expression) 
            : base(expression, BindingRestrictions.Empty, value)
        { }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryGetMember", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] args = minfo.GetParameters();
                if (args.Length == 2 && args[0].ParameterType == typeof(GetMemberBinder) && args[1].IsOut && args[1].ParameterType == typeof(object).MakeByRefType())
                {
                    DynamicMetaObject dynamicSuggestion = CreateExpression_2(minfo, binder, binder.FallbackGetMember(this));
                    return binder.FallbackGetMember(this, dynamicSuggestion);
                }
            }
            return base.BindGetMember(binder);
        }
        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TrySetMember", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] args = minfo.GetParameters();
                if (args.Length == 2 && args[0].ParameterType == typeof(SetMemberBinder) && args[1].ParameterType == typeof(object))
                {
                    DynamicMetaObject getDefault = binder.FallbackSetMember(this, value);
                    Expression expression = Expression.Condition
                    (
                        Expression.Call
                        (
                            Expression.Constant(Value),
                            minfo,
                            Expression.Constant(binder),
                            Expression.Convert
                            (
                                value.Expression,
                                typeof(object)
                            )
                        ),
                        Expression.Default(binder.ReturnType),
                        getDefault.Expression,
                        binder.ReturnType
                    );

                    BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                    DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
                    return binder.FallbackSetMember(this, value, dynamicSuggestion);
                }
            }
            return base.BindSetMember(binder, value);
        }
        public override DynamicMetaObject BindDeleteMember(DeleteMemberBinder binder)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryDeleteMember", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] args = minfo.GetParameters();
                if (args.Length == 1 && args[0].ParameterType == typeof(DeleteMemberBinder))
                {
                    DynamicMetaObject getDefault = binder.FallbackDeleteMember(this);
                    Expression expression = Expression.Condition
                    (
                        Expression.Call
                        (
                            Expression.Constant(Value),
                            minfo,
                            Expression.Constant(binder)
                        ),
                        Expression.Default(binder.ReturnType),
                        getDefault.Expression,
                        binder.ReturnType
                    );

                    BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                    DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
                    return binder.FallbackDeleteMember(this, dynamicSuggestion);
                }
            }
            return base.BindDeleteMember(binder);
        }

        public override DynamicMetaObject BindGetIndex(GetIndexBinder binder, DynamicMetaObject[] indexes)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryGetIndex", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] args = minfo.GetParameters();
                if (args.Length == 3 && args[0].ParameterType == typeof(GetIndexBinder) && args[1].ParameterType == typeof(object[]) && args[2].IsOut && args[2].ParameterType == typeof(object).MakeByRefType())
                {
                    DynamicMetaObject getDefault = binder.FallbackGetIndex(this, indexes);
                    ParameterExpression tmp = Expression.Variable(typeof(object), "tmp0");
                    Expression expression = Expression.Block
                    (
                        new ParameterExpression[] { tmp },
                        Expression.Condition
                        (
                            Expression.Call
                            (
                                Expression.Constant(Value),
                                minfo,
                                Expression.Constant(binder),
                                Expression.NewArrayInit
                                (
                                    typeof(object),
                                    indexes.ToExpressionList<object>()
                                ),
                                tmp
                            ),
                            tmp,
                            getDefault.Expression,
                            binder.ReturnType
                        )
                    );

                    BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                    DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
                    return binder.FallbackGetIndex(this, indexes, dynamicSuggestion);
                }
            }
            return base.BindGetIndex(binder, indexes);
        }
        public override DynamicMetaObject BindSetIndex(SetIndexBinder binder, DynamicMetaObject[] indexes, DynamicMetaObject value)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TrySetIndex", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] args = minfo.GetParameters();
                if (args.Length == 3 && args[0].ParameterType == typeof(SetIndexBinder) && args[1].ParameterType == typeof(object[]) && args[2].ParameterType == typeof(object))
                {
                    DynamicMetaObject getDefault = binder.FallbackSetIndex(this, indexes, value);
                    Expression expression = Expression.Block
                    (
                        Expression.Condition
                        (
                            Expression.Call
                            (
                                Expression.Constant(Value),
                                minfo,
                                Expression.Constant(binder),
                                Expression.NewArrayInit
                                (
                                    typeof(object),
                                    indexes.ToExpressionList<object>()
                                ),
                                Expression.Convert
                                (
                                    value.Expression,
                                    typeof(object)
                                )
                            ),
                            Expression.Default(binder.ReturnType),
                            getDefault.Expression,
                            binder.ReturnType
                        )
                    );

                    BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                    DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
                    return binder.FallbackSetIndex(this, indexes, value, dynamicSuggestion);
                }
            }
            return base.BindSetIndex(binder, indexes, value);
        }
        public override DynamicMetaObject BindDeleteIndex(DeleteIndexBinder binder, DynamicMetaObject[] indexes)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryDeleteIndex", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] args = minfo.GetParameters();
                if (args.Length == 2 && args[0].ParameterType == typeof(DeleteIndexBinder) && args[1].ParameterType == typeof(object[]))
                {
                    DynamicMetaObject getDefault = binder.FallbackDeleteIndex(this, indexes);
                    Expression expression = Expression.Condition
                    (
                        Expression.Call
                        (
                            Expression.Constant(Value),
                            minfo,
                            Expression.Constant(binder),
                            Expression.NewArrayInit
                            (
                                typeof(object),
                                indexes.ToExpressionList<object>()
                            )
                        ),
                        Expression.Default(binder.ReturnType),
                        getDefault.Expression,
                        binder.ReturnType
                    );

                    BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                    DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
                    return binder.FallbackDeleteIndex(this, indexes, dynamicSuggestion);
                }
            }
            return base.BindDeleteIndex(binder, indexes);
        }

        public override DynamicMetaObject BindInvoke(InvokeBinder binder, DynamicMetaObject[] args)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryInvoke", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] p_args = minfo.GetParameters();
                if (p_args.Length == 3 && p_args[0].ParameterType == typeof(InvokeBinder) && p_args[1].ParameterType == typeof(object[]) && p_args[2].IsOut && p_args[2].ParameterType == typeof(object).MakeByRefType())
                {
                    DynamicMetaObject dynamicSuggestion = CreateExpression_3(minfo, binder, args, binder.FallbackInvoke(this, args));
                    return binder.FallbackInvoke(this, args, dynamicSuggestion);
                }
            }
            return base.BindInvoke(binder, args);
        }
        public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryInvokeMember", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] p_args = minfo.GetParameters();
                if (p_args.Length == 3 && p_args[0].ParameterType == typeof(InvokeMemberBinder) && p_args[1].ParameterType == typeof(object[]) && p_args[2].IsOut && p_args[2].ParameterType == typeof(object).MakeByRefType())
                {
                    DynamicMetaObject dynamicSuggestion = CreateExpression_3(minfo, binder, args, binder.FallbackInvokeMember(this, args));
                    return binder.FallbackInvokeMember(this, args, dynamicSuggestion);
                }
            }
            return base.BindInvokeMember(binder, args);
        }

        public override DynamicMetaObject BindConvert(ConvertBinder binder)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryConvert", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] args = minfo.GetParameters();
                if (args.Length == 2 && args[0].ParameterType == typeof(ConvertBinder) && args[1].IsOut && args[1].ParameterType == typeof(object).MakeByRefType())
                {
                    DynamicMetaObject getDefault = binder.FallbackConvert(this);
                    ParameterExpression tmp = Expression.Variable(typeof(object), "tmp0");
                    Expression expression = Expression.Block
                    (
                        new ParameterExpression[] { tmp },
                        Expression.Condition
                        (
                            Expression.Call
                            (
                                Expression.Constant(Value),
                                minfo,
                                Expression.Constant(binder),
                                tmp
                            ),
                            Expression.Convert
                            (
                                tmp,
                                binder.ReturnType
                            ),
                            getDefault.Expression,
                            binder.ReturnType
                        )
                    );

                    BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                    return new DynamicMetaObject(expression, restrictions);
                }
            }
            return base.BindConvert(binder);
        }
        public override DynamicMetaObject BindCreateInstance(CreateInstanceBinder binder, DynamicMetaObject[] args)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryCreateInstance", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] p_args = minfo.GetParameters();
                if (p_args.Length == 3 && p_args[0].ParameterType == typeof(CreateInstanceBinder) && p_args[1].ParameterType == typeof(object[]) && p_args[2].IsOut && p_args[2].ParameterType == typeof(object).MakeByRefType())
                {
                    DynamicMetaObject getDefault = binder.FallbackCreateInstance(this, args);
                    ParameterExpression tmp = Expression.Variable(typeof(object), "tmp0");
                    Expression expression = Expression.Block
                    (
                        new ParameterExpression[] { tmp },
                        Expression.Condition
                        (
                            Expression.Call
                            (
                                Expression.Constant(Value),
                                minfo,
                                Expression.Constant(binder),
                                Expression.NewArrayInit
                                (
                                    typeof(object),
                                    args.ToExpressionList<object>()
                                ),
                                tmp
                            ),
                            Expression.Convert
                            (
                                tmp,
                                binder.ReturnType
                            ),
                            getDefault.Expression,
                            binder.ReturnType
                        )
                    );

                    BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                    return new DynamicMetaObject(expression, restrictions);
                }
            }
            return base.BindCreateInstance(binder, args);
        }

        public override DynamicMetaObject BindBinaryOperation(BinaryOperationBinder binder, DynamicMetaObject arg)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryBinaryOperation", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] p_args = minfo.GetParameters();
                if (p_args.Length == 3 && p_args[0].ParameterType == typeof(BinaryOperationBinder) && p_args[1].ParameterType == typeof(object) && p_args[2].IsOut && p_args[2].ParameterType == typeof(object).MakeByRefType())
                {
                    DynamicMetaObject getDefault = binder.FallbackBinaryOperation(this, arg);
                    ParameterExpression tmp = Expression.Variable(typeof(object), "tmp0");
                    Expression expression = Expression.Block
                    (
                        new ParameterExpression[] { tmp },
                        Expression.Condition
                        (
                            Expression.Call
                            (
                                Expression.Constant(Value),
                                minfo,
                                Expression.Constant(binder),
                                Expression.Convert
                                (
                                    arg.Expression,
                                    typeof(object)
                                ),
                                tmp
                            ),
                            tmp,
                            getDefault.Expression,
                            binder.ReturnType
                        )
                    );

                    BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
                    DynamicMetaObject dynamicSuggestion = new DynamicMetaObject(expression, restrictions);
                    return binder.FallbackBinaryOperation(this, arg, dynamicSuggestion);
                }
            }
            return base.BindBinaryOperation(binder, arg);
        }
        public override DynamicMetaObject BindUnaryOperation(UnaryOperationBinder binder)
        {
            MethodInfo minfo = Value.GetType().GetMethod("TryUnaryOperation", BindingFlags.Public | BindingFlags.Instance);
            if (minfo != null && minfo.ReturnType == typeof(bool))
            {
                ParameterInfo[] args = minfo.GetParameters();
                if (args.Length == 2 && args[0].ParameterType == typeof(UnaryOperationBinder) && args[1].IsOut && args[1].ParameterType == typeof(object).MakeByRefType())
                {
                    DynamicMetaObject dynamicSuggestion = CreateExpression_2(minfo, binder, binder.FallbackUnaryOperation(this));
                    return binder.FallbackUnaryOperation(this, dynamicSuggestion);
                }
            }
            return base.BindUnaryOperation(binder);
        }

        DynamicMetaObject CreateExpression_2(MethodInfo minfo, DynamicMetaObjectBinder binder, DynamicMetaObject getDefault)
        {
            ParameterExpression tmp = Expression.Variable(typeof(object), "tmp0");
            Expression expression = Expression.Block
            (
                new ParameterExpression[] { tmp },
                Expression.Condition
                (
                    Expression.Call
                    (
                        Expression.Constant(Value),
                        minfo,
                        Expression.Constant(binder),
                        tmp
                    ),
                    tmp,
                    getDefault.Expression,
                    binder.ReturnType
                )
            );

            BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
            return new DynamicMetaObject(expression, restrictions);
        }
        DynamicMetaObject CreateExpression_3(MethodInfo minfo, DynamicMetaObjectBinder binder, DynamicMetaObject[] args, DynamicMetaObject getDefault)
        {
            ParameterExpression tmp = Expression.Variable(typeof(object), "tmp0");
            Expression expression = Expression.Block
            (
                new ParameterExpression[] { tmp },
                Expression.Condition
                (
                    Expression.Call
                    (
                        Expression.Constant(Value),
                        minfo,
                        Expression.Constant(binder),
                        Expression.NewArrayInit
                        (
                            typeof(object),
                            args.ToExpressionList<object>()
                        ),
                        tmp
                    ),
                    tmp,
                    getDefault.Expression,
                    binder.ReturnType
                )
            );

            BindingRestrictions restrictions = GetRestrictions().Merge(getDefault.Restrictions);
            return new DynamicMetaObject(expression, restrictions);
        }

        BindingRestrictions GetRestrictions()
        {
            if (Value == null && HasValue) return BindingRestrictions.GetInstanceRestriction(Expression, null);
            else return BindingRestrictions.GetTypeRestriction(Expression, LimitType);
        }
    }
}