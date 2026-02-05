using System.Text.Json;

public class AnnotationsManager
{
    public Annotations Annotations { get; set; }

    public AnnotationsManager()
    {
        this.Annotations = new Annotations();
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public static AnnotationsManager Deserialize(string serializedData)
    {
        return JsonSerializer.Deserialize<AnnotationsManager>(serializedData)!;
    }

    public T? GetAnnotation<T>()
    {
        var property = GetProperty<T>();
        if (property == null) return default;
        return (T?) property.GetValue(this.Annotations);
    }

    public void SetAnnotation<T>(T value)
    {
        var property = GetProperty<T>();
        if (property == null) return;
        property.SetValue(this.Annotations, value);
    }

    private static System.Reflection.PropertyInfo? GetProperty<T>()
    {
        return GetProperty(typeof(T));
    }

    private static Dictionary<Type, System.Reflection.PropertyInfo?> PropertyInfoCache
        = new Dictionary<Type, System.Reflection.PropertyInfo?>();
    private static System.Reflection.PropertyInfo? GetProperty(Type typeVariable)
    {
        try
        {
            return PropertyInfoCache[typeVariable];
        }
        catch (KeyNotFoundException)
        {
            var property = typeof(Annotations).GetProperty(typeVariable.Name);
            PropertyInfoCache[typeVariable] = property;
            return property;
        }
    }

    private static List<System.Reflection.PropertyInfo>? AnnotationProperties;

    public IList<object> GetAnnotationList()
    {
        if (AnnotationProperties == null)
        {
            AnnotationProperties = [..typeof(Annotations).GetProperties()];
        }

        var result = new List<object>();
        foreach (var annotationProperty in AnnotationProperties)
        {
            var property = GetProperty(annotationProperty.PropertyType);
            if (property == null) continue;
            var item = property.GetValue(this.Annotations);
            if (item == null) continue;
            result.Add(item);
        }
        return result;
    }
}