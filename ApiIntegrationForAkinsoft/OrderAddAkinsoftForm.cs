using ApiIntegrationForAkinsoft.Entities.Order;
using ApiIntegrationForAkinsoft.Entities.OrderDetail;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ApiIntegrationForAkinsoft.Entities.OrderDetail.OrderDetailEntity;

namespace ApiIntegrationForAkinsoft
{
    public partial class OrderAddAkinsoftForm : Form
    {
        private const string OrderApiUrl = "https://dehapi.com/api/seller/order/get";
        private const string OrderDetailApiUrl = "https://dehapi.com/api/seller/order/detail/";
        private const string ApiKey = "sami";
        private const string ApiSecret = "sami";

        public OrderAddAkinsoftForm()
        {
            InitializeComponent();
        }
        public async Task<OrderEntity.Orders> GetOrdersAsync(int sayfaNo, int sayfaBoyut)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, OrderApiUrl);
                    request.Headers.Add("Dehasoft-Api-Key", ApiKey);
                    request.Headers.Add("Dehasoft-Api-Secret", ApiSecret);
                    var content = new StringContent($"{{\"page\": {sayfaNo}, \"limit\": {sayfaBoyut}}}", Encoding.UTF8, "application/json");
                    request.Content = content;
                    var response = await client.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"API isteği başarısız oldu. Durum Kodu: {response.StatusCode}");
                        return null;
                    }
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("API Yanıtı:\n" + responseBody);

                    var orderResponse = JsonConvert.DeserializeObject<OrderEntity>(responseBody);
                    return orderResponse.orders;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"API isteği sırasında bir hata oluştu: {ex.Message}");
                return null;
            }
        }
        public async Task<List<OrderEntity.Datum>> GetAllOrderAsync()
        {
            var allOrder=new List<OrderEntity.Datum>();
            int sayfaNo = 1;
            int sayfaBoyut = 10;
            bool result = true;
            while (result)
            {
                var orders = await GetOrdersAsync(sayfaNo, sayfaBoyut);
                if(orders != null && orders.data != null)
                {
                    allOrder.AddRange(orders.data);
                    if (orders.data.Count() < sayfaBoyut)
                    {
                        result = false;
                    }
                    else
                    {
                        sayfaNo++;
                    }
                }
                else
                {
                    result = false;
                }

            }
            return allOrder;
        }

        private async Task<(int newBLKODU, string CariKodu)> ProcessOrdersUserAsync(OrderEntity.Datum order)
        {
            string newCariKodu = null;
            int newBLKODU = 0;
            try
            {
                using (var connection = ConnectToSql.SqlConnection())
                {
                    // Öncelikle kullanıcıyı kontrol et
                    var existingUser = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT TOP 1 BLKODU, CARIKODU FROM CARI WHERE [E_MAIL]=@Email",
                        new { Email = order.get_user.email });

                    if (existingUser != null)
                    {
                        // Kullanıcı varsa, BLKODU ve CARIKODU'yu al
                        newBLKODU = (int)existingUser.BLKODU;
                        newCariKodu = existingUser.CARIKODU;
                    }
                    else
                    {
                        // Kullanıcı yoksa, yeni BLKODU ve CARIKODU oluştur ve ekle
                        newCariKodu = await GetNextCariKodu(connection);
                        newBLKODU = await connection.QuerySingleAsync<int>(@"
                    DECLARE @GEN_VALUE2 BIGINT;
                    EXECUTE SP_GEN_ID @GEN_NAME='CARI_GEN', @INCREMENT=0, @GEN_VALUE=@GEN_VALUE2 OUTPUT;
                    DECLARE @LOOP INTEGER;
                    SET @LOOP = 0;
                    WHILE (@LOOP < 1)
                    BEGIN
                        SELECT @GEN_VALUE2 = NEXT VALUE FOR CARI_GEN;
                        SET @LOOP = @LOOP + 1;
                    END
                    SELECT @GEN_VALUE2;
                ");

                        var insertSql = @"INSERT INTO CARI ([CARIKODU],[TICARI_UNVANI], [BLKODU], [E_MAIL], [ADI], [SOYADI], [ADRESI_1], [KAYIT_TARIHI],
                [ISKONTO],[PAZARLAMA_KULLAN],[RISKLI_MUSTERI],
                [DOVIZ_KULLAN],[DOVIZ_BIRIMI] ,[KREDI_LIMIT_UYARI] ,[KREDI_LIMITI_CEKSENET],[VADE_KULLAN],[MUHKODU_ALIS],[MUHKODU_SATIS],[STOK_FIYATI]
                ,[YASLANDIRMA_GRUBU],[UYRUGU],[KAYDEDEN],[SILINDI],[GARSON],[AKTIF],[SERVIS_PERSONELI] ,[ISK1_KULLAN],[ISK2_KULLAN],[ISK3_KULLAN],[EFATURA_KULLAN],[KAREKOD_TRSALAN]
                ,[ALICI_GRUBU],[TAKSITLI_MUSTERI],[EFATURA_GONDTIPI],[EFATURA_HESAP] ,[EIRSALIYE_KULLAN],[SEVK_ADRESI_FATURADAN_AL])
                VALUES (@CARIKODU,@TICARI_UNVANI, @BLKODU, @Email , @Name, @Surname, @Address, @CreatedAt, 0,0,0,1,'$',0,0,0,120,320,1,1,'T.C.','sa',0,0,1,0,0,0,0,0,0,1,0,0,0,0,0)";
                        await connection.ExecuteAsync(insertSql, new
                        {
                            CARIKODU = newCariKodu,
                            BLKODU = newBLKODU,
                            Email = order.get_user.email,
                            Name = order.get_user.name,
                            Surname = order.get_user.surname,
                            TICARI_UNVANI = order.get_user.name + ' ' + order.get_user.surname,
                            Address = order.get_invoice_address?.ToString(),
                            CreatedAt = DateTime.ParseExact(order.created_at, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                        });
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
            return (newBLKODU, newCariKodu);

        }
        private async Task ProcessOrderDetailAsync(OrderEntity.Get_Items detailItem, int newBLKODU, int siparisBlKodu ,int stokBlKodu)
        {
            if(siparisBlKodu==0)
                { return; }
            int orderDetailBLKODU = 0;
            try
            {
                using (var connection = ConnectToSql.SqlConnection())
                {
                    orderDetailBLKODU = await connection.QuerySingleAsync<int>(@"
                    DECLARE @GEN_VALUE2 BIGINT;
                    EXECUTE SP_GEN_ID @GEN_NAME='SIPARISHR_GEN', @INCREMENT=0, @GEN_VALUE=@GEN_VALUE2 OUTPUT;
                    DECLARE @LOOP INTEGER;
                    SET @LOOP = 0;
                    WHILE (@LOOP < 1)
                    BEGIN
                        SELECT @GEN_VALUE2 = NEXT VALUE FOR SIPARISHR_GEN;
                        SET @LOOP = @LOOP + 1;
                    END
                    SELECT @GEN_VALUE2;
                ");
 
                    var insertSiparisSql = @"
                INSERT INTO [SIPARISHR]([BLKODU],[BLMASKODU],[BLSTKODU],[STOKKODU],[STOK_ADI],[BIRIMI],[BIRIMI_2] ,[MIKTARI],[MIKTARI_2],[KPB_FIYATI],[KPB_KDV_HARICFY],[KPB_IND_FIYAT],[KPB_ARA_TUTAR],[KPB_IND_TUTAR]
                ,[BIRIM_CARPANI],[ISK_ORAN_CARI],[ISK_TUTAR_CARI],[KDV_ORANI],[KDV_TUTARI]
                ,[BARKODU],[ANA_STOKKODU]
                ,[DEPO_ADI],[DVZ_FIYATI]
      ,[DVZ_KDV_HARICFY]
      ,[DVZ_IND_FIYAT]
      ,[DVZ_ARA_TUTAR]
      ,[DVZ_IND_TUTAR] ,[DOVIZ_BIRIMI],[ISK_SNTUTAR_CARI]
      ,[ISK_ORAN_1]
      ,[ISK_TUTAR_1]
      ,[ISK_SNTUTAR_1]
      ,[ISK_ORAN_2]
      ,[ISK_TUTAR_2]
      ,[ISK_SNTUTAR_2]
      ,[ISK_ORAN_3]
      ,[ISK_TUTAR_3]
      ,[ISK_SNTUTAR_3]
      ,[ISK_ORAN_STOK]
      ,[ISK_TUTAR_STOK]
      ,[ISK_OZEL]
      ,[ISK_SNTUTAR_OZEL]
      ,[ISK_ARATUTAR],[KPB_TOPLAM_TUTAR]
      ,[KPB_KDVLI_TUTAR]
      ,[DVZ_TOPLAM_TUTAR]
      ,[DVZ_KDVLI_TUTAR],[ISK_SNTUTAR_STOK],[VADE_GUNU],[KPBDVZ],[ISK_TUTAR_CARI_DVZ]
      ,[ISK_SNTUTAR_CARI_DVZ]
      ,[ISK_TUTAR_1_DVZ]
      ,[ISK_SNTUTAR_1_DVZ]
      ,[ISK_TUTAR_2_DVZ]
      ,[ISK_SNTUTAR_2_DVZ]
      ,[ISK_TUTAR_3_DVZ]
      ,[ISK_SNTUTAR_3_DVZ]
      ,[ISK_TUTAR_STOK_DVZ]
      ,[ISK_OZEL_DVZ]
      ,[ISK_SNTUTAR_OZEL_DVZ]
      ,[ISK_SNTUTAR_STOK_DVZ],[MIKTARI_KALAN],[KDV_DURUMU] ,[SIRA_NO],[KPB_FIYATI_2]
      ,[SERINO_KULLAN] ,[BLEKOZELLIK_KODU]
      ,[DVZ_FIYATI_2],[STOK_BLOKE],[DEPOZITO_BLSTKODU],[DOVIZ_ALIS]
      ,[DOVIZ_SATIS],[OZEL_KODU])VALUES (@BLKODU,@BLMASKODU,@BLSTKODU,@STOKKODU,@STOK_ADI,@BIRIMI,@BIRIMI_2,
                @MIKTARI,@MIKTARI_2,@KPB_FIYATI,@KPB_KDV_HARICFY,@KPB_IND_FIYAT,@KPB_ARA_TUTAR,@KPB_IND_TUTAR,@BIRIM_CARPANI,@ISK_ORAN_CARI,@ISK_TUTAR_CARI,@KDV_ORANI, @KDV_TUTARI,@BARKODU,@ANA_STOKKODU
               ,'MERKEZ',0,0,0,0,0,'$',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,10,1,1,@KPB_FIYATI_2,0,0,0,0,0,33,34,@OZEL_KODU
                )";

                    var parameters = new
                    {
                        BLKODU = orderDetailBLKODU,
                        BLMASKODU = siparisBlKodu,
                        STOKKODU = detailItem.get_product_basic.stockcode,
                        BLSTKODU = stokBlKodu,
                        STOK_ADI = detailItem.get_product_basic.name,
                        BIRIMI = detailItem.get_unit.name ?? "",
                        BIRIMI_2= detailItem.get_unit.name ?? "",
                        MIKTARI = detailItem.quantity,
                        MIKTARI_2 = detailItem.quantity,
                        KPB_FIYATI = detailItem.sale_price,
                        KPB_FIYATI_2 = detailItem.sale_price,
                        KPB_KDV_HARICFY = detailItem.sale_price,
                        KPB_IND_FIYAT = detailItem.discount_price,
                        KPB_ARA_TUTAR = detailItem.sale_price,
                        KPB_IND_TUTAR = detailItem.buy_price,
                        BIRIM_CARPANI = detailItem.get_unit.multiplier,
                        ISK_ORAN_CARI = detailItem.discount,
                        ISK_TUTAR_CARI = detailItem.discount_price,
                        KDV_ORANI = detailItem.vat,
                        KDV_TUTARI = detailItem.vat_price,
                       
                        BARKODU = detailItem.get_product_basic.barcode,
                        ANA_STOKKODU = detailItem.get_product_basic.stockcode,
                        OZEL_KODU= stokBlKodu
                    };

                    await connection.ExecuteAsync(insertSiparisSql, parameters);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }
        private async Task<int> ProcessOrdersAsync(OrderEntity.Datum data, OrderEntity.Orders order, int newBLKODU, string newCariKodu)
        {
            int orderBLKODU = 0;
            try
            {
                using (var connection = ConnectToSql.SqlConnection())
                {
                    orderBLKODU = await connection.QuerySingleAsync<int>(@"
                    DECLARE @GEN_VALUE2 BIGINT;
                    EXECUTE SP_GEN_ID @GEN_NAME='SIPARIS_GEN', @INCREMENT=0, @GEN_VALUE=@GEN_VALUE2 OUTPUT;
                    DECLARE @LOOP INTEGER;
                    SET @LOOP = 0;
                    WHILE (@LOOP < 1)
                    BEGIN
                        SELECT @GEN_VALUE2 = NEXT VALUE FOR SIPARIS_GEN;
                        SET @LOOP = @LOOP + 1;
                    END
                    SELECT @GEN_VALUE2;
                ");
                    var insertSiparisSql = @"
                INSERT INTO [SIPARIS]([BLKODU] ,[BLCRKODU],[SIPARIS_TURU] , [CARIKODU],[TICARI_UNVANI],[ADI_SOYADI], [VERGI_NO],[VERGI_DAIRESI],[TEL1], [ADRESI],
 [OZEL_KODU],  [SIPARIS_NO],  [TARIHI], [ACIKLAMA],  [SEVK_ADRESI],[TOPLAM_GENEL_KPB], [DEGISTIRME_TARIHI],
                    [SIPARIS_DURUMU],[CEP_TEL],[E_MAIL],[KUL_STOK_FIYATI],[VADESI],[KDV_DURUMU],[KDV_ORANI],[ISK_KUL_CARI]
      ,[ISK_ORAN_CARI]
      ,[ISK_TUTAR_CARI]
      ,[ISK_KUL_1]
      ,[ISK_ORAN_1]
      ,[ISK_TUTAR_1]
      ,[ISK_KUL_2]
      ,[ISK_ORAN_2]
      ,[ISK_TUTAR_2]
      ,[ISK_KUL_3]
      ,[ISK_ORAN_3]
      ,[ISK_TUTAR_3]
      ,[ISK_KUL_STOK]
      ,[ISK_TUTAR_STOK]
      ,[ISK_KUL_OZEL]
      ,[ISK_TUTAR_OZEL]
      ,[ISK_KUL_ALT]
      ,[ISK_ORAN_ALT]
      ,[ISK_TUTAR_ALT1]
      ,[ISK_TUTAR_ALT2]
      ,[DOVIZ_KULLAN]
      ,[DOVIZ_BIRIMI]
      ,[KPBDVZ]
      ,[PAZ_DURUMU],[TOPLAM_ALT_KPB]
      ,[TOPLAM_ISK_KPB]
      ,[TOPLAM_ARA_KPB]
      ,[YUVARLAMA_KPB]
      ,[TOPLAM_KDV_KPB] ,[MIKTAR1_TOPLAM]
      ,[MIKTAR2_TOPLAM],[TOPLAM_ALT_DVZ]
      ,[TOPLAM_ISK_DVZ]
      ,[TOPLAM_ARA_DVZ]
      ,[YUVARLAMA_DVZ]
      ,[TOPLAM_KDV_DVZ]
      ,[TOPLAM_GENEL_DVZ]
      ,[TOPLAM_ISK_STOK]
      ,[TOPLAM_ISK_FAT]
      ,[TOPLAM_ISK_YUZDESI],[KAYDEDEN],[VADE_DURUMU],[SILINDI]
      ,[ISK_TUTAR_CARI_DVZ]
      ,[ISK_TUTAR_1_DVZ]
      ,[ISK_TUTAR_2_DVZ]
      ,[ISK_TUTAR_3_DVZ]
      ,[ISK_TUTAR_STOK_DVZ]
      ,[ISK_TUTAR_OZEL_DVZ]
      ,[ISK_TUTAR_ALT1_DVZ]
      ,[ISK_TUTAR_ALT2_DVZ]
      ,[TOPLAM_ISK_STOK_DVZ]
      ,[TOPLAM_ISK_FAT_DVZ]
      ,[PAZ_URUN_TUTARI_DVZ]
      ,[PAZ_ISC_TUTARI_DVZ]
      ,[YAZDIRILDI],[ONODEMELI_SATIS],[YUVARLAMA_KULLAN])VALUES (@BLKODU,@BLCRKODU, 1, @CARIKODU,@TICARI_UNVANI, @ADI_SOYADI,
                    @VERGI_NO,@VERGI_DAIRESI,@TEL1,@ADRESI,@OZEL_KODU,@SIPARIS_NO,@TARIHI,@ACIKLAMA,@SEVK_ADRESI,@TOPLAM_GENEL_KPB,@DEGISTIRME_TARIHI,@SIPARIS_DURUMU,
                    @CEP_TEL,
                    @E_MAIL,1,@VADESI,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,'$',0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,'sa',0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0
                )";
                    var parameters = new
                    {
                        BLKODU = orderBLKODU,
                        BLCRKODU = newBLKODU,
                        CARIKODU = newCariKodu,
                        TICARI_UNVANI = data.get_user.name,
                        ADI_SOYADI = data.get_user.name + " " + data.get_user.surname,
                        VERGI_NO = data.get_user.tax_number ?? "",
                        VERGI_DAIRESI = data.get_user.tax_office ?? "",
                        TEL1 = data.get_user.phone,
                        ADRESI = data.get_invoice_address?.ToString() ?? "",
                        OZEL_KODU = data.id,
                        SIPARIS_NO = data.oid,
                        TARIHI = DateTime.ParseExact(data.created_at, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        ACIKLAMA = data.note,
                        SEVK_ADRESI = data.get_transfer_address?.ToString() ?? "",
                        DEGISTIRME_TARIHI = DateTime.ParseExact(data.updated_at, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        SIPARIS_DURUMU = 1,
                        CEP_TEL = data.get_user.phone,
                        E_MAIL = data.get_user.email,
                        TOPLAM_GENEL_KPB = data.total,
                        VADESI = DateTime.ParseExact(data.created_at, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture),


                    };
                    await connection.ExecuteAsync(insertSiparisSql, parameters);
                }
                return orderBLKODU;
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Hata oluştu: {ex.Message}");
                return orderBLKODU;
            }
        }
        private async Task<int> ProcessOrderProductAsync(OrderEntity.Get_Product_Basic productBasic, int blKodu, OrderEntity.Get_Items item)
        {
            int stokBlKodu = 0;
            try
            {
                using (var connection = ConnectToSql.SqlConnection())
                {
                    string newStokKodu = await GetNextStokKodu(connection);

                    var existingProduct = await connection.QueryFirstOrDefaultAsync<dynamic>(
                        "SELECT TOP 1 BLKODU FROM STOK WHERE [OZEL_KODU1] = @id", // BLKODU'yu seçin
                        new { id = productBasic.id });

                    if (existingProduct == null)
                    {
                        // Ürün yoksa ekle
                        var insertSql = @"
                    INSERT INTO [STOK] 
                    ([STOKKODU], [OZEL_KODU1], [STOK_ADI], [ACIKLAMA1], [BLKODU]) 
                    VALUES 
                    (@STOKKODU, @OZEL_KODU1, @STOK_ADI, @ACIKLAMA1, @BLKODU)";

                        await connection.ExecuteAsync(insertSql, new
                        {
                            STOKKODU = newStokKodu,
                            OZEL_KODU1 = productBasic.id,
                            STOK_ADI = productBasic.name,
                            ACIKLAMA1 = productBasic.slug,
                            BLKODU = blKodu
                        });

                        // Fiyatı ekle
                        await InsertStokFiyatAsync(connection, item.buy_price, blKodu, 1);
                        await InsertStokFiyatAsync(connection, item.sale_price, blKodu, 2);

                        stokBlKodu = blKodu; // Yeni eklenen ürünün BLKODU'sunu döndür
                    }
                    else
                    {
                        // Ürün varsa BLKODU'sunu döndür
                        stokBlKodu =(int)existingProduct.BLKODU;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
            return stokBlKodu;
        }
        private async Task InsertStokFiyatAsync(SqlConnection connection, string fiyat, int blKodu, int alisSatis)
        {
            int newBLKODU = await GetNewStokFiyatKoduAsync(connection);
            var insertStokSql = @"
        INSERT INTO [STOK_FIYAT] 
        ([ALIS_SATIS], [FIYAT_NO], [FIYATI], [BLSTKODU], [BLKODU]) 
        VALUES 
        (@ALIS_SATIS, 1, @FIYATI, @BLSTKODU, @BLKODU)";

            await connection.ExecuteAsync(insertStokSql, new
            {
                ALIS_SATIS = alisSatis,
                FIYATI = fiyat,
                BLSTKODU = blKodu,
                BLKODU = newBLKODU
            });
        }
        private async Task<int> GetNewStokFiyatKoduAsync(SqlConnection connection)
        {
            return await connection.QuerySingleAsync<int>(@"
        DECLARE @GEN_VALUE2 BIGINT;
        EXECUTE SP_GEN_ID @GEN_NAME='STOK_FIYAT_GEN', @INCREMENT=0, @GEN_VALUE=@GEN_VALUE2 OUTPUT;
        DECLARE @LOOP INTEGER;
        SET @LOOP = 0;
        WHILE (@LOOP < 1)
        BEGIN
            SELECT @GEN_VALUE2 = NEXT VALUE FOR STOK_FIYAT_GEN;
            SET @LOOP = @LOOP + 1;
        END
        SELECT @GEN_VALUE2;
    ");
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            var allOrders = await GetAllOrderAsync();

            if (allOrders != null)
            {
                foreach (var orderData in allOrders)
                {
                    (int userBlKodu, string cariKodu) = await ProcessOrdersUserAsync(orderData);

                    using (var connection = ConnectToSql.SqlConnection())
                    {
                        var existingOrder = await connection.QueryFirstOrDefaultAsync<dynamic>(
                            "SELECT TOP 1 1 FROM SIPARIS WHERE SIPARIS_NO = @SIPARIS_NO",
                            new { SIPARIS_NO = orderData.oid });
                         
                        int siparisBlKodu;
                        if (existingOrder == null)
                         {
                            var orders = await GetOrdersAsync(1, 10);

                            siparisBlKodu = await ProcessOrdersAsync(orderData, orders, userBlKodu, cariKodu);
                            
                            if(siparisBlKodu != 0)
                            {
                                foreach (var item in orderData.get_items)
                                {
                                    int stokBlKodu=await ProcessOrderProductAsync(item.get_product_basic, userBlKodu, item);
                                    await ProcessOrderDetailAsync(item, userBlKodu, siparisBlKodu,stokBlKodu);
                                }
                            }
                        }
                        else if (existingOrder != null && existingOrder.BLKODU != null)
                        {
                            siparisBlKodu = existingOrder.BLKODU;
                            int stokBlKodu=0;
                            foreach (var item in orderData.get_items)
                            {
                                await ProcessOrderProductAsync(item.get_product_basic, userBlKodu, item);
                            }


                            foreach (var item in orderData.get_items)
                            {
                                var existingDetail = await connection.QueryFirstOrDefaultAsync<dynamic>(
                                "SELECT TOP 1 1 FROM SIPARISHR WHERE BLMASKODU = @BLMASKODU AND STOKKODU = @STOKKODU",
                                new
                                {
                                    BLMASKODU = siparisBlKodu,
                                    STOKKODU = item.get_product_basic.stockcode
                                });
                                if (existingDetail == null)
                                {
                                    await ProcessOrderDetailAsync(item, userBlKodu, siparisBlKodu,stokBlKodu);
                                }
                            }
                        }
                        
                        else
                        {
                            siparisBlKodu = 0;
                        }
                        
                    }
                }
                MessageBox.Show("Siparişler işlendi.");
            }
            else
            {
                MessageBox.Show("Sipariş alınamadı. API yanıtı boş veya beklenmeyen bir formatta.");
            }
        }
        private async Task<string> GetNextCariKodu(SqlConnection connection)
        {
            
            var maxCariKodu = await connection.QueryFirstOrDefaultAsync<string>("SELECT MAX(CARIKODU) FROM CARI");

            if (maxCariKodu == null)
            {
                return "CR00001"; // Başlangıç değeri
            }

            // Sayısal kısmı ayıkla ve arttır
            int sayisalKisim = int.Parse(maxCariKodu.Substring(2));
            int yeniSayisalKisim = sayisalKisim + 1;

            // Yeni CARIKODU değerini oluştur
            return $"CR{yeniSayisalKisim:D5}";
        }
        private async Task<string> GetNextStokKodu(SqlConnection connection)
        {
            var maxStokKodu = await connection.QueryFirstOrDefaultAsync<string>("SELECT MAX(STOKKODU) FROM STOK");
            if (maxStokKodu == null)
            {
                return "ST00001";
            }
            int sayisalKisim = int.Parse(maxStokKodu.Substring(2));
            int yeniSayisalKisim = sayisalKisim + 1;
            return $"ST{yeniSayisalKisim:D5}";
        }
    }
}
