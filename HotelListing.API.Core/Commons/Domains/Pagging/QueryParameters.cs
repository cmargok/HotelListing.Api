﻿namespace HotelListing.API.Commos.Domains.Pagging
{
    public class QueryParameters
    {
        public int _pageSize = 15;
        public int StartIndex { get; set; }
        public int PageNumber { get; set; }
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }
    }
}
