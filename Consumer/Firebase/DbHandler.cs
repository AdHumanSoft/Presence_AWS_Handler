using Google.Cloud.Firestore;


using Consumer.Firebase.Schema;

namespace Consumer.Firebase;



public interface IDbHandler
{
    Task<Entities?> GetEntityAsync(string entityId);
    Task<Centers?> GetCenterAsync(string entityId, string centerId);
    Task<Members?> GetMemberAsync(string entityId, string centerId, string memberId);

    Task<Admins?> GetAdminAsync(string adminId);

    Task<Contacts?> GetContactAsync(string contactId);
}

public sealed class DbHandler : IDbHandler
{

    private readonly FirestoreDb _db;
    private readonly ILogger<DbHandler> _logger;
    public DbHandler(
        FirestoreDb db,
        ILogger<DbHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    //Get Entity async
    public async Task<Entities?> GetEntityAsync(string entityId)
    {

        var entity = await _db
        .Collection("entities")
        .Document(entityId)
        .GetSnapshotAsync();
        if (entity.Exists)
        {
            _logger.LogInformation("Entity {0} found", entityId);
            return entity.ConvertTo<Entities>();
        }
        _logger.LogInformation("Entity {0} not found", entityId);
        return null;
    }

    //Get center async

    public async Task<Centers?> GetCenterAsync(
        string entityId,
        string centerId)
    {


        var center = await _db
        .Collection(nameof(Entities).ToLower())
        .Document(entityId)
        .Collection(nameof(Centers).ToLower())
        .Document(centerId)
        .GetSnapshotAsync();
        if (center.Exists)
        {
            _logger.LogInformation("Center {0}/{1} found", entityId, centerId);
            return center.ConvertTo<Centers>();
        }

        _logger.LogInformation("Center {0}/{1} not found", entityId, centerId);
        return null;
    }


    //Get admin async
    public async Task<Admins?> GetAdminAsync(string adminId)
    {
        var admin = await _db
        .Collection(nameof(Admins).ToLower())
        .Document(adminId)
        .GetSnapshotAsync();
        if (admin.Exists)
        {
            _logger.LogInformation("Admin {0} found", adminId);
            return admin.ConvertTo<Admins>();
        }
        _logger.LogInformation("Admin {0} not found", adminId);
        return null;
    }
    //Get get member async
    public async Task<Members?> GetMemberAsync(
        string entityId,
        string centerId,
        string memberId
    )
    {
        var user = await _db
        .Collection(nameof(Entities).ToLower())
        .Document(entityId)
        .Collection(nameof(Centers).ToLower())
        .Document(centerId)
        .Collection(nameof(Members).ToLower())
        .Document(memberId)
        .GetSnapshotAsync();

        if (user is null)
        {
            _logger.LogInformation("Member {0}/{1}/{2} not found", entityId, centerId, memberId);
            return null;
        }
        _logger.LogInformation("Member {0}/{1}/{2} found", entityId, centerId, memberId);
        return user.ConvertTo<Members>();
    }

    public async Task<Contacts?> GetContactAsync(string contactId)
    {
        var contact = await  _db.Collection(nameof(Contacts).ToLower()).Document(contactId).GetSnapshotAsync();
        if (contact.Exists)
        {
            _logger.LogInformation("Contact {0} found", contactId);
            return contact.ConvertTo<Contacts>();
        }

        _logger.LogInformation("Contact {0} not found", contactId);
        return null;
    }
}

