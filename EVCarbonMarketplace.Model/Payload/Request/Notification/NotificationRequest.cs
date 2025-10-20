using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EVCarbonMarketplace.Model.Enum;
using Google.Cloud.Firestore;
namespace EVCarbonMarketplace.Model.Payload.Request.Notification
{
    [FirestoreData]
    public class NotificationRequest
    {
        [FirestoreDocumentId] public string Id { get; set; } = default!;
        [FirestoreProperty] public string UserId { get; set; } = default!;
        [FirestoreProperty] public string Title { get; set; } = default!;
        [FirestoreProperty] public string Body { get; set; } = default!;
        [FirestoreProperty] public string Type { get; set; }
        [FirestoreProperty] public bool IsRead { get; set; } = false;
        [FirestoreProperty] public Timestamp CreatedAt { get; set; }

    }
}
