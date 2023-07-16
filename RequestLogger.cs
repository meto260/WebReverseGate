public class RequestLogger {
    TransformBuilderContext context;
    public RequestLogger(TransformBuilderContext _context) {
        context = _context;
    }

    public async Task SaveRequest() {
        var db = new SaveData();
        try {
            var clusterId = context.Cluster.ClusterId;
            var routeId = context.Route.RouteId;
            var metaDatas = context.Cluster.Metadata;
            var health = context.Cluster.HealthCheck;
            var destinations = context.Cluster.Destinations;

            await db.LogSave(new LoggerModel {
                clusterId = clusterId,
                routeId = routeId,
                metaDatas = JsonSerializer.Serialize(metaDatas),
                health = JsonSerializer.Serialize(health),
                destinations = JsonSerializer.Serialize(destinations)
            });
        }
        catch (Exception exc) {
            await db.ExceptionSave(exc.Message);
        }
    }
}