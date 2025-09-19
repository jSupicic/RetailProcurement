using Microsoft.AspNetCore.SignalR;

namespace Retail.Api.Hubs;

public class NotificationHub : Hub
{
    // Simple hub for broadcasting notifications.
    // Clients should subscribe to methods like:
    // - "SupplierCreated", "SupplierUpdated", "SupplierDeleted"
    // - "StoreItemCreated", "StoreItemUpdated", "StoreItemDeleted"
    // - "SupplierStoreItemCreated"
    // - "QuarterlyPlanCreated"
}