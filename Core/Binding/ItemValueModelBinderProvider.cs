using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Everest.Identity.Core.Binding
{
    public class ItemValueModelBinderProvider:IModelBinderProvider
    {
        /// <inheritdoc />
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var bindingInfo = context.BindingInfo;
            if (bindingInfo.BindingSource == null ||
                !bindingInfo.BindingSource.CanAcceptDataFrom(BindingSource.Custom))
            {
                return null;
            }

            var modelMetadata = context.Metadata;
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger<ItemValueModelBinderProvider>();

            if (!IsSimpleType(modelMetadata))
            {
                logger.LogError($"Cannot create item value model binder for type: {modelMetadata.ModelType}");
                return null;
            }

            // Since we are delegating the binding of the current model type to other binders, modify the
            // binding source of the current model type to a non-FromHeader binding source in order to avoid an
            // infinite recursion into this binder provider.
            var nestedBindingInfo = new BindingInfo(bindingInfo)
            {
                BindingSource = BindingSource.ModelBinding
            };

            var innerModelBinder = context.CreateBinder(
                modelMetadata.GetMetadataForType(modelMetadata.ModelType),
                nestedBindingInfo);

            if (innerModelBinder == null)
            {
                return null;
            }

            return new ItemValueModelBinder();
        }

        // Support binding only to simple types or collection of simple types.
        private bool IsSimpleType(ModelMetadata modelMetadata)
        {
            var metadata = modelMetadata.ElementMetadata ?? modelMetadata;
            return !metadata.IsComplexType;
        }
    }
}
