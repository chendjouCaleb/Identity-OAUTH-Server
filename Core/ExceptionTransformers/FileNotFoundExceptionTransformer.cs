using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Identity.Core.ExceptionTransformers
{
    public class FileNotFoundExceptionTransformer
    {
        public Type[] ExceptionTypes => new Type[] { typeof(FileNotFoundException) };

        public ErrorResponseModel BuildErrorModel(Exception ex)
        {
            FileNotFoundException exception = ex as FileNotFoundException;
            return new ErrorResponseModel
            {
                StatusCode = 404,
                Message = $"Le fichier { exception.FileName } est introuvable",
                Type = ex.GetType().Name
            };
        }

        public void EditContext(HttpContext context, Exception ex)
        {

        }
    }
}
