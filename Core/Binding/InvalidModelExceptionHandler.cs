using Identity.Core.Binding;
using System;

namespace Identity.Core.ExceptionTransformers
{
    public class InvalidModelExceptionHandler : ExceptionTransformer
    {
        public override Type[] ExceptionTypes => new Type[] { typeof(InvalidModelException) };
        public override ErrorResponseModel BuildErrorModel(Exception ex)
        {
            InvalidModelException exception = ex as InvalidModelException;
            BindingErrorModel model = new BindingErrorModel(exception.ModelState);
            model.StatusCode = 400;
            model.Message = exception.Message;
            model.Type = ex.GetType().Name;
            return model;
        }
    }
}
