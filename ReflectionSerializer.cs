using System.Collections;
using System.Reflection;
using System.Xml;

namespace ProdToolDOOM;

public class ReflectionSerializer<T, TU> where T : notnull
#if WINDOWS
    where TU : XmlWriter
#endif
{
    public void Serialize(T obj, TU writer)
    {
        #if WINDOWS
        var type = typeof(T);
        
        if (type.IsPrimitive)
        {
            writer.WriteStartElement(type.Name);
            writer.WriteValue(obj);
            writer.WriteEndElement();
            return;
        }
        
        if (type.IsGenericType)
        {
            SerializeProperty(obj, type.Name, writer);
            return;
        }

        writer.WriteStartElement(type.Name);
        foreach (var prop in type.GetProperties())
        {
            var value = prop.GetValue(obj);

            if (value == null) continue;

            SerializeProperty(value, prop.Name, writer);
        }

        writer.WriteEndElement();
        #endif
    }

    private void SerializeProperty(object value, string propName, TU writer)
    {
        bool wasCollection = false;
        try
        {
            Type valueType = value.GetType();
            if (valueType.IsGenericType)
            {
                wasCollection = CheckIfList(value, propName, writer, valueType);
                if (!wasCollection)
                    wasCollection = CheckIfDictionary(value, propName, writer, valueType);                
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }

        if (wasCollection) return;
        writer.WriteStartElement(propName);
        writer.WriteString(value.ToString());
        writer.WriteEndElement();
    }

    private bool CheckIfList(object value, string propName, TU writer, Type type)
    {
        if (type.GetGenericTypeDefinition() == typeof(List<>))
        {
            IList foundList = ((IList)value);
            if (foundList.Count > 0)
            {
                Type elementType = type.GetGenericArguments()[0];
                
                // new ReflectionSerializer, with T being elementType, and U remaining the same
                var serializerType = typeof(ReflectionSerializer<,>)
                    .MakeGenericType(elementType, typeof(TU));
                
                object? serializerInstance = Activator.CreateInstance(serializerType);
                
                // Then Trigger SerializeList inside that, passing allong foundList, with prop.Name and writer
                MethodInfo? serializeMethod = serializerType.GetMethod(
                    nameof(SerializeList),
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    [type, typeof(string), typeof(TU)],
                    null
                );
                
                serializeMethod?.Invoke(serializerInstance, [foundList, propName, writer]);
                return true;
            }
        }
        return false;
    }

    private bool CheckIfDictionary(object value, string propName, TU writer, Type type)
    {
        if (type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            Debug.Log(type.GetGenericArguments().ToString());
            // IDictionary<,> foundList = ((IDictionary<,>)value);
            // if (foundList.Count > 0)
            // {
            //     Type elementType = valueType.GetGenericArguments()[0];
            //     
            //     // new ReflectionSerializer, with T being elementType, and U remaining the same
            //     var serializerType = typeof(ReflectionSerializer<,>)
            //         .MakeGenericType(elementType, typeof(TU));
            //     
            //     object? serializerInstance = Activator.CreateInstance(serializerType);
            //     
            //     // Then Trigger SerializeList inside that, passing allong foundList, with prop.Name and writer
            //     MethodInfo? serializeMethod = serializerType.GetMethod(
            //         nameof(SerializeList),
            //         BindingFlags.Instance | BindingFlags.Public,
            //         null,
            //         [valueType, typeof(string), typeof(TU)],
            //         null
            //     );
            //     
            //     serializeMethod?.Invoke(serializerInstance, [foundList, propName, writer]);
            //     return true;
            // }
        }
        return false;
    }
    
    public void SerializeList(List<T> list, string name, TU writer)
    {
        if (list.Count <= 0) return;
        #if WINDOWS
        writer.WriteStartElement($"{name}_List");
        writer.WriteStartElement($"{name}_Count");
        writer.WriteValue(list.Count);
        writer.WriteEndElement();
        foreach (var instance in list)
        {
            new ReflectionSerializer<T, TU>().Serialize(instance, writer);
        }
        writer.WriteEndElement();
        #endif
    }
    
    public void SerializeDictionary<TW>(Dictionary<TW, T> dict, string name, TU writer) where TW : notnull
    {
        if (dict.Count <= 0) return;
        #if WINDOWS
        writer.WriteStartElement($"{name}_Dict");
        writer.WriteStartElement($"{name}_Count");
        writer.WriteValue(dict.Count);
        writer.WriteEndElement();
        foreach (var (key, value) in dict)
        {
            writer.WriteStartElement($"{name}_Entry");
            new ReflectionSerializer<TW, TU>().Serialize(key, writer);
            new ReflectionSerializer<T, TU>().Serialize(value, writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
        #endif
    }
}