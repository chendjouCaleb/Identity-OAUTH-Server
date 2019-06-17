using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Everest.Identity.Core.ExceptionTransformers
{
    public class ExceptionTransformerFactory
    {
        public IDictionary<Type, IExceptionTransformer> Transformers { get; set; } =
            new Dictionary<Type, IExceptionTransformer>();

        public ExceptionTransformerFactory()
        {
        }

        public void AddTransformer(IExceptionTransformer transformer)
        {
            foreach (Type type in transformer.ExceptionTypes)
            {
                Transformers.Add(type, transformer);
            }
        }

        public IExceptionTransformer GetExceptionTransformer(Type type)
        {
            IExceptionTransformer transformer;
            try
            {
                transformer = Transformers[type];
            }
            catch (KeyNotFoundException)
            {
                transformer = Transformers[typeof(Exception)];
            }
            return transformer;
        }
    }
}
