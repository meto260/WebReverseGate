public class SaveData {
    string username = "admin";
    string password = "quest";
    string database = "qdb";
    int port = 8812;
    string connectionString;
    public SaveData() {
        connectionString = $@"host=localhost;port={port};username={username};password={password}; database={database};ServerCompatibilityMode=NoTypeLoading;";

    }
    public async Task LogSave(LoggerModel input) {
        await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var sql = $"INSERT INTO Logger (createdate, clusterId, routeId, metaDatas, health, destinations) " +
            $"VALUES (now(), '{input.clusterId}', '{input.routeId}', '{input.metaDatas}', '{input.health}', '{input.destinations}')";

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@clusterId", NpgsqlDbType.Varchar, input.clusterId);
        command.Parameters.AddWithValue("@routeId", NpgsqlDbType.Varchar, input.routeId);
        command.Parameters.AddWithValue("@metaDatas", NpgsqlDbType.Varchar, input.metaDatas);
        command.Parameters.AddWithValue("@health", NpgsqlDbType.Varchar, input.health);
        command.Parameters.AddWithValue("@destinations", NpgsqlDbType.Varchar, input.destinations);
        await command.ExecuteNonQueryAsync();
    }

    public async Task ReqSave(ReqModel input) {
        await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var sql = "INSERT INTO Requests (createdate, reqId, requestMethod, requestUri, requestBody, requestHeader, requestCookies, requestBytesLength, requestContentType, requestHttps, requestTrailers, requestElapsed, clientIp) " +
            "VALUES (now(), @reqId, @requestMethod, @requestUri, @requestBody, @requestHeader, @requestCookies, @requestBytesLength, @requestContentType, @requestHttps, @requestTrailers, @requestElapsed, @clientIp)";

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@reqId", NpgsqlDbType.Uuid, input.reqId);
        command.Parameters.AddWithValue("@requestMethod", NpgsqlDbType.Varchar, input.requestMethod);
        command.Parameters.AddWithValue("@requestUri", NpgsqlDbType.Varchar, input.requestUri);
        command.Parameters.AddWithValue("@requestBody", NpgsqlDbType.Varchar, input.requestBody);
        command.Parameters.AddWithValue("@requestHeader", NpgsqlDbType.Varchar, input.requestHeader);
        command.Parameters.AddWithValue("@requestCookies", NpgsqlDbType.Varchar, input.requestCookies);
        command.Parameters.AddWithValue("@requestBytesLength", NpgsqlDbType.Bigint, input.requestBytesLength);
        command.Parameters.AddWithValue("@requestContentType", NpgsqlDbType.Varchar, input.requestContentType);
        command.Parameters.AddWithValue("@requestHttps", NpgsqlDbType.Integer, input.requestHttps ? 1 : 0);
        command.Parameters.AddWithValue("@requestTrailers", NpgsqlDbType.Integer, input.requestTrailers ? 1 : 0);
        command.Parameters.AddWithValue("@requestElapsed", NpgsqlDbType.Double, input.requestElapsed);
        command.Parameters.AddWithValue("@clientIp", NpgsqlDbType.Varchar, input.clientIp);
        await command.ExecuteNonQueryAsync();
    }

    public async Task ResSave(ResModel input) {
        await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var sql = $"INSERT INTO Responses (createdate, reqId, responseHeaders, responseCookies, responseStatus) " +
            $"VALUES (now(), @reqId, @responseHeaders, @responseCookies, @responseStatus)";

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@reqId", NpgsqlDbType.Uuid, input.reqId);
        command.Parameters.AddWithValue("@responseHeaders", NpgsqlDbType.Varchar, input.responseHeaders);
        command.Parameters.AddWithValue("@responseCookies", NpgsqlDbType.Varchar, input.responseCookies);
        command.Parameters.AddWithValue("@responseStatus", NpgsqlDbType.Integer, input.responseStatus);
        await command.ExecuteNonQueryAsync();
    }

    public async Task ExceptionSave(string input) {
        await using NpgsqlConnection connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        var sql = $"INSERT INTO Exceptions (createdate, message) " +
            $"VALUES (now(), @message)";

        await using NpgsqlCommand command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@message", NpgsqlDbType.Varchar, input);
        await command.ExecuteNonQueryAsync();
    }
}

public class LoggerModel {
    public DateTime createdate { get; set; }
    public string clusterId { get; set; }
    public string routeId { get; set; }
    public string metaDatas { get; set; }
    public string health { get; set; }
    public string destinations { get; set; }
}

public class ReqModel {
    public DateTime createdate { get; set; }
    public Guid reqId { get; set; }
    public string requestMethod { get; set; }
    public string requestUri { get; set; }
    public string requestBody { get; set; }
    public string requestHeader { get; set; }
    public string requestCookies { get; set; }
    public long requestBytesLength { get; set; }
    public string requestContentType { get; set; }
    public bool requestHttps { get; set; }
    public bool requestTrailers { get; set; }
    public string trace { get; set; }
    public double requestElapsed { get; set; }
    public string clientIp { get; set; }
}

public class ResModel {
    public DateTime createdate { get; set; }
    public Guid reqId { get; set; }
    public string responseHeaders { get; set; }
    public string responseCookies { get; set; }
    public int responseStatus { get; set; }
}