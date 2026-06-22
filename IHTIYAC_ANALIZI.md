# İhtiyaç Analizi — Mimar Sinan Göktaş

## 1. Firma ve Problem Tanımı

**Firma:** Mimar Sinan Göktaş — Sivas merkezli mimarlık ofisi  
**Faaliyet:** Konut/ticari proje çizimi, şantiye takibi, raporlama

### Mevcut Sorunlar
- Şantiye bilgileri dağınık tutuluyordu (telefon, adres, konum ayrı yerlerde)
- Günlük rapor, çizelge ve not dosyaları e-posta veya klasörlerle paylaşılıyordu
- Personel ve iş yapılan tarafların sisteme erişimi düzensizdi
- Teknik bilgisi sınırlı personel için karmaşık web arayüzü zorlayıcıydı

## 2. Müşteri İstekleri (Toplanan Gereksinimler)

| No | İstek | Öncelik |
|----|--------|---------|
| G1 | Birden fazla şantiye kaydı (ad, adres, telefon, konum) | Yüksek |
| G2 | Dosya paylaşımı: genel bilgi, rapor, not, çizelge | Yüksek |
| G3 | Kullanıcı girişi ve rol ayrımı (admin, personel, kullanıcı) | Yüksek |
| G4 | Adminin kullanıcı ve şifre yönetimi | Orta |
| G5 | Ofis iletişim ve harita bilgisi güncelleme | Orta |
| G6 | Kolay kullanılabilir arayüz | Yüksek |

## 3. Çözüm Kapsamı

Bu masaüstü program (`msgoktas_mu`) aynı işlevleri **bağımsız veritabanı** ile sunar:
- Web sitesi ile içerik paraleldir, veritabanları ortak değildir
- Windows ortamında tek tıkla çalıştırılabilir
- Admin paneli + ziyaretçi görünümü + personel iş takibi

## 4. Kapsam Dışı
- Kurulum/setup paketi (ayrı değerlendirme maddesi)
- Web sitesi ile canlı veri senkronizasyonu

## 5. Başarı Kriterleri
- Şantiye ekleme/silme/güncelleme hatasız çalışır
- Dosya yükleme ve açma işlemleri güvenli yapılır
- Yanlış veri girildiğinde program çökmez, uyarı verir
- Teknik bilgisi az kullanıcı menüden işlem yapabilir
