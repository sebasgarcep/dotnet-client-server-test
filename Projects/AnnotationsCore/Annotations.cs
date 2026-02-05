public class Annotations
{
    public Dictionary<string, IAnnotation> AnnotationDictionary { get; set; }

    public Annotations()
    {
        this.AnnotationDictionary = new Dictionary<string, IAnnotation>();
    }

    public T? GetAnnotation<T>() where T: IAnnotation
    {
        try
        {
            return (T) AnnotationDictionary[GetAnnotationTypeName<T>()];
        }
        catch (KeyNotFoundException)
        {
            return default;
        }
    }

    public void SetAnnotation<T>(T value) where T: IAnnotation
    {
        AnnotationDictionary[GetAnnotationTypeName<T>()] = value;
    }

    public IList<IAnnotation> GetAnnotationList()
    {
        return AnnotationDictionary.Values.ToList();
    }

    private string GetAnnotationTypeName<T>()
    {
        return typeof(T).Name;
    }
}