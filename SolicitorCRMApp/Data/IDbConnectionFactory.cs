using Microsoft.Data.SqlClient;

namespace SolicitorCRMApp.Data;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}
