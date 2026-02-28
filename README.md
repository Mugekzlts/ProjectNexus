# 🚀 ProjectNexus: DevOps Toolbox API

Düzce Üniversitesi Bilgisayar Mühendisliği kapsamında geliştirilen, sistem uzmanları için tasarlanmış minimalist bir envanter yönetim API'sidir. 

## 🛠️ Kullanılan Teknolojiler
* **Backend:** .NET 10.0 / Minimal API
* **Mimari:** Dependency Injection & Service Layer
* **Konteynerleştirme:** Docker & Docker Compose
* **Dökümantasyon:** Swagger (OpenAPI)
* **Veri Yönetimi:** JSON tabanlı kalıcı dosya sistemi

## 🏗️ Proje Mimarisi
Proje, **Single Responsibility** (Tekil Sorumluluk) prensibine uygun olarak tasarlanmıştır:
- `ToolService`: Tüm veri manipülasyonu ve dosya işlemlerinden sorumlu "Motor" sınıf.
- `Program.cs`: Bağımlılıkların yönetildiği ve API rotalarının tanımlandığı "Santral".

## 🐳 Docker ile Çalıştırma
Projeyi herhangi bir ortamda ayağa kaldırmak için:

```bash
# İmajı oluştur
sudo docker build -t project-nexus .

# Konteynırı başlat
sudo docker run -d -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --name nexus-app project-nexus

API Uç Noktaları (Endpoints)
Uygulama çalıştıktan sonra Swagger arayüzüne şu adresten ulaşabilirsiniz:
http://localhost:8080/swagger/index.html

GET /tools: Tüm araçları listeler.

POST /tools: Yeni bir araç ekler.

PUT /tools/{id}: Mevcut bir aracı günceller.

DELETE /tools/{id}: Belirtilen aracı siler.

Geliştirici : Muge - Duzce Universitesi