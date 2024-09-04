using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiIntegrationForAkinsoft.Entities.Order
{
    public class OrderEntity
    {
       
            public bool error { get; set; }
            public Orders orders { get; set; }
            public object status_ids { get; set; }

        public class Orders
        {
            public int current_page { get; set; }
            public Datum[] data { get; set; }
            public string first_page_url { get; set; }
            public int from { get; set; }
            public int last_page { get; set; }
            public string last_page_url { get; set; }
            public Link[] links { get; set; }
            public object next_page_url { get; set; }
            public string path { get; set; }
            public int per_page { get; set; }
            public object prev_page_url { get; set; }
            public int to { get; set; }
            public int total { get; set; }
        }

        public class Datum
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
            public float total { get; set; }
            public float discount_total { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public Get_User get_user { get; set; }
            public Get_Status get_status { get; set; }
            public object[] get_coupons { get; set; }
            public Get_Currency get_currency { get; set; }
            public Get_Cargo get_cargo { get; set; }
            public Get_Payment_Type get_payment_type { get; set; }
            public object get_transfer_address { get; set; }
            public object get_invoice_address { get; set; }
            public Get_Items[] get_items { get; set; }
        }

        public class Get_User
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

        public class Get_Status
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

        public class Get_Currency
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

        public class Get_Cargo
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

        public class Get_Payment_Type
        {
            public int id { get; set; }
            public string name { get; set; }
            public int active { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
        }

        public class Get_Items
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
            public float total { get; set; }
            public int is_deleted { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public Get_Product_Basic get_product_basic { get; set; }
            public Get_Unit get_unit { get; set; }
        }

        public class Get_Product_Basic
        {
            public int id { get; set; }
            public string barcode { get; set; }
            public string stockcode { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
            public Get_First_Image get_first_image { get; set; }
        }

        public class Get_First_Image
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

        public class Get_Unit
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

        public class Link
        {
            public string url { get; set; }
            public string label { get; set; }
            public bool active { get; set; }
        }

    }
}
