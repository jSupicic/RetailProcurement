# SignalR Integration for Store Items

This document describes the SignalR integration implemented for real-time updates in the store items list component.

## Overview

The application now supports real-time updates for store items through SignalR. When store items are created, updated, or deleted by any user, all connected clients will receive live notifications and see the changes immediately.

## Implementation Details

### 1. SignalR Service (`src/app/core/services/signalr.service.ts`)

The `SignalRService` manages the SignalR hub connection and provides:
- Connection lifecycle management (start/stop/reconnect)
- Real-time event listening for store item operations
- Connection state monitoring
- Automatic reconnection on connection loss

**Key Features:**
- Listens to `StoreItemCreated`, `StoreItemUpdated`, and `StoreItemDeleted` events
- Provides connection state observables
- Handles automatic reconnection
- Proper cleanup on service destruction

### 2. Store Items List Component Integration

The `ListComponent` has been enhanced to:
- Initialize SignalR connection on component load
- Listen for real-time store item notifications
- Update the UI automatically when changes occur
- Display connection status indicator
- Show toast notifications for real-time updates

**Real-time Features:**
- **Item Creation**: New items appear instantly in the list
- **Item Updates**: Modified items are updated in real-time
- **Item Deletion**: Deleted items are removed from the list
- **Connection Status**: Visual indicator shows connection state

### 3. Environment Configuration

Environment files have been created for different deployment scenarios:
- `src/environments/environment.ts` - Development configuration
- `src/environments/environment.prod.ts` - Production configuration

## Backend Requirements

The backend must have a SignalR hub (`NotificationHub`) that broadcasts the following events:

```csharp
// Store Item Events
await Clients.All.SendAsync("StoreItemCreated", storeItem);
await Clients.All.SendAsync("StoreItemUpdated", storeItem);
await Clients.All.SendAsync("StoreItemDeleted", storeItem);
```

## Installation

1. Install the SignalR package:
```bash
npm install @microsoft/signalr
```

2. The service is already configured in `app.config.ts` and ready to use.

## Usage

The SignalR integration is automatically active when the store items list component loads. No additional configuration is required.

### Connection Status Indicator

The component displays a visual indicator showing:
- 🟢 **Connected**: Real-time updates are active
- 🟡 **Reconnecting**: Attempting to reconnect
- 🔴 **Disconnected**: No real-time updates available

### Real-time Notifications

Users will see toast notifications when:
- Another user adds a new store item
- Another user updates an existing store item
- Another user deletes a store item

## Error Handling

The implementation includes comprehensive error handling:
- Connection failures are logged and users are notified
- Automatic reconnection attempts
- Graceful degradation when SignalR is unavailable
- Proper cleanup to prevent memory leaks

## Testing

To test the real-time functionality:
1. Open the store items list in multiple browser tabs/windows
2. Add, edit, or delete items in one tab
3. Observe real-time updates in other tabs

## Troubleshooting

### Common Issues

1. **Connection Failed**: Check if the backend SignalR hub is running and accessible
2. **No Real-time Updates**: Verify the backend is broadcasting the correct events
3. **CORS Issues**: Ensure the backend allows SignalR connections from the frontend domain

### Debug Information

Connection state and errors are logged to the browser console for debugging purposes.

## Future Enhancements

Potential improvements could include:
- User-specific notifications
- Conflict resolution for simultaneous edits
- Offline support with sync when reconnected
- Connection quality monitoring
- Custom notification preferences
