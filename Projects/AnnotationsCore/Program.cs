using System.Text.Json;

var annotations = new Annotations();
var typeA = new AnnotationTypeA { data = "Hello world!" };
annotations.SetAnnotation(typeA);
var typeB = new AnnotationTypeB { data = 100 };
annotations.SetAnnotation(typeB);

var serializedData = JsonSerializer.Serialize(annotations);

var deserializedAnnotations = JsonSerializer.Deserialize<Annotations>(serializedData);
Console.WriteLine("Hello World!");
