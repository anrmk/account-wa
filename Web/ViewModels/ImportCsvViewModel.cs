using System.Collections.Generic;

namespace Web.ViewModels {
    public class ImportCsvViewModel {
        public List<ImportCsvColViewModel> Columns { get; set; }

        public List<string> HeadRow { get; set; }
        public List<ImportCsvRowViewModel[]> Rows { get; set; }
    }

    public class ImportCsvColViewModel {
        public int Index { get; set; }
        public string Name { get; set; }
    }

    public class ImportCsvRowViewModel {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
