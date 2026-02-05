var annotationsManager = new AnnotationsManager();
var typeA = new AnnotationTypeA { data = "Hello world!" };
annotationsManager.SetAnnotation(typeA);
var annotationListA = annotationsManager.GetAnnotationList();
var typeB = new AnnotationTypeB { data = 100 };
annotationsManager.SetAnnotation(typeB);
var fetchedTypeB = annotationsManager.GetAnnotation<AnnotationTypeB>();
var annotationListB = annotationsManager.GetAnnotationList();

var serializedData = annotationsManager.Serialize();
Console.WriteLine(serializedData);

var deserializedAnnotations = AnnotationsManager.Deserialize(
    "{\"Annotations\":{\"AnnotationTypeA\":{\"data\":\"Hello world!\"},\"AnnotationTypeB\":{\"data\":100},\"AnnotationTypeC\":{\"data\": true}}}"
);
Console.WriteLine("Hello World!");
