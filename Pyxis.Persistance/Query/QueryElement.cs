using System;

namespace Pyxis.Persistance.Query
{
    public class QueryElement
    {
        private readonly Tuple<string, FieldComparison, object> _element;
        public QueryElement(string field, FieldComparison comparison, object value)
        {
            _element = Tuple.Create(field, comparison, value);
        }

        public string Field {get { return _element.Item1; }}
        public FieldComparison Comparison {get { return _element.Item2; }}
        public object Value {get { return _element.Item3; }}

        public static QueryElement FieldEquals(string field, object value)
        {
            return new QueryElement(field, FieldComparison.Equals, value);
        }
        public static QueryElement FieldCompare(string field, FieldComparison comparison, object value)
        {
            return new QueryElement(field, comparison, value);
        }
    }
}
