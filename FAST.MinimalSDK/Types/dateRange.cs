namespace FAST.Types
{
    public class dateRange
    {
        private DateTime _from;
        private DateTime _to;

        public dateRange()
        {
        }
        public dateRange(DateTime from, DateTime to) : this()
        {
            this.from = from;
            this.to = to;
        }
        public dateRange(DateTime from):this()
        {
            this.from = from;
            this.to = from;
        }


        public DateTime from
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
            }
        }

        public DateTime to
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
            }
        }

        public DateTime toEndOfMonth
        {
            get
            {
                return datesHelper.endOfMonth(_to);
            }
            set
            {
                this._to = datesHelper.endOfMonth(value);
            }
        }

        public DateTime fromBeginOfMonth
        {
            get
            {
                return datesHelper.startOfMonth(_from);
            }
            set
            {
                this._from = datesHelper.startOfMonth(value);
            }
        }

        public DateTime toEndOfYear
        {
            get
            {
                return datesHelper.endOfYear(_to);
            }
            set
            {
                this._to = datesHelper.endOfYear(value);
            }
        }

        public DateTime fromBeginOfYear
        {
            get
            {
                return datesHelper.startOfYear(_from);
            }
            set
            {
                this._from = datesHelper.startOfYear(value);
            }
        }

        public IEnumerable<DateTime> eachDay()
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public bool isBetween(DateTime date)
        {
            return datesHelper.isBetween(date, from, to, false);
        }
    
    }
}
