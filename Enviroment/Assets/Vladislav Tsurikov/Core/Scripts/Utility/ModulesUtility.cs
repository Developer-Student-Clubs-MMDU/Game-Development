using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.Assertions;
using UnityEngine;
using UnityEditor;

namespace VladislavTsurikov
{
    public static class ModulesUtility
    {
        #region Reflection
        static IEnumerable<Type> m_AssemblyTypes;
        public static IEnumerable<Type> GetAllAssemblyTypes()
        {
            if (m_AssemblyTypes == null)
            {
                m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(t =>
                    {
                        // Ugly hack to handle mis-versioned dlls
                        var innerTypes = new Type[0];
                        try
                        {
                            innerTypes = t.GetTypes();
                        }
                        catch { }
                        return innerTypes;
                    });
            }

            return m_AssemblyTypes;
        }

        public static IEnumerable<System.Type> GetAllTypesDerivedFrom<T>()
        {
#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
            return UnityEditor.TypeCache.GetTypesDerivedFrom<T>();
#else
            return GetAllAssemblyTypes().Where(t => t.IsSubclassOf(typeof(T)));
#endif
        }

        /// <summary>
        /// Helper method to get the first attribute of type <c>T</c> on a given type.
        /// </summary>
        /// <typeparam name="T">The attribute type to look for</typeparam>
        /// <param name="type">The type to explore</param>
        /// <returns>The attribute found</returns>
        public static T GetAttribute<T>(this System.Type type) where T : Attribute
        {
            Assert.IsTrue(type.IsDefined(typeof(T), false), "Attribute not found");
            return (T)type.GetCustomAttributes(typeof(T), false)[0];
        }

        public static Attribute[] GetMemberAttributes<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            Expression body = expr;

            if (body is LambdaExpression)
                body = ((LambdaExpression)body).Body;

            switch (body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var fi = (FieldInfo)((MemberExpression)body).Member;
                    return fi.GetCustomAttributes(false).Cast<Attribute>().ToArray();
                default:
                    throw new InvalidOperationException();
            }
        }

        public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
        {
            MemberExpression me;
            switch (expr.Body.NodeType)
            {
                case ExpressionType.MemberAccess:
                    me = expr.Body as MemberExpression;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            var members = new List<string>();
            while (me != null)
            {
                members.Add(me.Member.Name);
                me = me.Expression as MemberExpression;
            }

            var sb = new StringBuilder();
            for (int i = members.Count - 1; i >= 0; i--)
            {
                sb.Append(members[i]);
                if (i > 0) sb.Append('.');
            }

            return sb.ToString();
        }

        public static bool DoesTypeExist(string className)
        {
             var foundType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              from type in GetTypesSafe(assembly)
                              where type.Name == className
                              select type).FirstOrDefault();

            return foundType != null;
        }

        public static IEnumerable<Type> GetTypesSafe(System.Reflection.Assembly assembly)
        {
            Type[] types;

            try
            {
               types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types;
            }

            return types.Where(x => x != null);
        }

#if UNITY_EDITOR
        public static ScriptableObject CreateAsset(System.Type type, ScriptableObject parentAsset)
        {
            var newAsset = ScriptableObject.CreateInstance(type);
            newAsset.name = type.Name;

            AssetDatabase.AddObjectToAsset(newAsset, parentAsset);   
            AssetDatabase.SaveAssets();

            return newAsset;
        }

        public static void RemoveAsset(ScriptableObject asset)
        {
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(asset);
            AssetDatabase.SaveAssets();
        }
#endif

        public static void Destroy(UnityEngine.Object obj)
        {
            if (obj != null)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
        }

        #endregion
    }
}