# Kullanım Kılavuzu — Mimar Sinan Göktaş Masaüstü Programı

## Başlatma
1. `BASLAT.bat` dosyasına çift tıklayın  
   veya `MsgoktasMu` klasöründe `dotnet run` komutunu çalıştırın
2. İlk açılışta veritabanı (`msgoktas_mu.db`) otomatik oluşur

## Varsayılan Admin Hesabı
- **Kullanıcı adı:** admin  
- **Şifre:** admin123  

---

## Ziyaretçi (Giriş Yapmadan)

### Menü çubuğu
- **Görüntüle** menüsünden sayfalar arasında gezinilir
- **Yardım → Kullanım Kılavuzu** ile bu bilgilere program içinden de ulaşılır

### Şantiyeler
1. **Şantiyeler** veya **Genel Bilgiler** bölümüne gidin  
2. Listeden şantiyeyi çift tıklayın  
3. Adres, telefon ve harita bilgilerini görün  

### Dosyalar
1. Raporlar / Notlar / Çizelge menüsünü açın  
2. Dosyaya **çift tıklayarak** bilgisayarınızda açın  

---

## Kayıt Olma
1. **Giriş / Admin** → **Kayıt Ol**  
2. Sırayla alanları doldurun:
   - **Rol:** Personel veya Kullanıcı
   - **Kullanıcı adı:** 3-32 karakter
   - **Şifre:** en az 6 karakter
   - **Telefon:** +90 5XXXXXXXXX formatında
3. Kullanıcı rolünde şantiye/kurum alanları da doldurulabilir  

---

## Personel / Kullanıcı Girişi
1. Giriş yapın  
2. **İş Takibi** ekranı açılır  
3. Günlük rapor dosyalarını listeden açabilirsiniz  

---

## Admin Paneli

Admin girişinden sonra panel açılır.

### Genel Bilgiler
- **Şantiye Bilgileri** kutusundan yeni şantiye ekleyin veya listeden seçip düzenleyin  
- **Kaydet** ile kaydedin, **Sil** ile silin  
- Alt bölümden genel dosya yükleyin  

### Dosya Sekmeleri (Rapor / Not / Çizelge)
- **Dosya Ekle** → bilgisayardan seçin  
- **Aç** → dosyayı açar  
- **Sil** → dosyayı kaldırır (onay sorulur)  

### Kullanıcılar
- Tabloda bilgileri düzenleyin  
- **Seçiliyi Kaydet** ile uygulayın  
- Son admin silinemez  

### İletişim & Harita
- E-posta, telefon, adres ve koordinatları güncelleyin  
- Enlem/boylam **sayı** olmalıdır  

---

## Hata Durumunda
Program çökmez; kırmızı uyarı veya mesaj kutusu gösterir.  
**Yardım → Hata Mesajları** sekmesine bakın.

---

## Veritabanı Notu
Bu programın veritabanı web sitesinden **ayrıdır**.  
Konum: program klasörü içinde `msgoktas_mu.db`
