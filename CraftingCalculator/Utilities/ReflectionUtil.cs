using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace CraftingCalculator.Utilities
{
    public static class ReflectionUtil
    {
        /// <summary>
        /// Thank you Repo Man
        /// https://stackoverflow.com/questions/5411694/get-all-inherited-classes-of-an-abstract-class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetEnumerableOfType<T>(string nspace, params object[] constructorArgs)
        {
            List<T> objects = new List<T>();

            if(nspace != null)
            {
                foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(t => t.IsClass
                && !t.IsAbstract
                && t.Namespace == nspace
                && t.IsSubclassOf(typeof(T))))
                {
                    objects.Add((T)Activator.CreateInstance(type, constructorArgs));
                }
            }
            else
            {
                foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(t => t.IsClass
                && !t.IsAbstract
                && t.IsSubclassOf(typeof(T))))
                {
                    objects.Add((T)Activator.CreateInstance(type, constructorArgs));
                }
            }

            return objects;
        }
    }
}
