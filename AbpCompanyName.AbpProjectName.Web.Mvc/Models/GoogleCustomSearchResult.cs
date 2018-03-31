namespace AbpCompanyName.AbpProjectName.Web.Models
{
    public class GoogleCustomSearchResult
    {
        public string kind { get; set; }
        public Url url { get; set; }
        public Queries queries { get; set; }
        public ContextGoogle context { get; set; }
        public Searchinformation searchInformation { get; set; }
        public Item[] items { get; set; }
    }

    public class Url
    {
        public string type { get; set; }
        public string template { get; set; }
    }

    public class Queries
    {
        public Request[] request { get; set; }
        public Nextpage[] nextPage { get; set; }
    }

    public class Request
    {
        public string title { get; set; }
        public string totalResults { get; set; }
        public string searchTerms { get; set; }
        public int count { get; set; }
        public int startIndex { get; set; }
        public string inputEncoding { get; set; }
        public string outputEncoding { get; set; }
        public string safe { get; set; }
        public string cx { get; set; }
    }

    public class Nextpage
    {
        public string title { get; set; }
        public string totalResults { get; set; }
        public string searchTerms { get; set; }
        public int count { get; set; }
        public int startIndex { get; set; }
        public string inputEncoding { get; set; }
        public string outputEncoding { get; set; }
        public string safe { get; set; }
        public string cx { get; set; }
    }

    public class ContextGoogle
    {
        public string title { get; set; }
    }

    public class Searchinformation
    {
        public float searchTime { get; set; }
        public string formattedSearchTime { get; set; }
        public string totalResults { get; set; }
        public string formattedTotalResults { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string title { get; set; }
        public string htmlTitle { get; set; }
        public string link { get; set; }
        public string displayLink { get; set; }
        public string snippet { get; set; }
        public string htmlSnippet { get; set; }
        public string cacheId { get; set; }
        public string formattedUrl { get; set; }
        public string htmlFormattedUrl { get; set; }
        public Pagemap pagemap { get; set; }
    }

    public class Pagemap
    {
        public Place[] place { get; set; }
        public Postaladdress[] postaladdress { get; set; }
        public Localbusiness[] localbusiness { get; set; }

        public Cse_Thumbnail[] cse_thumbnail { get; set; }
        public Breadcrumb[] breadcrumb { get; set; }
        public Review[] review { get; set; }
        public Hreview[] hreview { get; set; }
        public Metatag[] metatags { get; set; }
        public Review1[] Review { get; set; }
        public Cse_Image[] cse_image { get; set; }
        public Hreviewaggregate[] hreviewaggregate { get; set; }
        public Restaurant[] restaurant { get; set; }
        public Aggregaterating[] aggregaterating { get; set; }
        public Listitem[] listitem { get; set; }
    }

    public class Cse_Thumbnail
    {
        public string width { get; set; }
        public string height { get; set; }
        public string src { get; set; }
    }

    public class Breadcrumb
    {
        public string url { get; set; }
        public string title { get; set; }
    }

    public class Review
    {
        public string reviewer { get; set; }
        public string reviewdate { get; set; }
        public string ratingstars { get; set; }
        public string image_url { get; set; }
    }

    public class Hreview
    {
        public string summary { get; set; }
        public string dtreviewed { get; set; }
        public string description { get; set; }
    }

    public class Metatag
    {
        public string referrer { get; set; }
        public string applicationname { get; set; }
        public string msapplicationtilecolor { get; set; }
        public string msapplicationtileimage { get; set; }
        public string msapplicationtooltip { get; set; }
        public string msapplicationstarturl { get; set; }
        public string msapplicationwindow { get; set; }
        public string msapplicationtask { get; set; }
        public string ogtitle { get; set; }
        public string ogdescription { get; set; }
        public string ogimage { get; set; }
        public string ogurl { get; set; }
        public string ogtype { get; set; }
        public string playfoursquarenumber_of_tips { get; set; }
        public string playfoursquarelocationlatitude { get; set; }
        public string playfoursquarelocationlongitude { get; set; }
        public string playfoursquareappears_on_lists { get; set; }
        public string ogsite_name { get; set; }
        public string fbapp_id { get; set; }
        public string twittersite { get; set; }
        public string twittercreator { get; set; }
        public string twittercard { get; set; }
        public string twitterurl { get; set; }
        public string twitterdescription { get; set; }
        public string twittertitle { get; set; }
        public string twitterimage { get; set; }
        public string aliphoneurl { get; set; }
        public string aliphoneapp_store_id { get; set; }
        public string aliphoneapp_name { get; set; }
        public string twitterappnameiphone { get; set; }
        public string twitterappidiphone { get; set; }
        public string twitterappurliphone { get; set; }
        public string alipadurl { get; set; }
        public string alipadapp_store_id { get; set; }
        public string alipadapp_name { get; set; }
        public string twitterappnameipad { get; set; }
        public string twitterappidipad { get; set; }
        public string twitterappurlipad { get; set; }
        public string alandroidurl { get; set; }
        public string alandroidpackage { get; set; }
        public string alandroidapp_name { get; set; }
        public string twitterappnamegoogleplay { get; set; }
        public string twitterappidgoogleplay { get; set; }
    }

    public class Review1
    {
        public string author { get; set; }
        public string name { get; set; }
        public string datePublished { get; set; }
        public string reviewBody { get; set; }
    }

    public class Cse_Image
    {
        public string src { get; set; }
    }

    public class Hreviewaggregate
    {
        public string pricerange { get; set; }
        public string votes { get; set; }
    }

    public class Restaurant
    {
        public string name { get; set; }
    }

    public class Aggregaterating
    {
        public string ratingvalue { get; set; }
        public string ratingcount { get; set; }
        public string bestrating { get; set; }
    }

    public class Listitem
    {
        public string item { get; set; }
        public string name { get; set; }
        public string position { get; set; }
    }

    public class Place
    {
        public string name { get; set; }
    }

    public class Postaladdress
    {
        public string streetaddress { get; set; }
        public string addresslocality { get; set; }
    }

    public class Localbusiness
    {
        public string name { get; set; }
        public string pricerange { get; set; }
        public string telephone { get; set; }
    }

}