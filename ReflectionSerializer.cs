using System.Collections;
using System.Reflection;
using System.Xml;

namespace ProdToolDOOM;

public class ReflectionSerializer<T, U> 
#if WINDOWS
    where U : XmlWriter
#endif
{
    public void Serialize(T obj, U writer)
    {
        #if WINDOWS
        var type = typeof(T);
        writer.WriteStartElement(type.Name);

        foreach (var prop in type.GetProperties())
        {
            var value = prop.GetValue(obj);

            if (value == null) continue;

            bool wasCollection = false;
            try
            {
                Type valueType = value.GetType();
                if (valueType.IsGenericType &&
                    valueType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    IList foundList = ((IList)value);
                    if (foundList.Count > 0)
                    {
                        wasCollection = true;
                        Type elementType = valueType.GetGenericArguments()[0];
                
                        // new ReflectionSerializer, with T being elementType, and U remaining the same
                        var serializerType = typeof(ReflectionSerializer<,>)
                            .MakeGenericType(elementType, typeof(U));
                
                        object? serializerInstance = Activator.CreateInstance(serializerType);
                
                        // Then Trigger SerializeList inside that, passing allong foundList, with prop.Name and writer
                        MethodInfo? serializeMethod = serializerType.GetMethod(
                            nameof(SerializeList),
                            BindingFlags.Instance | BindingFlags.Public,
                            null,
                            [valueType, typeof(string), typeof(U)],
                            null
                        );
                
                        serializeMethod?.Invoke(serializerInstance, [foundList, prop.Name ,writer]);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                throw;
            }

            if (wasCollection) continue;
            writer.WriteStartElement(prop.Name);
            writer.WriteString(value.ToString());
            writer.WriteEndElement();
        }

        writer.WriteEndElement();
        #endif
    }
    
    public void SerializeList(List<T> list, string name, U writer)
    {
        #if WINDOWS
        if (Project.levels.Count > 0)
        {
            writer.WriteStartElement(name);
            writer.WriteStartElement($"{name}_Count");
            writer.WriteValue(list.Count);
            writer.WriteEndElement();
            foreach (var instance in list)
            {
                new ReflectionSerializer<T, U>().Serialize(instance, writer);
            }
            writer.WriteEndElement();
        }
        #endif
    }
}