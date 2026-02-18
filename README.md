# Dapper Clean Architecture Sample

A .NET 10 full-stack sample project demonstrating **Clean Architecture** principles with **Dapper ORM** on the backend and **React + TypeScript** on the frontend. Showcases best practices for building maintainable, testable, and scalable applications.

---

## Architecture Overview

```
┌─────────────────────────────────────────────────────┐
│                  UI Layer (React)                    │
│          React + TypeScript + MUI + Vite             │
└──────────────────────┬──────────────────────────────┘
                       │ HTTP (REST)
┌──────────────────────▼──────────────────────────────┐
│                   API Layer                          │
│          Minimal API Endpoints + CORS                │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│               Application Layer                      │
│       Services, DTOs, Mappings, Validation           │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│                 Domain Layer                         │
│     Entities, Value Objects, Interfaces, Enums       │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│              Infrastructure Layer                    │
│    Dapper Repositories, SQLite, Type Handlers        │
└─────────────────────────────────────────────────────┘
```

**Dependency rule:** Her katman sadece kendisinin altındaki katmana bağımlıdır. Domain katmanı hiçbir şeye bağımlı değildir.

---

## Project Structure

```
DapperCleanSample/
├── src/
│   ├── Api/                                # Sunum katmanı
│   │   ├── Endpoints/
│   │   │   ├── CustomerEndpoints.cs        # GET/POST/PUT/DELETE /api/customers
│   │   │   ├── ProductEndpoints.cs         # GET/POST/PUT/DELETE /api/products
│   │   │   ├── OrderEndpoints.cs           # CRUD + PATCH status transitions
│   │   │   ├── EndpointExtensions.cs       # MapEndpoints() extension
│   │   │   └── IEndpoint.cs                # Endpoint contract
│   │   ├── Requests/
│   │   │   ├── customers.http              # HTTP request samples
│   │   │   ├── products.http
│   │   │   └── orders.http
│   │   ├── Program.cs                      # Host builder, DI, CORS, middleware
│   │   └── appsettings.json                # Connection string (SQLite)
│   │
│   ├── Application/                        # İş kuralları orkestrasyon katmanı
│   │   ├── DTOs/
│   │   │   ├── Customer/
│   │   │   │   ├── CustomerDto.cs
│   │   │   │   ├── CreateCustomerRequest.cs
│   │   │   │   └── UpdateCustomerRequest.cs
│   │   │   ├── Product/
│   │   │   │   ├── ProductDto.cs
│   │   │   │   ├── CreateProductRequest.cs
│   │   │   │   └── UpdateProductRequest.cs
│   │   │   └── Order/
│   │   │       ├── OrderDto.cs
│   │   │       ├── OrderItemDto.cs
│   │   │       └── CreateOrderRequest.cs
│   │   ├── Mappings/
│   │   │   ├── CustomerMappingExtensions.cs
│   │   │   ├── ProductMappingExtensions.cs
│   │   │   └── OrderMappingExtensions.cs
│   │   ├── Services/
│   │   │   ├── ICustomerService.cs         # Service contracts
│   │   │   ├── CustomerService.cs
│   │   │   ├── IProductService.cs
│   │   │   ├── ProductService.cs
│   │   │   ├── IOrderService.cs
│   │   │   └── OrderService.cs
│   │   └── DependencyInjection.cs          # AddApplication() extension
│   │
│   ├── Domain/                             # Çekirdek iş mantığı
│   │   ├── Entities/
│   │   │   ├── Product.cs                  # Validasyonlu entity
│   │   │   ├── Customer.cs                 # Email VO kullanan entity
│   │   │   ├── Order.cs                    # State machine ile durum yönetimi
│   │   │   └── OrderItem.cs
│   │   ├── ValueObjects/
│   │   │   └── Email.cs                    # Immutable, regex ile validasyon
│   │   ├── Enums/
│   │   │   └── OrderStatus.cs              # Pending → Paid → Shipped → Delivered
│   │   ├── Interfaces/
│   │   │   ├── ICustomerRepository.cs
│   │   │   ├── IProductRepository.cs
│   │   │   ├── IOrderRepository.cs
│   │   │   └── IOrderItemRepository.cs
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs
│   │   │   └── EntityNotFoundException.cs
│   │   └── Common/
│   │       └── PagedResult.cs              # Generic sayfalama
│   │
│   └── Infrastructure/                     # Veri erişim implementasyonu
│       ├── Data/
│       │   ├── DapperContext.cs             # IDbConnection factory
│       │   └── DatabaseInitializer.cs       # Schema oluşturma (SQLite)
│       ├── Repositories/
│       │   ├── CustomerRepository.cs
│       │   ├── ProductRepository.cs
│       │   ├── OrderRepository.cs
│       │   └── OrderItemRepository.cs
│       ├── TypeHandlers/
│       │   └── EmailTypeHandler.cs          # Email VO ↔ string mapping
│       └── DependencyInjection.cs           # AddInfrastructure() extension
│
├── tests/
│   ├── Domain.UnitTest/                     # Domain katmanı birim testleri
│   │   ├── Entities/
│   │   │   ├── ProductTests.cs
│   │   │   ├── CustomerTests.cs
│   │   │   ├── OrderTests.cs
│   │   │   └── OrderItemTests.cs
│   │   └── ValueObjects/
│   │       └── EmailTests.cs
│   │
│   ├── Application.UnitTests/               # Servis katmanı birim testleri
│   │   └── Services/
│   │       ├── CustomerServiceTests.cs
│   │       ├── ProductServiceTests.cs
│   │       └── OrderServiceTests.cs
│   │
│   ├── Infrastructure.IntegrationTests/     # Repository entegrasyon testleri
│   │   ├── Fixtures/
│   │   │   ├── DatabaseFixture.cs           # SQLite in-memory setup
│   │   │   ├── TestDapperContext.cs
│   │   │   └── EmailTypeHandler.cs
│   │   ├── Repositories/
│   │   │   ├── CustomerRepositoryTests.cs
│   │   │   ├── ProductRepositoryTests.cs
│   │   │   ├── OrderRepositoryTests.cs
│   │   │   └── Test*Repository.cs           # Test repository implementations
│   │   └── ModuleInitializer.cs
│   │
│   └── Api.IntegrationTests/               # API endpoint entegrasyon testleri
│       ├── Fixtures/
│       │   └── ApiWebApplicationFactory.cs  # WebApplicationFactory<Program>
│       └── Endpoints/
│           ├── CustomerEndpointTests.cs
│           ├── ProductEndpointTests.cs
│           └── OrderEndpointTests.cs
│
└── ui/                                      # React frontend
    ├── src/
    │   ├── types/
    │   │   └── index.ts                     # DTO tipleri, OrderStatus, PagedResult
    │   ├── services/
    │   │   ├── apiClient.ts                 # Axios instance + interceptors
    │   │   ├── customerService.ts
    │   │   ├── productService.ts
    │   │   └── orderService.ts
    │   ├── hooks/
    │   │   └── useApi.ts                    # Generic async data fetching hook
    │   ├── components/
    │   │   ├── Layout/Layout.tsx             # Sidebar + header + Outlet
    │   │   ├── StatusBadge/StatusBadge.tsx   # Order status renkli badge
    │   │   ├── ConfirmDialog/ConfirmDialog.tsx
    │   │   └── Toast/ToastProvider.tsx       # Context-based bildirim sistemi
    │   ├── pages/
    │   │   ├── Dashboard/Dashboard.tsx       # İstatistik kartları + son siparişler
    │   │   ├── Customers/CustomerList.tsx    # CRUD + email arama + sayfalama
    │   │   ├── Products/ProductList.tsx      # CRUD + sayfalama
    │   │   └── Orders/
    │   │       ├── OrderList.tsx             # Liste + sayfalama
    │   │       ├── OrderDetail.tsx           # Detay + status workflow butonları
    │   │       └── NewOrder.tsx              # Çoklu item ile sipariş oluşturma
    │   ├── App.tsx                           # Router + ThemeProvider
    │   ├── theme.ts                          # MUI tema (indigo/violet palette)
    │   ├── index.css                         # Global stiller + Inter font
    │   └── main.tsx                          # Giriş noktası
    ├── package.json
    ├── tsconfig.json
    └── vite.config.ts
```

---

## Technology Stack

### Backend (.NET 10)

| Package | Version | Purpose |
|---------|---------|---------|
| .NET | 10.0 | Target framework |
| Dapper | 2.1.66 | Micro ORM |
| Microsoft.Data.Sqlite | — | SQLite veri tabanı |
| Scalar.AspNetCore | — | API documentation UI |

### Frontend (React)

| Package | Purpose |
|---------|---------|
| React 19 + TypeScript | UI framework |
| Vite 7 | Dev server + bundler |
| MUI (Material UI) | Bileşen kütüphanesi |
| React Router 7 | Client-side routing |
| Axios | HTTP client |

### Testing

| Package | Purpose |
|---------|---------|
| xUnit 2.9 | Test framework |
| FluentAssertions 8.8 | Akıcı assertion kütüphanesi |
| Microsoft.Data.Sqlite | In-memory test DB |
| Microsoft.AspNetCore.Mvc.Testing | API entegrasyon testleri |
| coverlet.collector | Kod kapsam raporu |

---

## API Endpoints

### Customers (`/api/customers`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/customers` | Tüm müşteriler |
| GET | `/api/customers/paged?pageNumber=1&pageSize=10` | Sayfalı liste |
| GET | `/api/customers/{id}` | ID ile getir |
| GET | `/api/customers/email/{email}` | Email ile ara |
| POST | `/api/customers` | Yeni müşteri oluştur |
| PUT | `/api/customers/{id}` | Müşteri güncelle |
| DELETE | `/api/customers/{id}` | Müşteri sil |

### Products (`/api/products`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | Tüm ürünler |
| GET | `/api/products/paged?pageNumber=1&pageSize=10` | Sayfalı liste |
| GET | `/api/products/{id}` | ID ile getir |
| POST | `/api/products` | Yeni ürün oluştur |
| PUT | `/api/products/{id}` | Ürün güncelle |
| DELETE | `/api/products/{id}` | Ürün sil |

### Orders (`/api/orders`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders/paged?pageNumber=1&pageSize=10` | Sayfalı liste |
| GET | `/api/orders/{id}` | Sipariş bilgisi |
| GET | `/api/orders/{id}/details` | Sipariş + item'lar |
| GET | `/api/orders/customer/{customerId}` | Müşteriye göre filtrele |
| POST | `/api/orders` | Yeni sipariş oluştur |
| PATCH | `/api/orders/{id}/pay` | Ödendi olarak işaretle |
| PATCH | `/api/orders/{id}/ship` | Kargoya verildi |
| PATCH | `/api/orders/{id}/deliver` | Teslim edildi |
| PATCH | `/api/orders/{id}/cancel` | İptal et |
| DELETE | `/api/orders/{id}` | Sipariş sil |

---

## Order Status Workflow

```
┌─────────┐     ┌──────┐     ┌─────────┐     ┌───────────┐
│ Pending │────▶│ Paid │────▶│ Shipped │────▶│ Delivered │
└─────────┘     └──────┘     └─────────┘     └───────────┘
     │             │
     │  ┌──────────┘
     ▼  ▼
┌───────────┐
│ Cancelled │
└───────────┘
```

| Status | Color | Transitions |
|--------|-------|-------------|
| Pending | `warning` (yellow) | → Paid, → Cancelled |
| Paid | `info` (blue) | → Shipped, → Cancelled |
| Shipped | `secondary` (purple) | → Delivered |
| Delivered | `success` (green) | Final state |
| Cancelled | `error` (red) | Final state |

---

## Key Technical Decisions

### Domain Layer
- **Encapsulated Entities:** `private set` ile korunan property'ler, validasyonlu constructor ve update metotları
- **Value Objects:** `Email` sealed record olarak tanımlı, regex ile validasyon, immutable
- **State Machine:** `Order` entity'si kontrollü durum geçişlerine sahip, geçersiz geçişler `DomainException` fırlatır

### Infrastructure Layer
- **Specific Repositories:** Generic repository yerine, her entity için özelleştirilmiş repository (Dapper felsefesine uygun)
- **Custom Type Handlers:** `EmailTypeHandler` ile Value Object ↔ DB string mapping
- **SQLite:** `DatabaseInitializer` ile uygulama başlatıldığında otomatik schema oluşturma

### Application Layer
- **Service Pattern:** Her entity için `IService` / `Service` çifti
- **Manual Mapping:** Mapping extension metotları ile Entity ↔ DTO dönüşümü (AutoMapper bağımlılığı yok)
- **DTO Segregation:** `CreateRequest`, `UpdateRequest`, `Dto` ayrımı

### API Layer
- **Minimal API:** Controller yerine `IEndpoint` pattern ile endpoint grupları
- **CORS:** `http://localhost:5173` (Vite dev server) için yapılandırılmış
- **Scalar UI:** Development ortamında `https://localhost:7142/scalar/v1` adresinde API dokümantasyonu

### Frontend (UI)
- **MUI Theme:** Indigo/violet renk paleti, Inter font, premium admin panel görünümü
- **Service Layer:** Axios instance ile merkezi API client, her entity için service modülü
- **Component Pattern:** Layout (sidebar + outlet), StatusBadge, ConfirmDialog, ToastProvider
- **Responsive:** MUI breakpoint'leri ile mobile-uyumlu sidebar

---

## Testing Strategy

```
tests/
├── Domain.UnitTest/              → Entity validasyonları, state transitions
├── Application.UnitTests/        → Service iş mantığı testleri
├── Infrastructure.IntegrationTests/ → Repository CRUD (SQLite in-memory)
└── Api.IntegrationTests/         → HTTP endpoint testleri (WebApplicationFactory)
```

### Run Tests

```bash
# Tüm testler
dotnet test

# Sadece domain birim testleri
dotnet test tests/Domain.UnitTest

# Sadece servis testleri
dotnet test tests/Application.UnitTests

# Sadece repository entegrasyon testleri
dotnet test tests/Infrastructure.IntegrationTests

# Sadece API entegrasyon testleri
dotnet test tests/Api.IntegrationTests
```

---

## Getting Started

### Prerequisites
- .NET 10 SDK
- Node.js 18+ ve npm

### Backend

```bash
# Repository'yi klonla
git clone https://github.com/CevdetTufan/DapperCleanSample.git
cd DapperCleanSample

# Paketleri yükle ve derle
dotnet restore
dotnet build

# Testleri çalıştır
dotnet test

# API'yi başlat
cd src/Api
dotnet run
# → https://localhost:7142
# → Scalar UI: https://localhost:7142/scalar/v1
```

### Frontend

```bash
# UI bağımlılıklarını yükle
cd ui
npm install

# Dev server başlat
npm run dev
# → http://localhost:5173

# Production build
npm run build
```

### Geliştirme Ortamı

Backend ve frontend'i **aynı anda** iki ayrı terminal'de çalıştırın:

| Terminal | Komut | URL |
|----------|-------|-----|
| Terminal 1 | `cd src/Api && dotnet run` | `https://localhost:7142` |
| Terminal 2 | `cd ui && npm run dev` | `http://localhost:5173` |

> **Not:** SQLite veritabanı (`app.db`) API ilk başlatıldığında otomatik oluşturulur.

---

## Design Decisions

### Why Dapper over EF Core?
- **Performance:** Read-heavy operasyonlarda daha hızlı
- **Control:** Karmaşık sorgular için tam SQL kontrolü
- **Simplicity:** Daha az soyutlama, daha öngörülebilir davranış

### Why Specific Repositories over Generic?
- **Flexibility:** Her repository'ye özgü metotlar (`GetByEmail`, `GetPagedAsync`)
- **Dapper Philosophy:** SQL'i soyutlamak yerine kucaklar
- **Type Safety:** Repository metotları için derleme zamanı kontrolü

### Why Minimal API over Controllers?
- **Less Boilerplate:** Controller, action filter, model binding attribute'ları yok
- **Modular:** `IEndpoint` pattern ile gruplu endpoint tanımı
- **Performance:** Minimal API daha düşük overhead

### Why React + MUI for Frontend?
- **Ecosystem:** Geniş bileşen kütüphanesi, hazır tema sistemi
- **TypeScript:** Backend DTO'ları ile birebir tip uyumu
- **Dev Experience:** Vite ile hızlı HMR, anında geri bildirim

---

## License

This project is open source and available under the [MIT License](LICENSE).

## Contributing

Contributions, issues, and feature requests are welcome!

---

**Created with AI-assisted development** — Clean Architecture with Dapper + React
