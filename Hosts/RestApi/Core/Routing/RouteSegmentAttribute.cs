namespace CleanDDDArchitecture.Hosts.RestApi.Core.Routing;

// The RouteSegmentAttribute is a custom attribute used to define a specific segment of the route for a controller.
// This attribute allows you to set a custom name for the controller segment in the route, which can be different
// from the default controller name derived from the class name.
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class RouteSegmentAttribute : Attribute
{
    // Property to hold the custom segment string.
    public string Segment { get; private set; }

    // Constructor for the attribute. It takes a string parameter 'segment'
    // which represents the custom route segment you want to assign to the controller.
    // Example usage: [RouteSegment("customname")]
    public RouteSegmentAttribute(string segment)
    {
        Segment = segment;
    }
}