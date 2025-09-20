import { Injectable, OnDestroy } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { StoreItem } from './store-items.service';
import { Supplier } from './suppliers.service';
import { SupplierStoreItem } from './supplier-store-items.service';
import { QuarterlyPlan } from './statistics.service';
import { environment } from '../../../environments/environment';

export interface StoreItemNotification {
  type: 'created' | 'updated' | 'deleted';
  item: StoreItem;
}

export interface SupplierNotification {
  type: 'created' | 'updated' | 'deleted';
  supplier: Supplier;
}

export interface SupplierStoreItemNotification {
  type: 'created' | 'updated' | 'deleted';
  item: SupplierStoreItem;
}

export interface QuarterlyPlanNotification {
  type: 'created' | 'updated' | 'deleted';
  plan: QuarterlyPlan;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService implements OnDestroy {
  private hubConnection: HubConnection | null = null;
  private connectionStateSubject = new BehaviorSubject<HubConnectionState>(HubConnectionState.Disconnected);
  private storeItemNotificationsSubject = new Subject<StoreItemNotification>();
  private supplierNotificationsSubject = new Subject<SupplierNotification>();
  private supplierStoreItemNotificationsSubject = new Subject<SupplierStoreItemNotification>();
  private quarterlyPlanNotificationsSubject = new Subject<QuarterlyPlanNotification>();
  private destroy$ = new Subject<void>();

  private readonly hubUrl = environment.signalRUrl;

  constructor() {
    this.initializeConnection();
  }

  private initializeConnection(): void {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl)
      .withAutomaticReconnect()
      .build();

    this.setupEventHandlers();
  }

  private setupEventHandlers(): void {
    if (!this.hubConnection) return;

    // Store item events
    this.hubConnection.on('StoreItemCreated', (item: StoreItem) => {
      this.storeItemNotificationsSubject.next({ type: 'created', item });
    });

    this.hubConnection.on('StoreItemUpdated', (item: StoreItem) => {
      this.storeItemNotificationsSubject.next({ type: 'updated', item });
    });

    this.hubConnection.on('StoreItemDeleted', (item: StoreItem) => {
      this.storeItemNotificationsSubject.next({ type: 'deleted', item });
    });

    // Supplier events
    this.hubConnection.on('SupplierCreated', (supplier: Supplier) => {
      this.supplierNotificationsSubject.next({ type: 'created', supplier });
    });

    this.hubConnection.on('SupplierUpdated', (supplier: Supplier) => {
      this.supplierNotificationsSubject.next({ type: 'updated', supplier });
    });

    this.hubConnection.on('SupplierDeleted', (supplier: Supplier) => {
      this.supplierNotificationsSubject.next({ type: 'deleted', supplier });
    });

    // Supplier-Store Item events
    this.hubConnection.on('SupplierStoreItemCreated', (item: SupplierStoreItem) => {
      this.supplierStoreItemNotificationsSubject.next({ type: 'created', item });
    });

    this.hubConnection.on('SupplierStoreItemUpdated', (item: SupplierStoreItem) => {
      this.supplierStoreItemNotificationsSubject.next({ type: 'updated', item });
    });

    this.hubConnection.on('SupplierStoreItemDeleted', (item: SupplierStoreItem) => {
      this.supplierStoreItemNotificationsSubject.next({ type: 'deleted', item });
    });

    // Quarterly Plan events
    this.hubConnection.on('QuarterlyPlanCreated', (plan: QuarterlyPlan) => {
      this.quarterlyPlanNotificationsSubject.next({ type: 'created', plan });
    });

    this.hubConnection.on('QuarterlyPlanUpdated', (plan: QuarterlyPlan) => {
      this.quarterlyPlanNotificationsSubject.next({ type: 'updated', plan });
    });

    this.hubConnection.on('QuarterlyPlanDeleted', (plan: QuarterlyPlan) => {
      this.quarterlyPlanNotificationsSubject.next({ type: 'deleted', plan });
    });

    // Connection state events
    this.hubConnection.onclose(() => {
      this.connectionStateSubject.next(HubConnectionState.Disconnected);
    });

    this.hubConnection.onreconnecting(() => {
      this.connectionStateSubject.next(HubConnectionState.Reconnecting);
    });

    this.hubConnection.onreconnected(() => {
      this.connectionStateSubject.next(HubConnectionState.Connected);
    });
  }

  async startConnection(): Promise<void> {
    if (!this.hubConnection) {
      this.initializeConnection();
    }

    if (this.hubConnection?.state === HubConnectionState.Disconnected) {
      try {
        await this.hubConnection.start();
        this.connectionStateSubject.next(HubConnectionState.Connected);
        console.log('SignalR connection established');
      } catch (error) {
        console.error('Error starting SignalR connection:', error);
        this.connectionStateSubject.next(HubConnectionState.Disconnected);
        throw error;
      }
    }
  }

  async stopConnection(): Promise<void> {
    if (this.hubConnection && this.hubConnection.state !== HubConnectionState.Disconnected) {
      try {
        await this.hubConnection.stop();
        this.connectionStateSubject.next(HubConnectionState.Disconnected);
        console.log('SignalR connection stopped');
      } catch (error) {
        console.error('Error stopping SignalR connection:', error);
      }
    }
  }

  getConnectionState(): Observable<HubConnectionState> {
    return this.connectionStateSubject.asObservable();
  }

  getStoreItemNotifications(): Observable<StoreItemNotification> {
    return this.storeItemNotificationsSubject.asObservable();
  }

  getSupplierNotifications(): Observable<SupplierNotification> {
    return this.supplierNotificationsSubject.asObservable();
  }

  getSupplierStoreItemNotifications(): Observable<SupplierStoreItemNotification> {
    return this.supplierStoreItemNotificationsSubject.asObservable();
  }

  getQuarterlyPlanNotifications(): Observable<QuarterlyPlanNotification> {
    return this.quarterlyPlanNotificationsSubject.asObservable();
  }

  isConnected(): boolean {
    return this.hubConnection?.state === HubConnectionState.Connected;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.stopConnection();
  }
}
