using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Pyxis.Core.Id;
using Pyxis.Persistance.Query;

namespace Pyxis.Persistance.Azure
{
    public class TableStorePersistance : TableStorePersistance<DynamicTableEntity> 
    {
        public TableStorePersistance(CloudTableClient client, IDomainEntityMapper mapper, int batchLimitThreshold = 100, int parallelSaveThreshold = 3) : 
            base(client, mapper, batchLimitThreshold, parallelSaveThreshold)
        {
        }
    }

    public class TableStorePersistance<TE> : IPersistanceQuery, IPersistanceStore where TE : class, ITableEntity, new()
    {
        private readonly CloudTableClient _tableClient;
        private readonly IDomainEntityMapper _mapper;
        private readonly int _batchLimitThreshold = 100;
        private readonly int _parallelSaveThreshold = 3;
        private readonly IIdGenerator _idGenerator = new GuidIdGenerator();

        public TableStorePersistance(CloudTableClient client, IDomainEntityMapper mapper, int batchLimitThreshold = 100, int parallelSaveThreshold = 3)
        {
            if (batchLimitThreshold > 100 || batchLimitThreshold < 1) throw new ArgumentException("Batch threshold should be between 1 and 100 inclusive");
            _tableClient = client;
            _mapper = mapper;
            _batchLimitThreshold = batchLimitThreshold;
            _parallelSaveThreshold = parallelSaveThreshold;
        }

        public void Delete<T>(string[] keys, string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) return;
            var table = GetTable<T>();
            BatchDelete(context.ToLower(), keys.Where(x=> !string.IsNullOrEmpty(x)).Select(x=> x.ToLower()), table);
        }

        public void Delete<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(context)) return;
            Delete<T>(new[] { key.ToLower() }, context.ToLower());
        }

        public bool Any<T>(string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) return false;
            var table = GetTable<T>();
            var partitionQuery = GetPartitionQuery(context);
            var result = table.ExecuteQuery(partitionQuery).Any();
            return result;
        }

        private TableQuery<TE> GetPartitionQuery(string context)
        {
            var contextQueryElement = QueryElement.FieldEquals("PartitionKey", context.ToLower());
            return BuildTableQuery(contextQueryElement);
        }

        public void Purge<T>(string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) return;
            var table = GetTable<T>();
            DeleteContext(context, table);
        }

        public void Purge(string context = "default")
        {
            if (string.IsNullOrEmpty(context)) return;
            var tables = _tableClient.ListTables();
            foreach (var table in tables)
            {
                DeleteContext(context, table);
            }
        }

        private void DeleteContext(string context, CloudTable table)
        {
            var partitionQuery = GetPartitionQuery(context);
            var keys = table.ExecuteQuery(partitionQuery).Select(x => x.RowKey).ToArray();
            BatchDelete(context, keys, table);
        }

        private void BatchDelete(string context, IEnumerable<string> keys, CloudTable table)
        {
            var count = 0;
            var batchOperation = new TableBatchOperation();
            foreach (var key in keys)
            {
                var toDelete = new TableEntity { RowKey = key, PartitionKey = context, ETag = "*" };
                if (count == _batchLimitThreshold)
                {
                    table.ExecuteBatch(batchOperation);
                    batchOperation = new TableBatchOperation();
                    count = 0;
                }
                batchOperation.Delete(toDelete);
                count++;
            }
            if (batchOperation.Any())
            {
                table.ExecuteBatch(batchOperation);
            }
        }

        public IEnumerable<T> GetSome<T>(int count, string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) return new T[0];
            var table = GetTable<T>();
            var partitionQuery = GetPartitionQuery(context);
            var result = table.ExecuteQuery(partitionQuery).Take(count);
            return _mapper.Map<TE, T>(result);
        }

        public void Save<T>(T content, string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) throw new ArgumentException();
            var table = GetTable<T>();
            var contentEntity = _mapper.Map<T, TE>(content);
            GenerateKeyIfRequired(content, contentEntity);
            contentEntity.RowKey = contentEntity.RowKey.ToLower();
            contentEntity.PartitionKey = context.ToLower();
            var insertOperation = TableOperation.InsertOrReplace(contentEntity);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        private void GenerateKeyIfRequired<T>(T content, TE contentEntity) where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(contentEntity.RowKey))
            {
                if (content != null && string.IsNullOrEmpty(content.Id))
                {
                    content.Id = _idGenerator.GenerateId();
                }
                contentEntity.RowKey = content.Id;
            }
        }

        public void Save<T>(IEnumerable<T> content, string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) throw new ArgumentException();
            if (!content.Any()) return;

            var mapped = _mapper.Map<T, TE>(content).ToArray();
            var threadCount = mapped.Count() > _batchLimitThreshold*_parallelSaveThreshold ? -1 : 1;
            PerformSave<T>(context, mapped, threadCount);
        }

        private void PerformSave<T>(string context, IEnumerable<TE> mapped, int threadCount) 
        {
            var chunks = Split(mapped, _batchLimitThreshold);
            Parallel.ForEach( chunks, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, chunk =>
            {
                var client = new CloudTableClient(_tableClient.StorageUri, _tableClient.Credentials);
                var table = GetTable<T>(client);
                var batchOperation = new TableBatchOperation();
                foreach (var item in chunk)
                {
                    item.RowKey= item.RowKey.ToLower();
                    item.PartitionKey = context.ToLower();
                    batchOperation.InsertOrReplace(item);
                }
                table.ExecuteBatch(batchOperation);
            });
        }

        private IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> source, int chunkSize)
        {
            var copiedSource = new List<T>(source);
            var chunkList = new List<List<T>>();

            while (copiedSource.Any())
            {
                var copied = copiedSource.Select(x => x).Take(chunkSize).ToList();
                copiedSource.RemoveRange(0, copied.Count);
                chunkList.Add(copied);
            }
            return chunkList;
        }

        public T Get<T>(string key, string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(context)) return null;
            var queryString = QueryElement.FieldEquals("RowKey", key.ToLower());
            return Query<T>(new[] { queryString }, int.MaxValue, context.ToLower()).FirstOrDefault();
        }

        public IEnumerable<T> GetAll<T>(string context = "default") where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) return new T[0];
            return Query<T>( new QueryElement[0], int.MaxValue, context.ToLower());
        }

        public IEnumerable<T> Query<T>(QueryElement queryField, int resultLimit = int.MaxValue, string context = "default")
            where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) return new T[0];
            return Query<T>(new [] {queryField}, resultLimit, context.ToLower());
        }

        public IEnumerable<T> Query<T>(IEnumerable<QueryElement> queryFields, int resultLimit = int.MaxValue, string context = "default")
            where T : class, IIdentifiable
        {
            if (string.IsNullOrEmpty(context)) return new T[0];
            var table = GetTable<T>();
            var partitionString = QueryElement.FieldEquals("PartitionKey", context.ToLower());
            var allElements = new List<QueryElement>(queryFields);
            allElements.Add(partitionString);
            var query = BuildTableQuery(allElements.ToArray());
            var result = table.ExecuteQuery(query).Take(resultLimit);
            var mapped = _mapper.Map<TE, T>(result);
            return mapped;
        }

        private TableQuery<TE> BuildTableQuery(params QueryElement[] queryFields)
        {
            var combined = string.Empty;
            foreach (var queryField in queryFields)
            {
                var condition = GenerateFilterCondition(queryField);
                combined = string.IsNullOrWhiteSpace(combined)
                    ? condition
                    : TableQuery.CombineFilters(combined, TableOperators.And, condition);
            }
            return new TableQuery<TE>().Where(combined);
        }

        private string GenerateFilterCondition(QueryElement queryField)
        {
            if (queryField.Value is bool) return TableQuery.GenerateFilterConditionForBool(queryField.Field, GetComparison(queryField.Comparison), (bool)queryField.Value);
            if (queryField.Value is int) return TableQuery.GenerateFilterConditionForInt(queryField.Field, GetComparison(queryField.Comparison),(int) queryField.Value);
            if (queryField.Value is long) return TableQuery.GenerateFilterConditionForLong(queryField.Field, GetComparison(queryField.Comparison), (long)queryField.Value);
            if (queryField.Value is double) return TableQuery.GenerateFilterConditionForDouble(queryField.Field, GetComparison(queryField.Comparison), (double)queryField.Value);
            if (queryField.Value is string) return TableQuery.GenerateFilterCondition(queryField.Field, GetComparison(queryField.Comparison), (string)queryField.Value);
            if (queryField.Value is Guid) return TableQuery.GenerateFilterConditionForGuid(queryField.Field, GetComparison(queryField.Comparison), (Guid)queryField.Value);
            if (queryField.Value is DateTime) return TableQuery.GenerateFilterConditionForDate(queryField.Field, GetComparison(queryField.Comparison), (DateTime)queryField.Value);
            throw new ArgumentException("Unsupported field value type");
        }

        private string GetComparison(FieldComparison item)
        {
            if (item == FieldComparison.Equals) return QueryComparisons.Equal;
            if (item == FieldComparison.Greater) return QueryComparisons.GreaterThan;
            if (item == FieldComparison.GreaterEquals) return QueryComparisons.GreaterThanOrEqual;
            if (item == FieldComparison.Less) return QueryComparisons.LessThan;
            if (item == FieldComparison.LessOrEquals) return QueryComparisons.LessThanOrEqual;
            if (item == FieldComparison.Different) return QueryComparisons.NotEqual;
            return null;
        }

        private CloudTable GetTable<T>(CloudTableClient client = null)
        {
            // Create the table if it doesn't exist.
            var table = client == null ? _tableClient.GetTableReference(typeof(T).Name) : client.GetTableReference(typeof(T).Name);
            if (!table.Exists())
            {
                table.Create();
            }

            return table;
        }
    }
}