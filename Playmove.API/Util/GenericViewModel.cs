

namespace Playmate.Util
{
    public abstract class GenericViewModel<TModel, TViewModel> : IGenericViewModel<TModel, TViewModel>
    {
        public virtual TViewModel Cast(TModel model)
        {
            throw new NotImplementedException();
        } 
        
        public virtual TModel Cast(TViewModel model)
        {
            throw new NotImplementedException();
        }
    }


    public interface IGenericViewModel<TModel, TViewModel>
    {
        TViewModel Cast(TModel model);

        TModel Cast(TViewModel model);


    }
}
