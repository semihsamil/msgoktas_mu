# Mimar Sinan Göktaş — Masaüstü (msgoktas_mu)

Web sitesi (`sinan_goktas-v2`) ile **aynı içerik ve işlevleri** taşıyan, ancak **tamamen ayrı veritabanı** kullanan C# WinForms uygulaması.

- Web veritabanı: `sinan_goktas-v2/database.db`
- Masaüstü veritabanı: `MsgoktasMu/msgoktas_mu.db` (ilk çalıştırmada oluşur)

## Gereksinimler

- .NET 8 SDK
- Windows

## Çalıştırma

```bat
BASLAT.bat
```

veya

```bat
cd MsgoktasMu
dotnet run
```

## Varsayılan admin

- Kullanıcı adı: `admin`
- Şifre: `admin123`

## Özellikler

- Ana sayfa, şantiye listesi, dosya görüntüleme, harita, iletişim
- Giriş / kayıt (Personel, Kullanıcı)
- Admin paneli: şantiye CRUD, dosya yükleme (4 kategori), kullanıcı yönetimi (şifre dahil), iletişim & harita ayarları
- İş takibi ekranı (personel / kullanıcı)
