﻿using System;
using System.Collections.Generic;

namespace Core.Extension {
    public class Pager<T> {
        public int TotalItems { get; private set; }
        public int CurrentPage { get; private set; }
        public int? PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set; }
        public IEnumerable<T> Items { get; private set; }
        public Dictionary<string, List<string>> Filter { get; set; } = new Dictionary<string, List<string>>();
        public string[] Params { get; set; }

        public Pager(IEnumerable<T> list, int totalItems, int? page, int? pageSize, params string[] args) {
            var t = (decimal)totalItems / (decimal)(pageSize ?? totalItems);
            var totalPages = (int)Math.Ceiling(t);
            var currentPage = page ?? 1;
            var startPage = currentPage - 5;
            var endPage = currentPage + 4;
            if(startPage <= 0) {
                endPage -= (startPage - 1);
                startPage = 1;
            }
            if(endPage > totalPages) {
                endPage = totalPages;
                if(endPage > 10) {
                    startPage = endPage - 9;
                }
            }

            TotalItems = totalItems;
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalPages = totalPages;
            StartPage = startPage;
            EndPage = endPage;
            Items = list;
            Params = args;
        }
    }

    //public class PagerQuery {
    //    public int Take { get; set; } = 20;
    //    public int Skip { get; set; } = 0;
    //    public List<PagerSortQuery> Sort { get; set; }
    //}

    //public class PagerSortQuery {
    //    public bool Desc { get; set; } = false;
    //    public string Selector { get; set; }
    //}


}
