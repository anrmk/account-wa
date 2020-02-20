using Microsoft.AspNetCore.Mvc;

namespace Web.ViewModels {
    public class SearchViewModel {
        [FromQuery(Name = "search")]
        public string Search { get; set; } = "";

        [FromQuery(Name = "sort")]
        public string Sort { get; set; }

        [FromQuery(Name = "order")]
        public string Order { get; set; }

        [FromQuery(Name = "offset")]
        public int Offset { get; set; } = 0;

        [FromQuery(Name = "limit")]
        public int Limit { get; set; } = 10;
    }
}
