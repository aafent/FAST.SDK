using FAST.Types;

namespace FAST.Services.Models
{

    //
    // (!) read this: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters
    //


    /// <summary>
    /// Response model, containing a model and a collection 
    /// </summary>
    /// <typeparam name="modelType">The model type</typeparam>
    /// <typeparam name="collectionType">The collection type</typeparam>
    public class modelAndCollectionResponseModel<modelType, collectionType> : baseResponseModel<modelType, collectionType>
                                where modelType : class, new() where collectionType : class
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public modelAndCollectionResponseModel():base() 
        {
        }

        /// <summary>
        /// Constructor with model initialization
        /// </summary>
        /// <param name="model"></param>
        public modelAndCollectionResponseModel(modelType model):this()
        {
            this.model = model;
            this.processErrorsBeforeReturn();
        }
    }


    /// <summary>
    /// Response model, containing only a model
    /// </summary>
    /// <typeparam name="modelType">The model type</typeparam>
    public class modelResponseModel<modelType> : baseResponseModel<modelType, noModel>
        where modelType : class, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public modelResponseModel():base(){ }

        /// <summary>
        /// Constructor with model initialization
        /// </summary>
        /// <param name="model"></param>
        public modelResponseModel(modelType model):base() 
        { 
            this.model = model; 
            this.processErrorsBeforeReturn();
        }
    }

    /// <summary>
    /// Response model, containing nothing, caring just errors and the elementary model
    /// </summary>
    public class voidResponseModel : baseResponseModel<noModel, noModel>
    {

        public static void toVoid(object obj) { }
    }


    /// <summary>
    /// Response model, containing just a collection
    /// </summary>
    /// <typeparam name="collectionType">The collection type</typeparam>
    public class collectionResponseModel<collectionType> : baseResponseModel<noModel, collectionType>
        where collectionType : class, new()
    {
    }


    /// <summary>
    /// Response model, containing a string value as model
    /// </summary>
    public class stringResponseModel : baseResponseModel<anyType<string>, noModel>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public stringResponseModel()
        {
        }

        /// <summary>
        /// Constructor with string initialization
        /// </summary>
        /// <param name="value"></param>
        public stringResponseModel(string value)
        {
            this.model=value;
        }

        /// <summary>
        /// Returns the string representation of the model's value, or null if the model is null.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => this.model==null?null:this.model.V.ToString();
    }

    /// <summary>
    /// Response model, containing a datetime value as model
    /// </summary>
    public class dateTimeResponseModel : baseResponseModel<anyType<DateTime>, noModel>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public dateTimeResponseModel()
        {
        }

        /// <summary>
        /// Constructor with datetime initialization
        /// </summary>
        /// <param name="value"></param>
        public dateTimeResponseModel(DateTime value)
        {
            this.model = value;
        }
    }


    /// <summary>
    /// Response model, containing a file stream as a model
    /// </summary>
    public class fileStreamResponseModel : baseResponseModel<object, noModel>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public fileStreamResponseModel()
        {
        }

        /// <summary>
        /// Constructor with filestream initialization
        /// </summary>
        /// <param name="value"></param>
        public fileStreamResponseModel(FileStream value)
        {
            this.model = value;
        }

        /// <summary>
        /// Returns the filestream contained in the model, casting it to FileStream.
        /// </summary>
        /// <returns></returns>
        public FileStream toFileStream()
        {
            return (FileStream)this.model;
        }

    }

}

