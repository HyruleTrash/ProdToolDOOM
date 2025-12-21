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

        if (type == typeof(Vector2))
        {
            writer.WriteStartElement(type.Name);
            writer.WriteString(obj.ToString());
            writer.WriteEndElement();
            return;
        }
        
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
            Debug.LogError(e.Message);
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
        var arguments = type.GetGenericArguments();
        if (type.GetGenericTypeDefinition() != typeof(Dictionary<,>) || arguments.Length != 2) return false;
        if (value is not IDictionary dict || dict.Count <= 0) return false;
                
        // new ReflectionSerializer, with T being redundant, and U remaining the same
        var serializerType = typeof(ReflectionSerializer<,>).MakeGenericType(typeof(object), typeof(TU));
        object? serializerInstance = Activator.CreateInstance(serializerType);
                
        // Then Trigger SerializeDictionary inside that, passing along dict, with prop.Name and writer
        MethodInfo? serializeMethod = serializerType.GetMethod(
            nameof(SerializeDictionary),
            BindingFlags.Instance | BindingFlags.Public,
            null,
            [typeof(IDictionary), typeof(string), typeof(TU)],
            null
        );
        
        serializeMethod?.Invoke(serializerInstance, [dict, propName, writer]);
        return true;
    }
    
    public void SerializeList(List<T> list, string name, TU writer)
    {
        if (list.Count <= 0) return;
        #if WINDOWS
        writer.WriteStartElement($"{name}");
        writer.WriteAttributeString("collectionType", "List");
        writer.WriteStartAttribute("count");
        writer.WriteValue(list.Count);
        writer.WriteEndAttribute();
        foreach (var instance in list)
        {
            writer.WriteStartElement($"{name}_Entry");
            new ReflectionSerializer<T, TU>().Serialize(instance, writer);
            writer.WriteEndElement();
        }
        writer.WriteEndElement();
        #endif
    }
    
    public void SerializeDictionary(IDictionary dict, string name, TU writer)
    {
        if (dict.Count <= 0) return;
        #if WINDOWS
        writer.WriteStartElement($"{name}");
        writer.WriteAttributeString("collectionType", "Dictionary");
        writer.WriteStartAttribute("count");
        writer.WriteValue(dict.Count);
        writer.WriteEndAttribute();
        
        var dictTypes = dict.GetType().GetGenericArguments();
        Type dictKeyType = dictTypes[0];
        Type dictValueType = dictTypes[1];
            
        // new ReflectionSerializer, with T being KeyType, and U remaining the same
        var serializerType = typeof(ReflectionSerializer<,>).MakeGenericType(dictKeyType, typeof(TU));
        object? serializerInstance = Activator.CreateInstance(serializerType);
        MethodInfo? serializeMethod = serializerType.GetMethod(
            nameof(Serialize),
            BindingFlags.Instance | BindingFlags.Public,
            null,
            [dictKeyType, typeof(TU)],
            null
        );
            
        // new ReflectionSerializer, with T being dictValueType, and U remaining the same
        var serializerTypeValue = typeof(ReflectionSerializer<,>).MakeGenericType(dictValueType, typeof(TU));
        object? serializerInstanceValue = Activator.CreateInstance(serializerTypeValue);
        MethodInfo? serializeMethodValue = serializerTypeValue.GetMethod(
            nameof(Serialize),
            BindingFlags.Instance | BindingFlags.Public,
            null,
            [dictValueType, typeof(TU)],
            null
        );
        
        foreach (DictionaryEntry o in dict)
        {
            writer.WriteStartElement($"{name}_Entry");
            
            serializeMethod?.Invoke(serializerInstance, [o.Key, writer]);
            serializeMethodValue?.Invoke(serializerInstanceValue, [o.Value, writer]);
            
            writer.WriteEndElement();
        }
        
        writer.WriteEndElement();
        #endif
    }
}