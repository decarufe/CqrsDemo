using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CqrsDemo.Services
{
    public interface IView<in TKey, TInstance>
    {
        IEnumerable<TInstance> GetAll();
        TInstance GetByKey(TKey id);
        void Add(TInstance instance);
        void Update(TInstance instance);
        void Delete(TInstance instance);
    }
}