using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace CryptopayLibruary
{
    //Основной класс для работы с CryptoPay
    internal class CryptoPay
    {
        //Тут будет храниться токен Api
        public static string apiToken;


        public static bool isBotTestnet = false;


        public static HttpClient client = new HttpClient();


        public async Task<(string payLink, string invoiceId)> createPayLink(int amount, string asset, string description = "", string canSendComment = "true", string canSendAnonumous = "true")
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Crypto-Pay-API-Token", apiToken);

            //Свойства для нашей ссылки
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("asset", asset),
                new KeyValuePair<string, string>("amount", amount.ToString().Replace(',', '.')),
                new KeyValuePair<string, string>("description", description),
                new KeyValuePair<string, string>("allow_comments", canSendComment),
                new KeyValuePair<string, string>("allow_anonymous", canSendAnonumous),
            });

            string linkPost = "None";
            if (isBotTestnet == false) { linkPost = "https://pay.crypt.bot/api/createInvoice"; }
            if (isBotTestnet == true) { linkPost = "https://testnet-pay.crypt.bot/api/createInvoice"; }

            using (var response = await client.PostAsync(linkPost, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
                    var result = jsonResponse["result"];
                    return (
                        payLink: (string)result["pay_url"],
                        invoiceId: (string)result["invoice_id"]
                    );
                }
                else
                {
                    return (null, null);
                }
            }
        }



        public async Task<JObject> CheckPaymentStatus(long invoiceId)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Crypto-Pay-API-Token", apiToken);

            var content = new FormUrlEncodedContent(new[]
            {
                    new KeyValuePair<string, string>("invoice_id", invoiceId.ToString())
            });


            string linkPost = "None";
            if (isBotTestnet == false) { linkPost = "https://pay.crypt.bot/api/getInvoices"; }
            if (isBotTestnet == true) { linkPost = "https://testnet-pay.crypt.bot/api/getInvoices"; }
            

            using (var response = await client.PostAsync(linkPost, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
                    return jsonResponse;
                }
                else
                {
                    return null;
                }
            }

        }



        public async Task<(string incomeCheck, string checkId)> createIncomeLink(string asset, int amount)
        {
            client.BaseAddress = new Uri("https://testnet-pay.crypt.bot/");
            client.DefaultRequestHeaders.Add("Crypto-Pay-API-Token", apiToken);


            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("asset", asset),
                new KeyValuePair<string, string>("amount", amount.ToString().Replace(",", ".")),
            });

            //Определяем тип CryptoApi
            string linkPost = "None";
            if (isBotTestnet == false) { linkPost = "https://pay.crypt.bot/api/createCheck"; }
            if (isBotTestnet == true) { linkPost = "https://testnet-pay.crypt.bot/api/createCheck"; }


            var jsonContent = await client.PostAsync(linkPost, content);
            if (jsonContent.IsSuccessStatusCode)
            {
                JObject jsonResponse = JObject.Parse(await jsonContent.Content.ReadAsStringAsync());
                string incomeLink = jsonResponse["result"]["bot_check_url"].ToString();
                string incomeId = jsonResponse["result"]["check_id"].ToString();
                return (incomeLink, incomeId);
            }else
            {
                return (null, null);
            }

        }

        public async Task<JObject> deletePayCheck(int id)
        {
            client.BaseAddress = new Uri("https://testnet-pay.crypt.bot/");
            client.DefaultRequestHeaders.Add("Crypto-Pay-API-Token", apiToken);

            //Задаем свойства
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("invoice_id", id.ToString()),
            });

            //Определяем тип CryptoApi
            string linkPost = "None";
            if (isBotTestnet == false) { linkPost = "https://pay.crypt.bot/api/deleteInvoice"; }
            if (isBotTestnet == true) { linkPost = "https://testnet-pay.crypt.bot/api/deleteInvoice"; }


            var jsonContent = await client.PostAsync(linkPost, content);


            if (jsonContent.IsSuccessStatusCode)
            {
                JObject jsonResponse = JObject.Parse(await jsonContent.Content.ReadAsStringAsync());
                return jsonResponse;
            }
            else { return null; }
        }

        public async Task<JObject> deleteIncomeCheck(int id)
        {
            client.BaseAddress = new Uri("https://testnet-pay.crypt.bot/");
            client.DefaultRequestHeaders.Add("Crypto-Pay-API-Token", apiToken);

            //Задаем свойства
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("check_id", id.ToString()),
            });

            //Определяем тип CryptoApi
            string linkPost = "None";
            if (isBotTestnet == false) { linkPost = "https://pay.crypt.bot/api/deleteCheck"; }
            if (isBotTestnet == true) { linkPost = "https://testnet-pay.crypt.bot/api/deleteCheck"; }


            var jsonContent = await client.PostAsync(linkPost, content);


            if (jsonContent.IsSuccessStatusCode)
            {
                JObject jsonResponse = JObject.Parse(await jsonContent.Content.ReadAsStringAsync());
                return jsonResponse;
            }
            else { return null; }
        }

        public async Task<JObject> getBalance()
        {
            string linkPost = "None";
            if (isBotTestnet == false) { linkPost = "https://pay.crypt.bot/"; }
            if (isBotTestnet == true) { linkPost = "https://testnet-pay.crypt.bot/"; }

            client.BaseAddress = new Uri(linkPost);
            client.DefaultRequestHeaders.Add("Crypto-Pay-API-Token", apiToken);

            JObject vallue = JObject.Parse(await client.GetStringAsync("api/getBalance"));

            return vallue;
        }
        public async Task<JObject> getExchangeRates()
        {
            string linkPost = "None";
            if (isBotTestnet == false) { linkPost = "https://pay.crypt.bot/"; }
            if (isBotTestnet == true) { linkPost = "https://testnet-pay.crypt.bot/"; }

            client.BaseAddress = new Uri(linkPost);
            client.DefaultRequestHeaders.Add("Crypto-Pay-API-Token", apiToken);

            JObject vallue = JObject.Parse(await client.GetStringAsync("api/getExchangeRates"));

            return vallue;
        }
    }
}

