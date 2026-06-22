# Setup (Kurulum Paketi) Oluşturma

Bu proje **Inno Setup 6** ile Windows kurulum dosyası (`.exe`) üretir. Kurulum sonrası bilgisayarda .NET kurulu olması gerekmez.

## Gereksinimler

1. **.NET 8 SDK** — geliştirme bilgisayarında (zaten var)
2. **Inno Setup 6** — ücretsiz: https://jrsoftware.org/isdl.php

## Adım adım

### 1) Programı yayınla (publish)

Proje klasöründe:

```bat
PUBLISH.bat
```

Bu komut `publish\` klasörüne çalıştırılabilir dosyaları kopyalar (~150 MB, .NET dahil).

### 2) Setup dosyasını üret

```bat
BUILD_SETUP.bat
```

Başarılı olursa çıktı:

```text
setup\output\MsgoktasMu_Setup.exe
```

### 3) Dağıtım

`MsgoktasMu_Setup.exe` dosyasını USB, e-posta veya sunum klasörüne koyun. Kullanıcı çift tıklayıp kurulum sihirbazını izler.

## Kurulum nereye yapılır?

- Varsayılan: `%LocalAppData%\MimarSinanGoktas\MsgoktasMu`
- Veritabanı (`msgoktas_mu.db`) ve yüklenen dosyalar bu klasörde oluşur
- Program Files kullanılmaz; böylece yazma izni sorunu olmaz

## Manuel derleme (Inno Setup arayüzü)

1. `PUBLISH.bat` çalıştırın
2. Inno Setup Compiler’ı açın
3. `setup\MsgoktasMu.iss` dosyasını açın
4. **Build → Compile**
5. `setup\output\MsgoktasMu_Setup.exe` oluşur

## Sorun giderme

| Sorun | Çözüm |
|--------|--------|
| `dotnet publish` hatası | .NET 8 SDK kurulu mu kontrol edin |
| Inno Setup bulunamadı | Inno Setup 6’yı kurun, `BUILD_SETUP.bat` tekrar çalıştırın |
| Kurulumdan sonra program açılmıyor | Antivirüs engelliyor olabilir; klasörü güvenilir listeye ekleyin |
| Eski sürüm üzerine kurulum | Önce Denetim Masası → Programlar’dan eski sürümü kaldırın |

## Değerlendirme için not

Setup paketi, kaynak kod dışında ayrı bir teslim dosyasıdır. Sunumda hem kaynak projeyi hem `MsgoktasMu_Setup.exe` dosyasını gösterebilirsiniz.
