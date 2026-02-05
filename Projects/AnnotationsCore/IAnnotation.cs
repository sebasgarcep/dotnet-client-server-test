using System.Text.Json.Serialization;

[JsonDerivedType(typeof(AnnotationTypeA), typeDiscriminator: "a")]
[JsonDerivedType(typeof(AnnotationTypeB), typeDiscriminator: "b")]
public interface IAnnotation {}
