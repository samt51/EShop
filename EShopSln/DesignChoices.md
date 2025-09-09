1) Genel Bakış

Alan: E-ticaret mikroservisleri

Servisler: Catalog, Basket, Order, Payment

Mimarî: Stateless mikroservisler, her servis kendi veritabanına sahip ve bağımsız ölçeklenebilir.

2) Mimarî Yaklaşım ve Kalıplar

Onion Architecture: Order dışındaki tüm servislerde (Catalog, Basket, Payment) katmanlı, bağımlılıkların merkeze doğru azaldığı yapı.

DDD (Domain-Driven Design): Order servisinde rich domain modeli, aggregate/Value Object, domain event’leri ve sınırlandırılmış bağlamlar.

CQRS + MediatR: Komut (write) ve sorgu (read) sorumluluklarının ayrılması; istek akışında MediatR ile handler tabanlı orchestrasyon.

3) Durum Yönetimi ve Dayanıklılık

Stateless Servisler: Oturum/veri durumu dış bileşenlerde; servisler yatayda çoğaltılabilir.

Veritabanı Ayrımı: Her servisin kendi PostgreSQL veritabanı; schema izolasyonu ve bağımsız yaşam döngüsü.

Otomatik Migration: EF Core migration’ları uygulama başlangıcında çalıştırılarak şema uyumu garanti edilir.


4) İletişim ve Entegrasyon

Sync: Servislerin kendi REST API’leri (Swagger ile dokümante).

Async: RabbitMQ + MassTransit ile mesajlaşma; servisler arası gevşek bağ, geri basınç ve yeniden deneme kolaylığı.

Health: Servislerin healt servisleri de var.

SAGA Orkestrasyonu: Dağıtık işlem akışları için SAGA pattern; özellikle ödeme–sipariş tutarlılığı için kullanılıyor.

Ödeme başarılı → Payment bir event yayımlar, Order tüketir ve sipariş durumunu ilerletir.

Hem ödeme hem sipariş başarılı → OrderPaidEvent ile paketleme/e-posta gibi alt süreçler tetiklenir.

Olumsuz senaryoda → RefundPaymentCommand ile iade akışı devreye alınır.

5) Gözlemlenebilirlik ve Loglama

Serilog: Tüm servislerde yapılandırılmış loglar (istek günlüğü, korelasyon kimlikleri).

Log Depolama: Servislerin kendi veritabanlarında/uygun sink’lerde loglanır (ihtiyaca göre PostgreSQL/console).

6) API Gateway

Ocelot API Gateway: Tek giriş noktası, upstream/downstream yönlendirme, ortak URL yönetimi ve ileride rate-limit, auth vb. politikalar için kapı.

7) Konteynerleşme ve Yerel Çalışma

Docker Compose: Her servis, RabbitMQ dâhil, compose ile ayağa kalkar; tek komutla tutarlı lokal ortam.

8) CI/CD

Jenkins Pipeline: Build, test, containerize ve dağıtım adımları otomatik tetiklenir; kod değişikliklerinde güvenilir teslim.

9) Tercihlerin Gerekçeleri (Trade-offs)

Mikroservis + Ayrık Veritabanları: Otonomi ve bağımsız ölçeklenebilirlik ↔ raporlama/küresel transaction karmaşıklığı.

CQRS: Net sorumluluk ayrımı ve performans ↔ ek operasyonel karmaşıklık.

SAGA: Tutarlılık ve hata toleransı ↔ durum takibi ve izlenebilirlik gereksinimi artar.

Onion + DDD: Test edilebilir ve esnek tasarım ↔ öğrenme eğrisi ve başlangıç maliyeti.

10) Bilinen Sınırlar ve İyileştirme Fırsatları

Observability Derinliği: Dağıtık izleme için OpenTelemetry/Tracing (Jaeger/Grafana) eklenebilir.

Güvenlik: Gateway tarafında JWT, rate-limiting ve mTLS politikaları devreye alınabilir.

Dayanıklılık Paternleri: Outbox/Inbox, idempotency key’leri ve circuit-breaker (Polly) sistematikleştirilebilir.

Schema Evrimi: Versiyonlama ve backward-compatible migration stratejileri dokümante edilebilir.
