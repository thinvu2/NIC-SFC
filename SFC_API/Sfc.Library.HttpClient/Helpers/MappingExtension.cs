using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Sfc.Library.HttpClient.Helpers
{
    public static class MappingExtension
    {
        //public static T ToObject<T>(this IDictionary<string, object> source)
        //    where T : class, new()
        //{
        //    var someObject = new T();
        //    var someObjectType = someObject.GetType();

        //    foreach (var item in source)
        //    {
        //        var key = char.ToUpper(item.Key[0]) + item.Key.Substring(1);
        //        var targetProperty = someObjectType.GetProperty(key);


        //        if (targetProperty.PropertyType == typeof(string))
        //        {
        //            targetProperty.SetValue(someObject, item.Value);
        //        }
        //        else
        //        {

        //            var parseMethod = targetProperty.PropertyType.GetMethod("TryParse",
        //                BindingFlags.Public | BindingFlags.Static, null,
        //                new[] { typeof(string), targetProperty.PropertyType.MakeByRefType() }, null);

        //            if (parseMethod != null)
        //            {
        //                var parameters = new[] { item.Value, null };
        //                var success = (bool)parseMethod.Invoke(null, parameters);
        //                if (success)
        //                {
        //                    targetProperty.SetValue(someObject, parameters[1]);
        //                }

        //            }
        //        }
        //    }

        //    return someObject;
        //}

        //public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        //{
        //    return source.GetType().GetProperties(bindingAttr).ToDictionary
        //    (
        //        propInfo => propInfo.Name,
        //        propInfo => propInfo.GetValue(source, null)
        //    );
        //}
        //public static T ToObject<T>(this IDictionary<string, object> source)
        //where T : class, new()
        //{
        //    var someObject = new T();
        //    var someObjectType = someObject.GetType();

        //    foreach (var item in source)
        //    {
        //        someObjectType
        //                 .GetProperty(item.Key)
        //                 .SetValue(someObject, item.Value, null);
        //    }

        //    return someObject;
        //}

        //public static IDictionary<string, object> AsDictionary(this object source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        //{
        //    return source.GetType().GetProperties(bindingAttr).ToDictionary
        //    (
        //        propInfo => propInfo.Name,
        //        propInfo => propInfo.GetValue(source, null)
        //    );

        //}

        //public static T ToClass<T>(this IDictionary<string, object> dict) where T : class, new()
        //{
        //    return (T)ConverToClass(dict, typeof(T));
        //}

        //private static object ConverToClass(IDictionary<string, object> dic, Type classToUse)
        //{
        //    Type type = classToUse;
        //    var obj = Activator.CreateInstance(type);

        //    foreach (var item in dic)
        //    {
        //        var property = type.GetProperty(item.Key);
        //        if (property == null) continue;

        //        var value = item.Value;
        //        if (value is Dictionary<string, object> && !property.PropertyType.FullName.Contains("Generic.IList"))
        //        {
        //            property.SetValue(obj, ConverToClass((Dictionary<string, object>)(item.Value), property.PropertyType));
        //            continue;
        //        }
        //        if (property.PropertyType.FullName.Contains("Generic.IList"))
        //        {
        //            var subClassTouse = property.PropertyType.GetGenericArguments()[0];

        //            Type genericListType = typeof(List<>);
        //            Type concreteListType = genericListType.MakeGenericType(subClassTouse);
        //            var list = (IList)Activator.CreateInstance(concreteListType, new object[] { });

        //            var values = (Dictionary<string, object>)dic[item.Key];

        //            foreach (var itemClass in values)
        //            {
        //                list.Add(ConverToClass((Dictionary<string, object>)itemClass.Value, subClassTouse));
        //            }
        //            property.SetValue(obj, list);
        //            continue;
        //        }
        //        property.SetValue(obj, item.Value);
        //    }

        //    return obj;
        //}

        public static T ToObject<T>(this IDictionary<string, object> dict) 
            where T : class, new()
        {
            var t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    continue;

                KeyValuePair<string, object> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

                // ...and change the type
                object newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }
            return t;
        }
        public static T Create<T>(this IDictionary<string, object> propertyBag)
        {
            var result = (T)FormatterServices.GetUninitializedObject(typeof(T));

            foreach (var item in from member in typeof(T).GetMembers()
                                 let dataMemberAttr = member.GetCustomAttributes(typeof(DataMemberAttribute), true).Cast<DataMemberAttribute>().SingleOrDefault()
                                 where dataMemberAttr != null && propertyBag.ContainsKey(dataMemberAttr.Name)
                                 select new { Member = member, Value = propertyBag[dataMemberAttr.Name] })
            {
                var property = item.Member as PropertyInfo;
                if (property != null)
                {
                    property.SetValue(result, item.Value, null);
                    continue;
                }

                var field = item.Member as FieldInfo;
                if (field != null)
                {
                    field.SetValue(result, item.Value);
                    continue;
                }
            }

            return result;
        }
        //private static T Get<T>(string key)
        //{
        //    return (T)Convert.ChangeType(_dict[key], typeof(T), CultureInfo.InvariantCulture);
        //}
        public static IEnumerable<T> ToListObject<T>(this IEnumerable<IDictionary<string, object>> dicts)
           where T : class, new()
        {
            var ts = new List<T>();
            foreach(var dict in dicts)
            {
                var t = new T();
                PropertyInfo[] properties = t.GetType().GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                        continue;

                    KeyValuePair<string, object> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                    // Find which property type (int, string, double? etc) the CURRENT property is...
                    Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

                    // Fix nullables...
                    Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

                    // ...and change the type
                    object newA = Convert.ChangeType(item.Value, newT);
                    t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
                }
                ts.Add(t);
            }
            return ts.AsEnumerable();
 
        }
    }
}
