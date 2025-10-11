using FAST.Core;
using FAST.Core.Models;

namespace FAST.Services.Models
{

    /// <summary>
    /// Base class for response models that include a main model and a collection of row models.
    /// </summary>
    /// <typeparam name="modelType"></typeparam>
    /// <typeparam name="rowsModelType"></typeparam>
    public abstract class baseResponseModel<modelType, rowsModelType> : elementaryModelClass, IdataFoundQuestion, ImultiErrorCarrier, 
                                                                                              IerrorCarrier, Irowset<rowsModelType>,
                                                                                              IpropertyModel<modelType>
        where modelType : class, new() where rowsModelType : class
    {

        /// <summary>
        /// Constructor that initializes the model and data properties if they implement IniceToInstantiateOnConstructor.
        /// </summary>
        public baseResponseModel()
        {
            if ( typeof(modelType).GetInterface(nameof(IniceToInstantiateOnConstructor)) != null ) 
            {
                if(this.model==null) this.model = new();
            }
            if (typeof(rowsModelType).GetInterface(nameof(IniceToInstantiateOnConstructor)) != null) 
            {
                if (this.data== null) this.data = new();
            }
        }

        /// <summary>
        /// The main model of the response.
        /// </summary>
        public modelType model { get; set; }

        /// <summary>
        /// A list of row models included in the response.
        /// </summary>
        public List<rowsModelType> data { get; set; }

        /// <summary>
        /// Processes errors from the model and data before returning, aggregating them into the base error properties.
        /// </summary>
        /// <param name="items"></param>
        public new void processErrorsBeforeReturn(params IerrorCarrier[] items)
        {
            this.hasError=false;
            if ( model != null )
            { 
                if ( model is IerrorCarrier ) 
                {
                    if ( ((IerrorCarrier)model).hasError ) base.processErrorsBeforeReturn(((IerrorCarrier)model));
                }
            }
            if ( data!=null )
            {
                if (data.Count > 0 )
                {
                    if (data[0] is IerrorCarrier )
                    {
                        foreach ( var row in data)
                        {
                            if (((IerrorCarrier)row).hasError) base.processErrorsBeforeReturn(((IerrorCarrier)row));
                        }
                    }
                }
            }
            base.processErrorsBeforeReturn(items);
        }

    }

}

