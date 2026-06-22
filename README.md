# Mimar Sinan Göktaş — Masaüstü (msgoktas_mu)

Şantiye takip programı — web sitesiyle aynı işlev, **ayrı veritabanı**.

## Belgeler
- [İhtiyaç Analizi](IHTIYAC_ANALIZI.md) — değerlendirme kriteri 1
- [Kullanım Kılavuzu](KULLANIM_KILAVUZU.md) — değerlendirme kriteri 6

## Çalıştırma (geliştirme)
```bat
BASLAT.bat
```

## Setup (kurulum paketi)
Kullanıcıya dağıtılacak `.exe` kurulum dosyası için: **[SETUP_KILAVUZU.md](SETUP_KILAVUZU.md)**

Kısa özet:
1. `PUBLISH.bat` — yayın dosyalarını üretir
2. `BUILD_SETUP.bat` — `setup\output\MsgoktasMu_Setup.exe` oluşturur (Inno Setup 6 gerekir)

## Admin
- Kullanıcı: `admin`
- Şifre: `admin123`

## Özellikler
- Çoklu şantiye yönetimi, dosya paylaşımı (4 kategori)
- Rol tabanlı giriş (admin / personel / kullanıcı)
- Menü çubuğu, yardım ekranı, giriş doğrulama ve hata uyarıları

## Veritabanı
- `MsgoktasMu/msgoktas_mu.db` — web sitesinden bağımsız
