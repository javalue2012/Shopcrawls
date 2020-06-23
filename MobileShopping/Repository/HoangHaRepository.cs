using System;
using System.Collections.Generic;
using System.Linq;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using MobileShopping.Model;
using MobileShopping.Utility;

namespace MobileShopping.Repository
{
    public class HoangHaRepository : IShopRepository
    {
        //private string baseLink = "https://hoanghamobile.com";
        //private string mobileListLink = "/dien-thoai-di-dong-c14.html?p={0}";
        //private string searchLink = "/tim-kiem.html?p={0}&kwd={1}";

        //private string baseLink = "https://fptshop.com.vn";
        //private string mobileListLink = "/dien-thoai?sort=ban-chay-nhat?p={0}";
        //private string searchLink = "/tim-kiem&kwd={1}";


        private string baseLink = "https://edition.cnn.com";
        private string mobileListLink = "/search";
        private string searchLink = "/search?size=10&q=trump&kwd={1}";

        public ProductDetail GetProductDetail(string link)
        {
            var _web = HtmlWebSingleton.GetInstance();
            HtmlDocument document = _web.Load(baseLink + link);
            // set detail where we take the first class contain all item we want to selectoe
            //var detail = document.DocumentNode.QuerySelector(".product-details");
            var detail = document.DocumentNode.QuerySelector(".f-wrap");
            var product = new ProductDetail();
            //product.Description = HtmlEntity.DeEntitize(detail.QuerySelector(".info .simple-prop").InnerHtml.InsertNewLine().RemoveHtmlTag());
            product.Description = HtmlEntity.DeEntitize(detail.QuerySelector(".fs-dtbox main_spec .fs-tsright >li").InnerHtml.InsertNewLine().RemoveHtmlTag());
            //product.Description = detail.QuerySelector(".info .simple-prop").InnerHtml;
            product.ProductName = HtmlEntity.DeEntitize(detail.QuerySelector("h1").InnerText);
            product.Price = detail.QuerySelector(".product-price span").InnerText;
            var links = detail.QuerySelectorAll("#slider1_container > div > div  img:first-child").ToList();
            foreach (var item in links)
            {
                product.ImageLinkLst.Add(item.Attributes["src"].Value);
            }
            return product;
        }

        public List<Product> GetProductListByIndex(string search = "", int index = 1)
        {
            var web = HtmlWebSingleton.GetInstance();
            HtmlDocument document = web.Load(string.IsNullOrEmpty(search) ? string.Format(baseLink + mobileListLink, index) : string.Format(baseLink + searchLink, index, search));
           // var threadItems = document.DocumentNode.QuerySelectorAll(".list-item").ToList();
            //var threadItems = document.DocumentNode.QuerySelectorAll(".fs-lpil").ToList();
            var threadItems = document.DocumentNode.QuerySelectorAll(".cnn-search__results-list").ToList();
            List<Product> items = new List<Product>();
            foreach (var item in threadItems)
            {
                //var productName = HtmlEntity.DeEntitize(item.QuerySelector(".product-name a").InnerText);
                //var price = item.QuerySelector(".product-price").InnerText;
                //var image = item.QuerySelector(".mosaic-block .mosaic-backdrop img").Attributes["src"].Value;
                //var path = item.QuerySelector(".mosaic-block > a").Attributes["href"].Value;
                //var productName = item.QuerySelector(".fs-lpil-img").Attributes["title"].Value;
                var productName = HtmlEntity.DeEntitize(item.QuerySelector(".fs-lpil-if .fs-lpil-name >a").InnerText);
                var price = item.QuerySelector(".fs-lpil-if .fs-lpil-price >p").InnerText;
                var image = item.QuerySelector(".fs-lpil-img img").Attributes["data-original"].Value;
                var path = item.QuerySelector(".fs-lpil-img").Attributes["href"].Value;
                items.Add(new Product()
                {
                    ProductName = productName,
                    Price = price,
                    Thumbnail = image,
                    Link = path
                });
            }
            return items;
        }

        public int GetTotalProduct(string search)
        {
            var _web = HtmlWebSingleton.GetInstance();
            HtmlDocument document = _web.Load(string.IsNullOrEmpty(search) ? string.Format(baseLink + mobileListLink, 1) : string.Format(baseLink + searchLink, 1, search));
            if (document.DocumentNode.QuerySelectorAll(".pageing-container .paging a").Count() != 0)
            {
                var link = document.DocumentNode.QuerySelectorAll(".pageing-container .paging a").Last().Attributes["href"].Value;
                var total = link.Substring(link.LastIndexOf("=") + 1);
                return Convert.ToInt32(total);
            }
            return 1;
        }
    }
}
