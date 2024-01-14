using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Diagnostics.Eventing.Reader;

namespace AracKiralamaUygulamasi
{
    class Program
    {
        public class Arac
        {
            public int Id { get; set; }
            public string Marka { get; set; }
            public string Model { get; set; }
            public string Plaka { get; set; }
            public string Kategori { get; set; }
            public bool KiralandiMi { get; set; }
            public double GunlukKiraUcreti { get; set; }
            public Arac()
            {
                KiralandiMi = false;
                this.GunlukKiraUcreti = 0; // Varsayılan değer
            }


        }
        public class Randevu
        {
            public int Id { get; set; }
            public string MusteriAdi { get; set; }
            public string AracMarka { get; set; }
            public DateTime RandevuTarihi { get; set; }
            public DateTime teslimTarihi { get; set; }
            public bool OdemeYapildiMi { get; set; }
            public double topUcret {  get; set; }
            public string AracKategori { get; set; }

        }

        public enum AracKategori
        {
            SUV,
            Sedan,
            Hatchback,
            Luxury
        }

        static List<Arac> araclar = new List<Arac>();
        static int nextAracId = 1;
        static List<Randevu> randevular = new List<Randevu>();
        static int nextRandevuId = 1;

        static void Main(string[] args)
        {
            VerileriDosyadanOku();

            Console.WriteLine("Araç Kiralama Sistemine Hoş Geldiniz!");
            Console.WriteLine("1. Yönetici Girişi");
            Console.WriteLine("2. Müşteri Girişi");
            Console.Write("Seçiminiz: ");
            string secim = Console.ReadLine();
            Console.Clear();

            switch (secim)
            {
                case "1":
                    YoneticiGirisi();
                    break;
                case "2":
                    MusteriGirisi();
                    break;
                default:
                    Console.WriteLine("Geçersiz seçim!");
                    break;
            }
        }

        static void YoneticiGirisi()
        {
            bool girisBasarili = false;

            do
            {
                Console.Write("Kullanıcı Adı: ");
                string kullaniciAdi = Console.ReadLine();
                Console.Write("Şifre: ");
                string sifre = SifreGirisi();

                if (GirisKontrol(kullaniciAdi, sifre))
                {
                    Console.Clear();
                    BaslatYoneticiAracTakipSistemi();
                    girisBasarili = true; // Doğru giriş yapıldığında döngüden çıkmak için
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Kullanıcı adı veya şifre yanlış! Lütfen tekrar deneyin.");
                }
            } while (!girisBasarili); // Eğer giriş başarılı değilse, döngü tekrar çalışacak.
        }

        static void MusteriGirisi()
        {
            Console.Clear();
            BaslatMusteriSistemi();

        }

        static bool GirisKontrol(string kullaniciAdi, string sifre)
        {
            // Basit bir kontrol; gerçekte veritabanı veya daha güvenli bir yöntem kullanılmalıdır.
            return kullaniciAdi == "admin" && sifre == "12345"; // Örnek değerler
        }

        static string SifreGirisi()
        {
            string sifre = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    sifre += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && sifre.Length > 0)
                    {
                        sifre = sifre.Substring(0, (sifre.Length - 1));
                        Console.Write("\b \b");
                    }
                }
            }
            while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return sifre;
        }

        static void BaslatYoneticiAracTakipSistemi()
        {
            while (true)
            {
                Console.WriteLine("Yönetici Arac Takip Sistemine Hoş Geldiniz!");
                Console.WriteLine("1. Araç Ekle");
                Console.WriteLine("2. Araç Sil");
                Console.WriteLine("3. Araç Güncelle");
                Console.WriteLine("4. Araçları Listele");
                Console.WriteLine("5. Randevu Oluştur");
                Console.WriteLine("6. Randevuyu Sil");
                Console.WriteLine("7. Randevu Güncelle");
                Console.WriteLine("8. Randevuları Listele");
                Console.WriteLine("9. Çıkış");
                Console.Write("Seçiminiz: ");


                string secim = Console.ReadLine();
                Console.Clear();

                switch (secim)
                {
                    case "1":
                        AracEkle();
                        break;
                    case "2":
                        AraçSil();
                        break;
                    case "3":
                        AraçGuncelle();
                        break;
                    case "4":
                        araclarıListele();
                        break;
                    case "5":
                        RandevuOluştur();
                        break;
                    case "6":
                        RandevuSil();
                        break;
                    case "7":
                        RandevuGuncelle();
                        break;
                    case "8":
                        RandevularıListele();
                        break;
                    case "9":
                        return; // Programdan çıkış
                    default:
                        Console.WriteLine("Geçersiz seçim!");
                        break;
                }
            }
        }

        static void BaslatMusteriSistemi()
        {
            while (true)
            {
                Console.WriteLine("Müşteri Arac Takip Sistemine Hoş Geldiniz!");
                Console.WriteLine("1. Randevu oluştur");
                Console.WriteLine("9. Çıkış");
                Console.Write("Seçiminiz: ");


                string secim = Console.ReadLine();
                Console.Clear();

                switch (secim)
                {
                    case "1":
                        RandevuOluştur();
                        break;
                    case "9":
                        return; // Programdan çıkış
                    default:
                        Console.WriteLine("Geçersiz seçim!");
                        break;
                }
            }
        }

        static void AracEkle()
        {
            Console.WriteLine("Yeni araç ekleyin:");

            Arac arac = new Arac();

            arac.Id = nextAracId++;

            Console.WriteLine("Lütfen aracın kategorisini seçin:");
            Console.WriteLine("1. SUV");
            Console.WriteLine("2. Sedan");
            Console.WriteLine("3. Hatchback");
            Console.WriteLine("4. Luxury");
            Console.Write("Seçiminiz (1-4): ");
            string secim = Console.ReadLine();
            Console.Clear();

            switch (secim)
            {
                case "1":
                    arac.Kategori = "SUV";
                    break;
                case "2":
                    arac.Kategori = "Sedan";
                    break;
                case "3":
                    arac.Kategori = "Hatchback";
                    break;
                case "4":
                    arac.Kategori = "Luxury";
                    break;
                default:
                    Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin.");
                    return;
            }

            Console.Write("Marka: ");
            arac.Marka = Console.ReadLine();

            Console.Write("Model: ");
            arac.Model = Console.ReadLine();

            Console.Write("Plaka (Örn: 34ABC123): ");
            arac.Plaka = Console.ReadLine();

            Console.Write("Günlük Kira Ücreti: ");
            if (double.TryParse(Console.ReadLine(), out double kiraUcreti))
            {
                arac.GunlukKiraUcreti = kiraUcreti;
            }
            else
            {
                Console.WriteLine("Geçersiz kira ücreti formatı.");
                Console.Clear();
            }

            // Kategori seçimi için seçenekleri sunalım


            araclar.Add(arac);
            Console.Clear();
            Console.WriteLine("Araç başarıyla eklendi.");
            VerileriDosyayaKaydet();
        }

        static void AraçSil()
        {
            araclarıListele2();
            Console.Write("\nSilmek istediğiniz aracın ID'sini girin: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var silinecekAraç = araclar.Find(a => a.Id == id);
                if (silinecekAraç != null)
                {
                    araclar.Remove(silinecekAraç);
                    Console.Clear();
                    Console.WriteLine("Araç başarıyla silindi.");
                    VerileriDosyayaKaydet();
                }
                else
                {
                    Console.WriteLine("Belirtilen ID ile bir araç bulunamadı.");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz ID formatı.");
            }
            Console.Clear();
        }

        static void AraçGuncelle()
        {
            // Kategori seçimi
            Console.WriteLine("Lütfen araç kategorisi seçiniz:");
            Console.WriteLine("1. SUV");
            Console.WriteLine("2. Sedan");
            Console.WriteLine("3. Hatchback");
            Console.WriteLine("4. Luxury");
            Console.Write("Seçiminiz (1-4): ");
            if (int.TryParse(Console.ReadLine(), out int kategoriSecim))
            Console.Clear();
            {
                string secilenKategori = "";
                switch (kategoriSecim)
                {
                    case 1:
                        secilenKategori = "SUV";
                        break;
                    case 2:
                        secilenKategori = "Sedan";
                        break;
                    case 3:
                        secilenKategori = "Hatchback";
                        break;
                    case 4:
                        secilenKategori = "Luxury";
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim!");
                        return;
                }

                // Seçilen kategoriye göre ve boşta olan araçları listeleyin
                List<Arac> kategoriyeAitBosAraclar = araclar.Where(a => a.Kategori == secilenKategori && !randevular.Any(r => r.AracMarka == a.Marka)).ToList();

                if (kategoriyeAitBosAraclar.Count == 0)
                {
                    Console.WriteLine("Bu kategoride boşta mevcut araç bulunmamaktadır.");
                    Console.Clear();
                    return;
                }

                Console.WriteLine("\nKategoriye ait olan araçlar ve ID'leri:");
                foreach (var arac in kategoriyeAitBosAraclar)
                {
                    string durum = arac.KiralandiMi ? "Kirada" : "Boşta";
                    Console.WriteLine($"ID: {arac.Id}, Marka: {arac.Marka}, Model: {arac.Model}, Plaka: {arac.Plaka}, Günlük Ücreti: {arac.GunlukKiraUcreti},Durum: {durum}");
                }
            }
                Console.Write("\nGüncellemek istediğiniz aracın ID'sini girin: ");
            Console.Clear();
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var guncellenecekAraç = araclar.Find(a => a.Id == id);
                if (guncellenecekAraç != null)
                {
                    Console.WriteLine("Hangi özelliği güncellemek istediğinizi seçin:");
                    Console.WriteLine("1. Marka");
                    Console.WriteLine("2. Model");
                    Console.WriteLine("3. Plaka");
                    Console.WriteLine("4. Kategori");
                    Console.WriteLine("5. Günlük Fiyat");
                    Console.Write("Seçiminiz (1-5): ");

                    string secim = Console.ReadLine();
                    Console.Clear();

                    switch (secim)
                    {
                        case "1":
                            Console.Write("Yeni Marka: ");
                            guncellenecekAraç.Marka = Console.ReadLine();
                            break;
                        case "2":
                            Console.Write("Yeni Model: ");
                            guncellenecekAraç.Model = Console.ReadLine();
                            break;
                        case "3":
                            Console.Write("Yeni Plaka: ");
                            guncellenecekAraç.Plaka = Console.ReadLine();
                            break;
                        case "4":
                            Console.Write("Yeni Kategori: ");
                            guncellenecekAraç.Kategori = Console.ReadLine();
                            break;
                        case "5":
                            Console.Write("Yeni Fiyat: ");
                            guncellenecekAraç.GunlukKiraUcreti = Convert.ToDouble(Console.ReadLine());
                            break;
                        default:
                            Console.WriteLine("Geçersiz seçim!");
                            return;
                    }

                    Console.WriteLine("Araç bilgileri başarıyla güncellendi.");
                    Console.Clear();
                    araclarıListele();
                    VerileriDosyayaKaydet();
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Belirtilen ID ile bir araç bulunamadı.");
                }
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Geçersiz ID formatı.");
            }
        }

        static Dictionary<string, List<Arac>> KategoriyeGoreAraclariGrupla()
        {
            Dictionary<string, List<Arac>> gruplar = new Dictionary<string, List<Arac>>();

            foreach (var arac in araclar)
            {
                if (!gruplar.ContainsKey(arac.Kategori))
                {
                    gruplar[arac.Kategori] = new List<Arac>();
                }
                gruplar[arac.Kategori].Add(arac);
            }

            return gruplar;
        }
        static void araclarıListele()
        {
            var grupluAraclar = KategoriyeGoreAraclariGrupla();

            Console.WriteLine("\nAraçlar Kategoriye Göre Listeleniyor:");

            foreach (var kvp in grupluAraclar)
            {
                Console.WriteLine($"\n{kvp.Key} Kategorisi:");
                foreach (var arac in kvp.Value)
                {
                    // Eğer araç kirada değilse listeye dahil et
                    if (!randevular.Any(r => r.AracMarka == arac.Marka))
                    {
                        string durum = arac.KiralandiMi ? "Kirada" : "Boşta";
                        Console.WriteLine($"ID: {arac.Id}, Marka: {arac.Marka}, Model: {arac.Model}, Plaka: {arac.Plaka}, Günlük Kira Ücreti: {arac.GunlukKiraUcreti}, Durum: {durum}");
                    }
                }
            }

            Console.WriteLine("\nAna Menüye dönmek için herhangi bir tuşa basınız.");
            Console.ReadLine();
            Console.Clear();
        }

        static void araclarıListele2()
        {
            var grupluAraclar = KategoriyeGoreAraclariGrupla();

            Console.WriteLine("\nAraçlar Kategoriye Göre Listeleniyor:");

            foreach (var kvp in grupluAraclar)
            {
                Console.WriteLine($"\n{kvp.Key} Kategorisi:");
                foreach (var arac in kvp.Value)
                {
                    // Eğer araç kirada değilse listeye dahil et
                    if (!randevular.Any(r => r.AracMarka == arac.Marka))
                    {
                        string durum = arac.KiralandiMi ? "Kirada" : "Boşta";
                        Console.WriteLine($"ID: {arac.Id}, Marka: {arac.Marka}, Model: {arac.Model}, Plaka: {arac.Plaka}, Günlük Kira Ücreti: {arac.GunlukKiraUcreti}, Durum: {durum}");
                    }
                }
            }
        }



        static void RandevuOluştur()
        {
            Console.WriteLine("\nRandevu oluşturma işlemi:");

            // Kategori seçimi
            Console.WriteLine("Lütfen araç kategorisi seçiniz:");
            Console.WriteLine("1. SUV");
            Console.WriteLine("2. Sedan");
            Console.WriteLine("3. Hatchback");
            Console.WriteLine("4. Luxury");
            Console.Write("Seçiminiz (1-4): ");
            if (int.TryParse(Console.ReadLine(), out int kategoriSecim))
                Console.Clear();
            {
                string secilenKategori = "";
                switch (kategoriSecim)
                {
                    case 1:
                        secilenKategori = "SUV";
                        break;
                    case 2:
                        secilenKategori = "Sedan";
                        break;
                    case 3:
                        secilenKategori = "Hatchback";
                        break;
                    case 4:
                        secilenKategori = "Luxury";
                        break;
                    default:
                        Console.WriteLine("Geçersiz seçim!");
                        return;
                }

                // Seçilen kategoriye göre ve boşta olan araçları listeleyin
                List<Arac> kategoriyeAitBosAraclar = araclar.Where(a => a.Kategori == secilenKategori && !randevular.Any(r => r.AracMarka == a.Marka)).ToList();


                if (kategoriyeAitBosAraclar.Count == 0)
                {
                    Console.Clear();
                    Console.WriteLine("Bu kategoride boşta mevcut araç bulunmamaktadır.");
                    return;
                }

                Console.WriteLine("\nKategoriye ait olan araçlar ve ID'leri:");
                foreach (var arac in kategoriyeAitBosAraclar)
                {
                    string durum = arac.KiralandiMi ? "Kirada" : "Boşta";
                    Console.WriteLine($"ID: {arac.Id}, Marka: {arac.Marka}, Model: {arac.Model}, Plaka: {arac.Plaka}, Günlük Ücreti: {arac.GunlukKiraUcreti},Durum: {durum}");
                }

                Console.Write("\nKiralamak istediğiniz aracın ID'sini girin: ");
                if (int.TryParse(Console.ReadLine(), out int secilenAracId))
                    
                {
                    Console.Clear();
                    var secilenArac = araclar.FirstOrDefault(a => a.Id == secilenAracId);
                    if (secilenArac != null && !secilenArac.KiralandiMi)
                    {
                        if (secilenArac.KiralandiMi)
                        {
                            Console.WriteLine("Seçilen araç zaten kirada. Lütfen farklı bir araç seçin.");
                            return; // Fonksiyondan çıkış yapar ve kullanıcıya yeni bir seçim yapmasını sağlar.
                        }
                        secilenArac.KiralandiMi = true;

                        
                        Console.Write("İsim: ");
                        string isim = Console.ReadLine();

                        Console.Write("Soyisim: ");
                        string soyisim = Console.ReadLine();

                        Console.Write("Kimlik Numarası: ");
                        string kimlikNo = Console.ReadLine();

                        Console.Write("Telefon Numarası: ");
                        string telefon = Console.ReadLine();

                        Console.Write("E-mail: ");
                        string email = Console.ReadLine();

                        // Kiralama tarihi ve süresini alalım
                        Console.Write("Kiralama Tarihi (MM/dd/yyyy formatında): ");
                        DateTime kiralamaTarihi;
                        while (!DateTime.TryParseExact(Console.ReadLine(), "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out kiralamaTarihi))
                        {
                            Console.WriteLine("Geçersiz tarih formatı. Lütfen MM/dd/yyyy formatında girin.");
                            Console.Write("Kiralama Tarihi: ");
                        }

                        Console.Write("Kaç gün kiralamak istediğinizi girin: ");
                        int kiralamaSuresi;
                        while (!int.TryParse(Console.ReadLine(), out kiralamaSuresi) || kiralamaSuresi <= 0)
                        {
                            Console.Clear();
                            Console.WriteLine("Geçersiz süre. Lütfen pozitif bir sayı girin.");
                            Console.Write("Kaç gün kiralamak istediğinizi girin: ");
                        }
                        Console.Clear();

                        // Teslim tarihini hesaplayalım
                        DateTime teslimTarihi = kiralamaTarihi.AddDays(kiralamaSuresi);

                        // Seçilen aracın günlük kira ücretini alalım
                        double gunlukKiraUcreti = secilenArac.GunlukKiraUcreti;

                        // Toplam kira ücretini hesaplayalım
                        double topUcret = gunlukKiraUcreti * kiralamaSuresi;

                        Console.WriteLine("\nToplam Tutarınız: " + topUcret);

                        // Ödeme bilgilerini alalım
                        Console.WriteLine("\nÖdeme Bilgileri:");
                        Console.Write("Kart Üzerindeki İsim: ");
                        string kartIsim = Console.ReadLine();

                        Console.Write("Kart No: ");
                        string kartNo = Console.ReadLine();

                        Console.Write("Son Kullanım Tarihi (MM/yy formatında): ");
                        string sonKullanımTarihi = Console.ReadLine();

                        Console.Write("CVV: ");
                        string cvv = Console.ReadLine();
                        Console.Clear();




                        // Mail gönderme işlemi
                        MailGonder(email, isim, soyisim, secilenArac.Marka, topUcret, teslimTarihi, kiralamaTarihi);
                        randevular.Add(new Randevu
                        {
                            Id = randevular.Count + 1, // Örnek olarak, ID'yi mevcut randevu sayısına göre belirleyelim.
                            MusteriAdi = isim + " " + soyisim,
                            AracMarka = secilenArac.Marka,
                            RandevuTarihi = kiralamaTarihi,
                            teslimTarihi = teslimTarihi,
                            OdemeYapildiMi = true // Ödemenin yapılmadığı varsayılsın, gerekirse güncellenebilir.
                        });

                        VerileriDosyayaKaydet();
                    }
                    else
                    {
                        Console.WriteLine("Geçersiz araç ID'si.");
                        Console.Clear();
                    }
                }
                else
                {
                 Console.WriteLine("Geçersiz ID formatı.");
                    Console.Clear();
                }
            }
        }




        static void MailGonder(string email, string isim, string soyisim, string aracMarka , double topUcret, DateTime teslimTarihi , DateTime kiralamaTarihi)
        {
            // Gmail üzerinden e-posta gönderimi için gerekli ayarları yapılandırın
            string senderEmail = "kiralikaractakipsistemi@gmail.com";  // Kendi e-posta adresinizi buraya yazın
            string senderPassword = "uxjr jwpm ermy gblm";      // E-posta şifrenizi buraya yazın
            string receiverEmail = email;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            mail.To.Add(receiverEmail);
            mail.Subject = "Araç Kiralama Bilgileri";
            mail.Body = $"Sayın {isim} {soyisim}, {aracMarka} marka aracınız için randevunuz oluşturulmuştur.\nÖdenen Tutar: {topUcret}.\nAracı Teslim Alma Tarihiniz: {teslimTarihi}.\nAracı Teslim Etme Tarihi: {kiralamaTarihi}.";

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpServer.EnableSsl = true;

            try
            {
                smtpServer.Send(mail);
                Console.Clear();
                Console.WriteLine("E-posta başarıyla gönderildi.");
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("E-posta gönderirken hata oluştu: " + ex.Message);
            }
        }

        /*static void ListeleMevcutRandevular()
        {
            Console.WriteLine("\nMevcut Randevular:");

            foreach (var randevu in randevular)
            {
                Console.WriteLine($"ID: {randevu.Id}, Müşteri Adı: {randevu.MusteriAdi}, Araç Marka: {randevu.AracMarka}, Randevu Tarihi: {randevu.RandevuTarihi}, Teslim Tarihi: {randevu.TeslimTarihi}");
            }

            Console.WriteLine("\nAna Menüye dönmek için herhangi bir tuşa basınız.");
            Console.ReadLine();
            Console.Clear();
        }*/

        static void RandevuGuncelle()
        {
            Console.WriteLine("\nGüncellemek istediğiniz randevunun ID'sini girin veya mevcut randevuları görüntülemek için 'listele' yazın: ");
            string giris = Console.ReadLine();

            if (giris.ToLower() == "listele")
            {
                ListeleMevcutRandevular();
                return;
            }

            if (int.TryParse(giris, out int id))
            {
                Console.Clear();

                var guncellenecekRandevu = randevular.Find(r => r.Id == id);

                if (guncellenecekRandevu != null)
                {
                    Console.WriteLine("Mevcut Randevu Bilgileri:");
                    Console.WriteLine($"ID: {guncellenecekRandevu.Id}, Müşteri Adı: {guncellenecekRandevu.MusteriAdi}, Araç Marka: {guncellenecekRandevu.AracMarka}, Randevu Tarihi: {guncellenecekRandevu.RandevuTarihi}, Teslim Tarihi: {guncellenecekRandevu.teslimTarihi}");

                    Console.WriteLine("\nGüncellenecek Bilgiyi Seçin:");
                    Console.WriteLine("1. Müşteri Adı");
                    Console.WriteLine("2. Araç Marka");
                    Console.WriteLine("3. Randevu Tarihi");
                    Console.WriteLine("4. Teslim Tarihi");

                    Console.Write("Seçiminiz (1-4): ");
                    if (int.TryParse(Console.ReadLine(), out int secim))
                    {
                        Console.Clear();

                        switch (secim)
                        {
                            case 1:
                                Console.Write("Yeni Müşteri Adı: ");
                                guncellenecekRandevu.MusteriAdi = Console.ReadLine();
                                break;
                            case 2:
                                Console.Write("Yeni Araç Marka: ");
                                guncellenecekRandevu.AracMarka = Console.ReadLine();
                                break;
                            case 3:
                                Console.Write("Yeni Randevu Tarihi (MM/dd/yyyy): ");
                                if (DateTime.TryParse(Console.ReadLine(), out DateTime tarih))
                                {
                                    guncellenecekRandevu.RandevuTarihi = tarih;
                                }
                                else
                                {
                                    Console.WriteLine("Geçersiz tarih formatı.");
                                    return;
                                }
                                break;
                            case 4:
                                Console.Write("Yeni Teslim Tarihi (MM/dd/yyyy): ");
                                if (DateTime.TryParse(Console.ReadLine(), out DateTime teslimTarihi))
                                {
                                    guncellenecekRandevu.teslimTarihi = teslimTarihi;
                                }
                                else
                                {
                                    Console.WriteLine("Geçersiz tarih formatı.");
                                    return;
                                }
                                break;
                            default:
                                Console.WriteLine("Geçersiz seçim.");
                                return;
                        }

                        Console.WriteLine("Randevu bilgileri başarıyla güncellendi.");
                    }
                    else
                    {
                        Console.WriteLine("Geçersiz seçim.");
                    }
                }
                else
                {
                    Console.WriteLine("Belirtilen ID ile bir randevu bulunamadı.");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz ID formatı.");
            }

            VerileriDosyayaKaydet();
        }

        static void ListeleMevcutRandevular()
        {
            Console.WriteLine("\nMevcut Randevular:");

            foreach (var randevu in randevular)
            {
                Console.WriteLine($"ID: {randevu.Id}, Müşteri Adı: {randevu.MusteriAdi}, Araç Marka: {randevu.AracMarka}, Randevu Tarihi: {randevu.RandevuTarihi}, Teslim Tarihi: {randevu.teslimTarihi}");
            }

            Console.WriteLine("\nAna Menüye dönmek için herhangi bir tuşa basınız.");
            Console.ReadLine();
            Console.Clear();
        }

        static void RandevuSil()
        {
            RandevularıListele();
            Console.Write("\nSilmek istediğiniz randevunun ID'sini girin: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var silinecekRandevu = randevular.Find(r => r.Id == id);
                if (silinecekRandevu != null)
                {
                    randevular.Remove(silinecekRandevu);
                    Console.Clear();
                    Console.WriteLine("Randevu başarıyla silindi.");
                    VerileriDosyayaKaydet();
                }
                else
                {
                    Console.WriteLine("Belirtilen ID ile bir randevu bulunamadı.");
                }
            }
            else
            {
                Console.WriteLine("Geçersiz ID formatı.");
            }
            Console.Clear();
        }



        static void RandevularıListele()
        {
            Console.WriteLine("\nRandevular Listeleniyor:");

            foreach (var randevu in randevular)
            {
                Console.WriteLine($"ID: {randevu.Id}, Müşteri Adı: {randevu.MusteriAdi}, Araç Marka: {randevu.AracMarka}, Randevu Tarihi: {randevu.RandevuTarihi}");
            }

            Console.WriteLine("\nAna Menüye dönmek için herhangi bir tuşa basınız.");
            Console.ReadLine();
            Console.Clear();
        }

        static void VerileriDosyayaKaydet()
        {
            using (StreamWriter sw = new StreamWriter("veriler.txt"))
            {
                // Araçları dosyaya yaz
                foreach (var arac in araclar)
                {
                    sw.WriteLine($"{arac.Id},{arac.Marka},{arac.Model},{arac.Plaka},{arac.Kategori},{arac.KiralandiMi},{arac.GunlukKiraUcreti}");
                }

                // Randevuları dosyaya yaz
                foreach (var randevu in randevular)
                {
                    sw.WriteLine($"{randevu.Id},{randevu.MusteriAdi},{randevu.AracMarka},{randevu.RandevuTarihi},{randevu.teslimTarihi},{randevu.OdemeYapildiMi},{randevu.topUcret}");
                }
            }
        }

        static void VerileriDosyadanOku()
        {
            if (File.Exists("veriler.txt"))
            {
                string[] satirlar = File.ReadAllLines("veriler.txt");

                foreach (var satir in satirlar)
                {
                    string[] parcalar = satir.Split(',');

                    if (parcalar.Length >= 7)
                    {
                        Arac yeniArac = new Arac
                        {
                            Id = int.Parse(parcalar[0]),
                            Marka = parcalar[1],
                            Model = parcalar[2],
                            Plaka = parcalar[3],
                            Kategori = parcalar[4],
                            KiralandiMi = bool.Parse(parcalar[5]),
                            GunlukKiraUcreti = double.Parse(parcalar[6])
                        };

                        araclar.Add(yeniArac);
                        nextAracId = Math.Max(nextAracId, yeniArac.Id + 1);
                    }

                    if (parcalar.Length > 7)
                    {
                        Randevu yeniRandevu = new Randevu
                        {
                            Id = int.Parse(parcalar[7]),
                            MusteriAdi = parcalar[8],
                            AracMarka = parcalar[9],
                            RandevuTarihi = DateTime.Parse(parcalar[10]),
                            teslimTarihi = DateTime.Parse(parcalar[11]),
                            OdemeYapildiMi = bool.Parse(parcalar[12]),
                            topUcret = double.Parse(parcalar[13])
                        };

                        randevular.Add(yeniRandevu);
                        nextRandevuId = Math.Max(nextRandevuId, yeniRandevu.Id + 1);
                    }
                }
            }
        }
    }
    }