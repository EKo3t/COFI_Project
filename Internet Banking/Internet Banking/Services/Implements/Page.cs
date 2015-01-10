﻿namespace Internet_Banking.Services.Implements
{
    public class Page
    {
        public int Number { get; set; }
        public int Capacity { get; set; }
        public int Count { get; set; }
        public bool IsLast { get { return Number == Count; } }
        public bool IsFirst { get { return Number == 1; } }

        public Page()
        {
            Number = 0;
            Capacity = int.MaxValue;
            Count = 1;
        }
    }
}