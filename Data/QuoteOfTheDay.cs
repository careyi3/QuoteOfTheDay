using System;
using System.Collections.Generic;

namespace QuoteOfTheDay.Data
{
    public class Quote
    {
        public string quote { get; set; }
        public string author { get; set; }
    }

    public class Contents
    {
        public IList<Quote> quotes { get; set; }
    }

    public class QuoteResponse
    {
        public Contents contents { get; set; }
    }
}