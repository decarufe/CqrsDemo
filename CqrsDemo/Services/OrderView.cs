using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CqrsDemo.Services
{
    public class OrderView : IView<Guid, OrderDto>
    {
        private static readonly Dictionary<Guid, OrderDto> _items = new Dictionary<Guid, OrderDto>();
        private static int _lastKey;

        public IEnumerable<OrderDto> GetAll()
        {
            return _items.Values;
        }

        private Func<OrderDto, Guid> Key
        {
            get { return dto => dto.Id; }
        }

        public OrderDto GetByKey(Guid id)
        {
            return _items[id];
        }

        public void Add(OrderDto instance)
        {
            var orderDto = Clone(instance);
            _items.Add(Key(instance), orderDto);
        }

        public void Update(OrderDto instance)
        {
            _items[Key(instance)] = Clone(instance);
        }

        public void Delete(OrderDto instance)
        {
            _items.Remove(Key(instance));
        }

        private OrderDto Clone(OrderDto source)
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<OrderDto>(json);
        }
    }
}