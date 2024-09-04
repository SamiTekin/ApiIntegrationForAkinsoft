using Dapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiIntegrationForAkinsoft.Entities.OrderDetail
{
    public class OrderDetailEntity
    {
        public bool error { get; set; }
        public string message { get; set; }
        public Order order { get; set; }
        public class GetCargo
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public int price { get; set; }
            public int discount { get; set; }
            public int free_shipping { get; set; }
            public object extra { get; set; }
            public int active { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class GetCurrency
        {
            public int id { get; set; }
            public string name { get; set; }
            public string code { get; set; }
            public string symbol { get; set; }
            public string buying { get; set; }
            public string selling { get; set; }
            public int is_default { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class GetDetailItem
        {
            public int id { get; set; }
            public int order_id { get; set; }
            public int user_id { get; set; }
            public int product_id { get; set; }
            public object variant_item_id { get; set; }
            public int unit_id { get; set; }
            public object campaign_id { get; set; }
            public int quantity { get; set; }
            public int vat { get; set; }
            public int discount { get; set; }
            public string buy_price { get; set; }
            public string sale_price { get; set; }
            public string vat_price { get; set; }
            public string discount_price { get; set; }
            public int vat_included { get; set; }
            public double total { get; set; }
            public int is_deleted { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public GetProduct get_product { get; set; }
            public GetUnit get_unit { get; set; }
        }

        public class GetFirstImage
        {
            public int id { get; set; }
            public int product_id { get; set; }
            public object variant_item_id { get; set; }
            public string path { get; set; }
            public string url { get; set; }
            public string alt { get; set; }
            public string name { get; set; }
            public string extension { get; set; }
            public int is_main { get; set; }
            public int is_active { get; set; }
            public int rank { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class GetPaymentType
        {
            public int id { get; set; }
            public string name { get; set; }
            public int active { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class GetProduct
        {
            public int id { get; set; }
            public object warranty_id { get; set; }
            public int unit_id { get; set; }
            public int brand_id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public object barcode { get; set; }
            public object stockcode { get; set; }
            public string description { get; set; }
            public int stock { get; set; }
            public int active { get; set; }
            public object discount { get; set; }
            public int vat { get; set; }
            public int is_deleted { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public GetFirstImage get_first_image { get; set; }
        }

        public class GetStatus
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string bg_color { get; set; }
            public string text_color { get; set; }
            public string slug { get; set; }
            public int is_default { get; set; }
            public int rank { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class GetUnit
        {
            public int id { get; set; }
            public string name { get; set; }
            public string symbol { get; set; }
            public int increase { get; set; }
            public int type { get; set; }
            public int multiplier { get; set; }
            public int is_default { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class GetUser
        {
            public int id { get; set; }
            public string name { get; set; }
            public string surname { get; set; }
            public string email { get; set; }
            public object identity_number { get; set; }
            public object date_of_birth { get; set; }
            public string phone { get; set; }
            public object gender { get; set; }
            public object tax_office { get; set; }
            public object tax_number { get; set; }
            public object company { get; set; }
            public object note { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class Order
        {
            public int id { get; set; }
            public string oid { get; set; }
            public int user_id { get; set; }
            public object transfer_address_id { get; set; }
            public object invoice_address_id { get; set; }
            public object user_card_id { get; set; }
            public string note { get; set; }
            public int status_id { get; set; }
            public int payment_type_id { get; set; }
            public int currency_id { get; set; }
            public int cargo_id { get; set; }
            public int cargo_price { get; set; }
            public object cargo_number { get; set; }
            public object tracking { get; set; }
            public int interest_amount { get; set; }
            public double total { get; set; }
            public double discount_total { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public GetUser get_user { get; set; }
            public GetStatus get_status { get; set; }
            public GetCurrency get_currency { get; set; }
            public GetPaymentType get_payment_type { get; set; }
            public object get_transfer_address { get; set; }
            public object get_invoice_address { get; set; }
            public List<GetDetailItem> get_detail_items { get; set; }
            public List<object> get_coupons { get; set; }
            public GetCargo get_cargo { get; set; }
            public object get_payment { get; set; }
        }
    }
    
}
