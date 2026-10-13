# claude-key (ClaudeTusu)

Klavyedeki **Copilot** veya **Bağlam menüsü** tuşuna basıldığında Copilot yerine **Claude** masaüstü uygulamasını açan küçük bir Windows programı.

- Ek program kurmaya gerek yok: Windows'taki hazır C# derleyicisiyle (.NET Framework 4) derlenir.
- Pencere açmadan arka planda çalışır, yaklaşık 10–15 MB bellek kullanır.
- Seçilen tuş Windows'a iletilmez; Copilot ya da sağ tık menüsü açılmaz, yalnızca Claude açılır.
- Claude'un Microsoft Store (MSIX) sürümünü de klasik kurulumunu da destekler.

## Kurulum

1. Depoyu indirin (**Code → Download ZIP**) ve kalıcı bir klasöre çıkarın, örneğin `Belgeler\ClaudeTusu`.
   Program kurulumdan sonra bu klasörden çalışır, klasörü silmeyin.
2. Klavyenize uygun dosyaya çift tıklayın:
   - `Kur - Copilot tusu.vbs`
   - `Kur - Baglam menusu tusu.vbs`
3. "Kurulum bitti" mesajından sonra tuşa basıp deneyin.

Kurulum programı derler, `bin\ClaudeTusu.exe` dosyasını oluşturur, Windows başlangıcına ekler ve hemen başlatır.
Bir sorun olursa ayrıntılar klasördeki `kurulum_log.txt` dosyasına yazılır.

## Kaldırma

`Kaldir.vbs` dosyasına çift tıklayın. Program durdurulur ve başlangıçtan çıkarılır; ardından klasörü silebilirsiniz.

## Elle kullanım

```
bin\ClaudeTusu.exe copilot   # Copilot tuşu
bin\ClaudeTusu.exe menu      # Bağlam menüsü tuşu
```

Aynı anda yalnızca bir kopya çalışır. Durdurmak için Görev Yöneticisi'nden **ClaudeTusu** işlemini sonlandırın.

## Notlar

- Bağlam menüsü tuşu atandığında sağ tık menüsü için **Shift + F10** kullanılabilir.
- Yönetici olarak çalışan bir pencere öndeyken Windows güvenlik gereği tuşu programa iletmez; o anda tuş çalışmaz.
- Kurulum dosyaları `.vbs` olarak verilmiştir; `.bat` dosyaları bazı bilgisayarlarda düzenleyicide açılacak şekilde ayarlı olabilir.

## Nasıl çalışır?

Program düşük seviyeli bir klavye kancası (`WH_KEYBOARD_LL`) kurar:

- **Copilot tuşu** Windows'a `Win + Shift + F23` gönderir. Program `F23`'ü yakalar, Win bırakıldığında Başlat menüsü açılmasın diye araya atanmamış bir tuş (`0xE8`) basımı ekler ve Claude'u açar.
- **Bağlam menüsü tuşu** (`VK_APPS`) doğrudan yakalanır.

## Proje yapısı

```
src/ClaudeTusu.cs           Program kaynak kodu
scripts/kurulum.bat         Derleme + başlangıca ekleme
scripts/kaldir.bat          Durdurma + başlangıçtan çıkarma
Kur - *.vbs, Kaldir.vbs     Çift tıklanarak çalıştırılan başlatıcılar
.github/workflows/build.yml Her gönderimde Windows'ta derleme denetimi
```
