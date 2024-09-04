using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiIntegrationForAkinsoft
{
    public class HttpContent
    {
        private const string OrderApiUrl = "https://dehapi.com/api/seller/order/get";
        private const string OrderDetailApiUrl = "https://dehapi.com/api/seller/order/detail/1";
        private const string ApiKey = "fba159cc284cfc20f34584823eac17639874b4b6f9be1a144ddd44";
        private const string ApiSecret = "fae1cc97534f0c57f98e0861807dba2568ac0173b1811ec96b8";
        
        //public async Task<OrderResponse> GetOrdersAsync()
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var request = new HttpRequestMessage(HttpMethod.Post, OrderApiUrl);
        //        request.Headers.Add("Dehasoft-Api-Key", ApiKey);
        //        request.Headers.Add("Dehasoft-Api-Secret", ApiSecret);
        //        var response = await client.SendAsync(request);
        //        response.EnsureSuccessStatusCode();
        //        return await response.Content.ReadFromJsonAsync<OrderResponse>();
        //    }

        //}
        //public async Task<OrderDetailResponse> GetOrderDetailAsync(int orderId)
        //{
        //    using (var client =new HttpClient())
        //    {
        //        var request= new HttpRequestMessage(HttpMethod.Get, $"{OrderDetailApiUrl}?orderId={orderId}");
        //        request.Headers.Add("Dehasoft-Api-Key", ApiKey);
        //        request.Headers.Add("Dehasoft-Api-Secret",ApiSecret);
        //        var response = await client.SendAsync(request);
        //        response.EnsureSuccessStatusCode();
        //        return await response.Content.ReadFromJsonAsync<OrderDetailResponse>();
        //    }
        //}
        
    }
}
