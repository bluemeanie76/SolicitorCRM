using System.Data;

namespace SolicitorCRMApp.Data;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
