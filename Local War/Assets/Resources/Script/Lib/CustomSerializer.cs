using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


/// <summary>
/// Custom Serializer for network and file serialization
/// </summary>
public class CustomSerializer
{
    protected static SortedDictionary<Type, Dictionary<string, SerializationAction>> networkSerializationMap =
        new SortedDictionary<Type, Dictionary<string, SerializationAction>>(new TypeComparer());

    protected static SortedDictionary<Type, XmlSerializer> fileSerializationMap = new
        SortedDictionary<Type, XmlSerializer>(new TypeComparer());

    /// <summary>
    /// Dictionary implemtation of a switch statment. faster then c# switch and allows for any type
    /// </summary>
    protected static SortedDictionary<Type, Action<FieldInfo, object, NetworkWriter>> NetworkSerializeDictionary =
        new SortedDictionary<Type, Action<FieldInfo, object, NetworkWriter>>(new TypeComparer())
    {
        {typeof(bool), (field, item , writer) => {
            writer.Write((bool)field.GetValue(item));
        }},
        {typeof(byte), (field, item , writer) => {
            writer.Write((byte)field.GetValue(item));
        }},
        {typeof(char), (field, item , writer) => {
            writer.Write((char)field.GetValue(item));
        }},
        {typeof(Color), (field, item , writer) => {
            writer.Write((Color)field.GetValue(item));
        }},
        {typeof(Color32), (field, item , writer) => {
            writer.Write((Color32)field.GetValue(item));
        }},
        {typeof(decimal), (field, item , writer) => {
            writer.Write((decimal)field.GetValue(item));
        }},
        {typeof(double), (field, item , writer) => {
            writer.Write((double)field.GetValue(item));
        }},
        {typeof(float), (field, item , writer) => {
            writer.Write((float)field.GetValue(item));
        }},
        {typeof(GameObject), (field, item , writer) => {
            writer.Write((GameObject)field.GetValue(item));
        }},
        {typeof(int), (field, item , writer) => {
            writer.Write((int)field.GetValue(item));
        }},
        {typeof(long), (field, item , writer) => {
            writer.Write((long)field.GetValue(item));
        }},
        {typeof(Matrix4x4), (field, item , writer) => {
            writer.Write((Matrix4x4)field.GetValue(item));
        }},
        {typeof(MessageBase), (field, item , writer) => {
            writer.Write((MessageBase)field.GetValue(item));
        }},
        {typeof(NetworkHash128), (field, item , writer) => {
            writer.Write((NetworkHash128)field.GetValue(item));
        }},
        {typeof(NetworkIdentity), (field, item , writer) => {
            writer.Write((NetworkIdentity)field.GetValue(item));
        }},
        {typeof(NetworkInstanceId), (field, item , writer) => {
            writer.Write((NetworkInstanceId)field.GetValue(item));
        }},
        {typeof(NetworkSceneId), (field, item , writer) => {
            writer.Write((NetworkSceneId)field.GetValue(item));
        }},
        {typeof(Plane), (field, item , writer) => {
            writer.Write((Plane)field.GetValue(item));
        }},
        {typeof(Quaternion), (field, item , writer) => {
            writer.Write((Quaternion)field.GetValue(item));
        }},
        {typeof(Ray), (field, item , writer) => {
            writer.Write((Ray)field.GetValue(item));
        }},
        {typeof(Rect), (field, item , writer) => {
            writer.Write((Rect)field.GetValue(item));
        }},
        {typeof(sbyte), (field, item , writer) => {
            writer.Write((sbyte)field.GetValue(item));
        }},
        {typeof(short), (field, item , writer) => {
            writer.Write((short)field.GetValue(item));
        }},
        {typeof(string), (field, item , writer) => {
            writer.Write((string)field.GetValue(item));
        }},
        {typeof(Transform), (field, item , writer) => {
            writer.Write((Transform)field.GetValue(item));
        }},
        {typeof(uint), (field, item , writer) => {
            writer.Write((uint)field.GetValue(item));
        }},
        {typeof(ulong), (field, item , writer) => {
            writer.Write((ulong)field.GetValue(item));
        }},
        {typeof(ushort), (field, item , writer) => {
            writer.Write((ushort)field.GetValue(item));
        }},
        {typeof(Vector2), (field, item , writer) => {
            writer.Write((Vector2)field.GetValue(item));
        }},
        {typeof(Vector3), (field, item , writer) => {
            writer.Write((Vector3)field.GetValue(item));
        }},
        {typeof(Vector4), (field, item , writer) => {
            writer.Write((Vector4)field.GetValue(item));
        }}
    };

    /// <summary>
    /// Dictionary implemtation of a switch statment. faster then c# switch and allows for any type
    /// </summary>
    protected static SortedDictionary<Type, Action<FieldInfo, object, NetworkReader>> networkDeserializeDictionary =
        new SortedDictionary<Type, Action<FieldInfo, object, NetworkReader>>(new TypeComparer())
{
        {typeof(bool), (field, item , reader) => {
            field.SetValue(item,reader.ReadBoolean());
        }},
        {typeof(byte), (field, item , reader) => {
            field.SetValue(item,reader.ReadByte());
        }},
        {typeof(char), (field, item , reader) => {
            field.SetValue(item,reader.ReadChar());
        }},
        {typeof(Color), (field, item , reader) => {
            field.SetValue(item,reader.ReadColor());
        }},
        {typeof(Color32), (field, item , reader) => {
            field.SetValue(item,reader.ReadColor32());
        }},
        {typeof(decimal), (field, item , reader) => {
            field.SetValue(item,reader.ReadDecimal());
        }},
        {typeof(double), (field, item , reader) => {
            field.SetValue(item,reader.ReadDouble());
        }},
        {typeof(float), (field, item , reader) => {
            field.SetValue(item,reader.ReadSingle());
        }},
        {typeof(GameObject), (field, item , reader) => {
            field.SetValue(item,reader.ReadGameObject());
        }},
        {typeof(int), (field, item , reader) => {
            field.SetValue(item,reader.ReadInt32());
        }},
        {typeof(long), (field, item , reader) => {
            field.SetValue(item,reader.ReadInt64());
        }},
        {typeof(Matrix4x4), (field, item , reader) => {
            field.SetValue(item,reader.ReadMatrix4x4());
        }},
        {typeof(NetworkHash128), (field, item , reader) => {
            field.SetValue(item,reader.ReadNetworkHash128());
        }},
        {typeof(NetworkIdentity), (field, item , reader) => {
            field.SetValue(item,reader.ReadNetworkIdentity());
        }},
        {typeof(NetworkInstanceId), (field, item , reader) => {
            field.SetValue(item,reader.ReadNetworkId());
        }},
        {typeof(NetworkSceneId), (field, item , reader) => {
            field.SetValue(item,reader.ReadSceneId());
        }},
        {typeof(Plane), (field, item , reader) => {
            field.SetValue(item,reader.ReadPlane());
        }},
        {typeof(Quaternion), (field, item , reader) => {
            field.SetValue(item,reader.ReadQuaternion());
        }},
        {typeof(Ray), (field, item , reader) => {
            field.SetValue(item,reader.ReadRay());
        }},
        {typeof(Rect), (field, item , reader) => {
            field.SetValue(item,reader.ReadRect());
        }},
        {typeof(sbyte), (field, item , reader) => {
            field.SetValue(item,reader.ReadSByte());
        }},
        {typeof(short), (field, item , reader) => {
            field.SetValue(item,reader.ReadInt16());
        }},
        {typeof(string), (field, item , reader) => {
            field.SetValue(item,reader.ReadString());
        }},
        {typeof(Transform), (field, item , reader) => {
            field.SetValue(item,reader.ReadTransform());
        }},
        {typeof(uint), (field, item , reader) => {
            field.SetValue(item,reader.ReadUInt32());
        }},
        {typeof(ulong), (field, item , reader) => {
            field.SetValue(item,reader.ReadUInt64());
        }},
        {typeof(ushort), (field, item , reader) => {
            field.SetValue(item,reader.ReadUInt16());
        }},
        {typeof(Vector2), (field, item , reader) => {
            field.SetValue(item,reader.ReadVector2());
        }},
        {typeof(Vector3), (field, item , reader) => {
            field.SetValue(item,reader.ReadVector3());
        }},
        {typeof(Vector4), (field, item , reader) => {
            field.SetValue(item,reader.ReadVector4());
        }}
    };

    /// <summary>
    /// Add a type to the custom serializer. 
    /// </summary>
    /// <typeparam name="T">Type to be aded</typeparam>
    public static void AddSerializer<T>() where T : new()
    {
        //get the object type and check if it is already added
        Type objectType = new T().GetType();
        if (fileSerializationMap.ContainsKey(objectType))
            return;

        // create a xml serializer for file serializtion
        fileSerializationMap.Add(objectType, new XmlSerializer(objectType));
        Dictionary<string, SerializationAction> newTypeInstructions = new Dictionary<string, SerializationAction>();
        
        //grab public fields through reflection. iterat over them with LINQ
        foreach (var field in
            from field in objectType.GetFields(BindingFlags.Public | BindingFlags.Instance)
             where field.IsPublic
             select field)
        {
            // add a network serializer and deserializer for each field based on type
            if (NetworkSerializeDictionary.ContainsKey(field.FieldType))
            {
                newTypeInstructions.Add(field.Name, new SerializationAction(
                    (obj, writer) => NetworkSerializeDictionary[field.FieldType](field, obj, writer),
                    (obj, reader) => networkDeserializeDictionary[field.FieldType](field, obj, reader)));
            }
            else if (networkSerializationMap.ContainsKey(field.FieldType))
            {
                newTypeInstructions.Add(field.Name, new SerializationAction(
                    (obj, writer) => Serialize(obj, writer),
                    (obj, reader) =>
                    {
                        field.SetValue(obj, Deserialize(field.GetType(), reader));
                    }
                    ));
            }
            else
                throw new ArgumentException("Unknown serialization type \"" + field.GetType() + "\".");
        }
        networkSerializationMap.Add(objectType, newTypeInstructions);
    }

    /// <summary>
    /// Serialize the object to a <see cref="NetworkWriter"/>
    /// </summary>
    /// <typeparam name="T">Custom serialized type</typeparam>
    /// <param name="value">Values to be serialized</param>
    /// <param name="writer">Destination for the serialization</param>
    public static void Serialize<T>(T value, NetworkWriter writer)
    {
        foreach (var kvp in networkSerializationMap[value.GetType()])
        {
            kvp.Value.serializeNetwork(value, writer);
        }
    }

    /// <summary>
    /// Serialize the object to a stream
    /// </summary>
    /// <param name="obj">Values to serialize</param>
    /// <param name="stream">Stream to output the data on</param>
    public static void Serialize(object obj, Stream stream)
    {
        fileSerializationMap[obj.GetType()].Serialize(stream, obj);
        stream.Flush();
    }

    /// <summary>
    /// Deserialize an object from a a network reader
    /// </summary>
    /// <typeparam name="T">Type to be deserialized</typeparam>
    /// <param name="reader">Network reader to read from</param>
    /// <returns>Deserialized values</returns>
    public static T Deserialize<T>(NetworkReader reader) where T : new()
    {
        T toReturn = new T();
        foreach (var kvp in networkSerializationMap[toReturn.GetType()])
        {
            kvp.Value.deserializeNetwork(toReturn, reader);
        }
        return toReturn;
    }

    /// <summary>
    /// Deserialize an object from a a network reader
    /// </summary>
    /// <param name="objType">Type of the object to deserialized</param>
    /// <param name="reader">Network reader to read from</param>
    /// <returns>Deserialized values</returns>
    public static object Deserialize(Type objType, NetworkReader reader)
    {
        object toReturn = Activator.CreateInstance(objType);
        foreach (var kvp in networkSerializationMap[toReturn.GetType()])
        {
            kvp.Value.deserializeNetwork(toReturn, reader);
        }
        return toReturn;
    }

    /// <summary>
    /// Deserialize an object from a stream
    /// </summary>
    /// <typeparam name="T">Type to be deserialized</typeparam>
    /// <param name="stream">stream to read from</param>
    /// <returns>Deserialized values</returns>
    public static T Deserialize<T>(Stream stream) where T : new()
    {
        T toReturn = new T();
        return (T)(fileSerializationMap[toReturn.GetType()].Deserialize(stream));
    }

    protected class SerializationAction
    {
        public Action<object, NetworkWriter> serializeNetwork;
        public Action<object, NetworkReader> deserializeNetwork;
        public SerializationAction(
            Action<object, NetworkWriter> networkSerializer,
            Action<object, NetworkReader> networkDeserializer)
        {
            serializeNetwork = networkSerializer;
            deserializeNetwork = networkDeserializer;
        }
    }

    protected class TypeComparer : IComparer<Type>
    {
        public Int32 Compare(Type x, Type y)
        {
            return String.Compare(x.Name, y.Name);
        }
    }
}

